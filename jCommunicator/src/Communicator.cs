using mLogger;
using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Sftp;
using System.Globalization;
using System.Text.RegularExpressions;

namespace jCommunicator
{ 
//
// Manages and SSH connection to a Cluster Hub, and tunnels to individual Nodes for file operations and command execution.
    
    public enum ServiceStatus
    {
        Active = 0,
        Activating = 1,
        Deactivating = 2,
        Inactive = 3,
        Failed = 4,
        Error = 5
    }
    public readonly struct SSHCheckResult
    {
        public bool Success { get; }
        public Exception? Exception { get; }
        public long checkTimespan { get; }

        public SSHCheckResult(bool success, Exception? exception, long checkTime)
        {
            Success = success;
            Exception = exception;
            checkTimespan = checkTime;
        }
    }
    public struct FileInfo
    {
        public string Name { get; }
        public long Length { get; }
        public DateTime LastWriteTime { get; }

        public FileInfo(string name, long length, DateTime lastWriteTime)
        {
            Name = name;
            Length = length;
            LastWriteTime = lastWriteTime;
        }
    }
    public class Communicator : IAsyncDisposable
    {
        private class NodeInfo
        {
            public string Host { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int LocalPort { get; set; }
            public ForwardedPortLocal Port { get; set; }
            public SftpClient Sftp { get; set; }

            public string ConnectionHost => LocalPort == 22 ? Host : "127.0.0.1";

            public SftpClient CreateSftpClient()
            {
                return new SftpClient(
                    "127.0.0.1",
                    LocalPort,
                    Username,
                    Password);
            }
        }

        public readonly string _host;
        public readonly string _username;
        private readonly string _password;

        private SshClient _sshClient;
        private SftpClient _sftpClient;
        private readonly Dictionary<string, NodeInfo> _nodeConnections = new();

        private readonly SemaphoreSlim _connectLock = new SemaphoreSlim(1, 1);

        public bool IsConnected => _sshClient != null && _sshClient.IsConnected;

        Logger logger = Logger.Instance;

        //Constructor/Destructor
        public Communicator(string host, string username, string password) //, RichTextBox outputBox = null, Action<string> logAction = null)
        {
            logger.AddSource("Communicator");
            logger.LogHeading(LogLevel.INFO, "Communicator", $"Starting Communicator for {host}");

            _host = host;
            _username = username;
            _password = password;
        }
        public void Dispose()
        {
            logger.LogHeading(LogLevel.INFO, "Communicator", $"Closing Communicator for {_host}");
            DisconnectAsync();
        }

        #region Connection and Checks
        //Connection Methods
        public async Task<bool> ConnectAsync()
        {
            await _connectLock.WaitAsync();
        
            try
            {
                if (IsConnected) return true;

                CancellationToken sshToken = new CancellationToken();
                CancellationToken sftpToken = new CancellationToken();

                // Initialize SSH client and node tunnels
                _sshClient = new SshClient(_host, _username, _password);

                logger.Log(LogLevel.INFO, "Communicator", $"Connecting to Cluster Hub at {_username}@{_host}. Please wait...");
                await _sshClient.ConnectAsync(sshToken);
                if (!_sshClient.IsConnected)
                {
                    throw new InvalidOperationException("Failed to connect to SSH client.");
                }
                _sftpClient = new SftpClient(_host, _username, _password);
                await _sftpClient.ConnectAsync(sftpToken);
                await RebuildNodeTunnelsAsync();
                logger.Log(LogLevel.INFO, "Communicator", $"Connected to {_host}");

                return true;
            }
            catch
            {
                _sftpClient?.Dispose();
                _sshClient?.Dispose();

                _sftpClient = null;
                _sshClient = null;

                throw;
            }
            finally
            {
                _connectLock.Release();
            }
        }
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }
        public async Task DisconnectAsync()
        {
            await _connectLock.WaitAsync();

            try
            {
                foreach (NodeInfo node in _nodeConnections.Values)
                {
                    if (node.Sftp != null)
                    {
                        if (node.Sftp.IsConnected)
                            node.Sftp.Disconnect();

                        node.Sftp.Dispose();
                        node.Port.Stop();
                        node.Port.Dispose();
                        node.Sftp = null;
                    }
                }
                //_nodeConnections.Clear();

                if (_sftpClient != null && _sftpClient.IsConnected)
                {
                    _sftpClient.Disconnect();
                    logger.Log(LogLevel.INFO, "Communicator", "SFTP client disconnected");
                }
                _sftpClient?.Dispose();
                _sftpClient = null;

                if (_sshClient != null && _sshClient.IsConnected)
                {
                    _sshClient.Disconnect();
                    logger.Log(LogLevel.INFO, "Communicator", "SSH client disconnected");
                }
                _sshClient?.Dispose();
                _sshClient = null;
            }
            finally
            {
                _connectLock.Release();
            }
        }
        public async Task CheckConnectionAsync()
        {
            if (!IsConnected)
            {
                await ConnectAsync();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before executing node command.");
                    throw new InvalidOperationException("Not connected to hub.");
                }
            }
        }
        public async Task<SSHCheckResult> checkSSHDeviceAsync(bool verbose)
        {
            SSHCheckResult returnValue = new SSHCheckResult();
            bool allConnected = true;

            //foreach (var (device, communicator) in Settings.All.Hubs.Zip(hubs, (d, c) => (d, c)))
            //{
            
            if (verbose)
                logger.Log(LogLevel.INFO, "Communicator", $"Checking SSH connection to device {_host} as {_username}...\n");

            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                bool connected = await ConnectAsync();
                sw.Stop(); 
                
                if (verbose)
                    logger.Log(LogLevel.INFO, "Communicator", $"Total connection attempt time for {_host}: {sw.ElapsedMilliseconds} ms.\n");

                return new SSHCheckResult(connected, null, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();

                if (verbose)
                    logger.Log(LogLevel.INFO, "Communicator", $"Total connection attempt time for {_host}: {sw.ElapsedMilliseconds} ms.\n");

                return new SSHCheckResult(false, ex, sw.ElapsedMilliseconds);
            }
        }
        public async Task<int> AddNodeTunnelAsync(string nodeHost, string nodeUsername, string nodePassword, bool verbose = false)
        {
            if (nodeHost == null || nodeHost == "")
                throw new ArgumentNullException("nodeHost passed as null");
            if (nodeUsername == null || nodeUsername == "")
                throw new ArgumentNullException("nodeUsername passed as null");
            if (nodePassword == null)
                throw new ArgumentNullException("nodePassword passed as null");

            if (!IsConnected)
            {
                await ConnectAsync();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before initializing Node SFTP.");
                    throw new InvalidOperationException("Failed to connect before initializing Node SFTP.");
                }
            }

            if (_nodeConnections.ContainsKey(nodeHost))
                return _nodeConnections[nodeHost].LocalPort; // Already initialized

            try
            {
                // Pick a unique local port per node (2222, 2223, etc.)
                int localPort = 2200 + _nodeConnections.Count;

                var port = new ForwardedPortLocal("127.0.0.1", (uint)localPort, nodeHost, 22);
                _sshClient.AddForwardedPort(port);
                port.Start();
                if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Forwarded port created for Node {nodeHost}: localhost:{localPort}");

                CancellationToken sftpToken = new CancellationToken();
                var sftp = new SftpClient("127.0.0.1", localPort, nodeUsername, nodePassword);
                await sftp.ConnectAsync(sftpToken);
                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Stable SFTP connection established to Node {nodeHost} via forwarded port");

                //_nodeConnections[nodeHost] = (port, sftp);
                _nodeConnections[nodeHost] = new NodeInfo
                {
                    Host = nodeHost,
                    Username = nodeUsername,
                    Password = nodePassword,
                    LocalPort = localPort,
                    Port = port,
                    Sftp = sftp
                };

                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Initialized SFTP tunnel for {nodeHost}");
                return localPort;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Failed to initialize Node SFTP for {nodeHost}: {ex.Message}");
                return 0;
            }
        }
        private async Task RebuildNodeTunnelsAsync(bool verbose = false)
        {
            foreach (var node in _nodeConnections.Values)
            {
                try
                {
                    if (node.Port != null)
                    {
                        node.Port.Stop();
                        _sshClient.RemoveForwardedPort(node.Port);
                    }

                    var newPort = new ForwardedPortLocal("127.0.0.1", (uint)node.LocalPort, node.Host, 22);
                    _sshClient.AddForwardedPort(newPort);
                    newPort.Start();

                    node.Port = newPort;

                    node.Sftp?.Dispose();
                    CancellationToken sftpToken = new CancellationToken();
                    node.Sftp = new SftpClient("127.0.0.1", node.LocalPort, node.Username, node.Password);
                    await node.Sftp.ConnectAsync(sftpToken);

                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Rebuilt tunnel and SFTP for {node.Host}");
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Failed to rebuild tunnel for {node.Host}: {ex.Message}");
                }
            }
        }
        public async Task<bool> PingNodeAsync(string host, bool verbose = false)
        {
            try
            {
                await CheckConnectionAsync();

                string cmd = $"ping -c 1 -W 2 {host} >/dev/null 2>&1 && echo connected || echo disconnected";
                var result = await ExecuteHubCommandAsync(cmd, verbose);
                result = result.Trim();

                bool isConnected = !(result.Contains("disconnected"));

                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Node '{host}' connection check: {result}");

                return isConnected;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error checking connection for node '{host}': {ex.Message}");
                return false;
            }
        }
        #endregion


        #region Private Helpers
        // SSH Command Methods
        public async Task<string> ExecuteHubCommandAsync(string command, bool verbose = false)
        {
            await CheckConnectionAsync();

            if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"SSH Executing> {command}");

            using (var cmd = _sshClient.CreateCommand(command))
            {
                var result = cmd.Execute();

                if (!string.IsNullOrEmpty(cmd.Error))
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"SSH Error: {cmd.Error}");
                    //throw new FileNotFoundException($"SSH Error: {cmd.Error}");
                }

                if (verbose && !string.IsNullOrWhiteSpace(result))
                    logger.Log(LogLevel.INFO, "Communicator", result);

                return result;
            }
        }
        public async Task<string> ExecuteNodeCommandAsync(string cmd, string host, string username, bool verbose = false)
        {
            if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Preparing to execute command on node {username}@{host}: {cmd}");

            // Escape quotes in command for safety
            string escapedCmd = cmd.Replace("\"", "\\\"");

            // Build SSH command that runs on the Hub to connect to the Node
            string nodeCommand = $"ssh -o BatchMode=yes {username}@{host} \"{escapedCmd}\"";
            if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"{username}: Executing via SSH-> {cmd}");
            return await ExecuteHubCommandAsync(nodeCommand, verbose);
        }
        private static async Task<List<DownloadResult>> DownloadAsync(SftpClient sftp, List<ClusterFileIOCommand> commands, bool verbose = false)
        {
            var results = new List<DownloadResult>();
            foreach (ClusterFileIOCommand command in commands)
            {
                DownloadResult result = new DownloadResult(command);
                results.Add(result);
            }
            foreach (DownloadResult result in results)
            {
                await DownloadAsync(sftp, result, verbose); //maybe remove await?
            }
            return results;
        }
        private static async Task<DownloadResult> DownloadAsync(SftpClient sftp, DownloadResult result, bool verbose = false)
        {
            try
            {
                // Check if the remote file exists
                if (result.Command.checkExists)
                {
                    await CheckExistsAsync(sftp, result, verbose);
                    if (result.FileExists)
                    {
                        result.Exception = new FileNotFoundException($"Remote file not found: {result.Command.RemotePath}");
                        return result;
                    }
                }

                // Get file attributes for size and last write time
                if (result.Command.getAttributes)
                {
                    GetAttributes(sftp, result, verbose);
                }

                // Transfer the file to the local path
                switch (result.Command.Type)
                { 
                    case (ClusterFileIOCommandType.Download):
                        await DownloadFileAsync(sftp, result, verbose);
                        break;
                    case ClusterFileIOCommandType.Upload:
                        await UploadFileAsync(sftp, result, verbose);
                        break;
                    case ClusterFileIOCommandType.Move:
                        await MoveFileAsync(sftp, result, verbose);
                        break;
                }
                result.MainProcedureSucceeded = true;

                // Delete after Download if specified in the command
                if (result.Command.deleteAfter)
                { 
                    if (result.MainProcedureSucceeded)
                    {
                        // Delete the remote file unles it is the nonDeleteFile
                        await DeleteFileAsync(sftp, result, verbose);
                        result.DeleteSucceeded = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        private static async Task CheckExistsAsync(SftpClient sftp, DownloadResult result, bool verbose = false)
        {
            result.FileExists = await sftp.ExistsAsync(result.Command.RemotePath);
            if (verbose)
            {    
                if (result.FileExists)
                    Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Remote file exists: {result.Command.RemotePath}");
                else
                    Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Remote file does not exist: {result.Command.RemotePath}");
            }
        }
        private static void GetAttributes(SftpClient sftp, DownloadResult result, bool verbose = false)
        {
            // Get file attributes for size and last write time
            result.Attributes = sftp.GetAttributes(result.Command.RemotePath);
            if (verbose)
            {
                Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Remote file attributes for {result.Command.RemotePath}: Size={result.Attributes.Size} bytes, LastWriteTime={result.Attributes.LastWriteTime}");
            }
        }
        private static async Task DownloadFileAsync(SftpClient _client, DownloadResult result, bool verbose = false)
        {
            if (!string.IsNullOrEmpty(result.Command.LocalDir))
            {
                Directory.CreateDirectory(result.Command.LocalDir);
            }
            await using FileStream fs = File.Create(result.Command.LocalPath);
            await _client.DownloadAsync(result.Command.RemotePath, fs);
            await fs.FlushAsync();
            result.MainProcedureSucceeded = true; 
            if (verbose) Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Downloaded '{result.Command.RemotePath}' → '{result.Command.LocalPath}' ({result.Attributes?.Size} bytes)");
        }
        private static async Task UploadFileAsync(SftpClient _client, DownloadResult result, bool verbose = false)
        {
            if (!File.Exists(result.Command.LocalPath))
            {
                Logger.Instance.Log(LogLevel.ERROR, "Communicator", $"Local file not found: {result.Command.LocalPath}");
                throw new FileNotFoundException($"Local file not found: {result.Command.LocalPath}");
            }
            await using FileStream fs = File.OpenRead(result.Command.LocalPath);
            await _client.UploadAsync(fs, result.Command.RemotePath);
            await fs.FlushAsync();
            result.MainProcedureSucceeded = true;
            Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Uploaded {result.Command.LocalPath} → {result.Command.RemotePath} ({fs.Length} bytes)");
        }
        private static async Task MoveFileAsync(SftpClient _client, DownloadResult result, bool verbose = false)
        {
            string newRemotePath = Path.Combine(result.Command.RemoteDir, result.Command.LocalFileName).Replace('\\', '/');
            await _client.RenameFileAsync(result.Command.RemotePath, newRemotePath, CancellationToken.None);
            result.MainProcedureSucceeded = true;
            Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Moved remote file: {result.Command.RemotePath} → {newRemotePath}");
        }
        private static async Task DeleteFileAsync(SftpClient _client, DownloadResult result, bool verbose = false)
        {
            CancellationToken deleteToken = new CancellationToken();
            await _client.DeleteFileAsync(result.Command.RemotePath, deleteToken);

            result.DeleteSucceeded = true;

            Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Deleted remote file: {result.Command.RemotePath}");
        }
        #endregion

        #region Public API
        // Hub File Methods
        public async Task<bool> HubFileExists(string hubFilePath, bool verbose = false)
        {
            try
            {
                DownloadResult result = new DownloadResult(new ClusterFileIOCommand(hubFilePath.Split().Last(), "", ClusterFileIOCommandType.Exists, checkExists: true));
                await CheckExistsAsync(_sftpClient, result, verbose);
                return result.FileExists;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error checking existence of {hubFilePath}: {ex.Message}");
                throw;
            }
        }
        public async Task<DateTime> HubFileLastModified(string hubFilePath, bool verbose = false)
        {
            try
            {
                DownloadResult result = new DownloadResult(new ClusterFileIOCommand(hubFilePath.Split().Last(), "", ClusterFileIOCommandType.Attributes, getAttributes: true));
                //await CheckExistsAsync(_sftpClient, result, verbose);
                GetAttributes(_sftpClient, result, verbose);
                return result.Attributes.LastWriteTime;
            }
            catch (FileNotFoundException ex)
            {
                return DateTime.MinValue;
            }
        }
        public async Task<List<LinuxFileInfo>> GetListOfHubFiles(string directory, string fileExtension, bool verbose = false)
        {
            if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Getting list of files from {_host}: {directory}/*.{fileExtension.TrimStart('.')}");
            string command = $"ls -l --full-time \"{directory}\"/*.{fileExtension.TrimStart('.')}";
            string output = await ExecuteHubCommandAsync(command, verbose);

            var files = new List<LinuxFileInfo>();

            // Skip "total xxx"
            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Regex regex = new Regex(
                @"^(?<perm>\S+)\s+" +
                @"(?<links>\d+)\s+" +
                @"(?<owner>\S+)\s+" +
                @"(?<group>\S+)\s+" +
                @"(?<size>\d+)\s+" +
                @"(?<date>\d{4}-\d{2}-\d{2})\s+" +
                @"(?<time>\d{2}:\d{2}:\d{2}\.\d+)\s+" +
                @"(?<offset>[+-]\d{4})\s+" +
                @"(?<name>.+)$");

            foreach (string line in lines)
            {
                if (line.StartsWith("total "))
                    continue;

                Match match = regex.Match(line);
                if (!match.Success)
                    continue;

                string time = match.Groups["time"].Value;
                int dot = time.IndexOf('.');
                if (dot >= 0 && time.Length - dot - 1 > 7)
                {
                    time = time.Substring(0, dot + 8);
                }
                string offset = match.Groups["offset"].Value;
                offset = offset.Insert(offset.Length - 2, ":");
                string timestamp = $"{match.Groups["date"].Value} {time} {offset}";

                files.Add(new LinuxFileInfo
                {
                    Permissions = match.Groups["perm"].Value,
                    HardLinks = int.Parse(match.Groups["links"].Value),
                    Owner = match.Groups["owner"].Value,
                    Group = match.Groups["group"].Value,
                    Size = long.Parse(match.Groups["size"].Value),
                    LastWriteTime = DateTimeOffset.ParseExact(
                        timestamp,
                        "yyyy-MM-dd HH:mm:ss.fffffff zzz",
                        CultureInfo.InvariantCulture),
                    Name = match.Groups["name"].Value
                });
            }

            return files;
        }
        public async Task<bool> DeleteHubFile(string hubFilePath, bool verbose = true)
        {
            try
            {
                DownloadResult result = new DownloadResult(new ClusterFileIOCommand(hubFilePath.Split().Last(), "", ClusterFileIOCommandType.Delete, deleteAfter: true));
                await DeleteFileAsync(_sftpClient, result, verbose);
                return result.DeleteSucceeded;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error deleting {hubFilePath}: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> MoveHubFile(string currentFilePath, string newFilePath, bool verbose = false)
        {
            try
            {
                CancellationToken renameToken = new CancellationToken();
                await _sftpClient.RenameFileAsync(currentFilePath.Replace('\\', '/'), newFilePath.Replace('\\', '/'), renameToken);
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error renaming {currentFilePath} → {newFilePath}: {ex.Message}");
                throw;
            }
        }

        // Node File Methods
        public async Task<bool> NodeFileExists(string nodeFilePath, string host, bool verbose = false)
        {
            if (!_nodeConnections.ContainsKey(host))
                throw new System.Net.Sockets.SocketException();

            try
            {
                DownloadResult result = new DownloadResult(new ClusterFileIOCommand(nodeFilePath.Split().Last(), "", ClusterFileIOCommandType.Exists, checkExists: true));
                await CheckExistsAsync(_nodeConnections[host].Sftp, result, verbose);
                return result.FileExists;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error checking existence of {nodeFilePath}: {ex.Message}");
                throw;
            }
        }
        public async Task<DateTime?> NodeFileLastModified(string nodeFilePath, string host, bool verbose = false)
        {
            if (!_nodeConnections.ContainsKey(host))
                throw new System.Net.Sockets.SocketException();

            try
            {
                DownloadResult result = new DownloadResult(new ClusterFileIOCommand(nodeFilePath.Split().Last(), "", ClusterFileIOCommandType.Attributes, getAttributes: true));
                //await CheckExistsAsync(_nodeConnections[host].Sftp, result, verbose);
                GetAttributes(_nodeConnections[host].Sftp, result, verbose);
                if (verbose)
                {
                    logger.Log(LogLevel.INFO, "Communicator", $"File {nodeFilePath} on {host} last written to: {result.Attributes?.LastWriteTime}");
                }
                return result.Attributes?.LastWriteTime;
            }
            catch (FileNotFoundException ex)
            {
                return DateTime.MinValue;
            }
        }
        public async Task<List<LinuxFileInfo>> GetListOfNodeFiles(string directory, string fileExtension, string host, string username, bool verbose = false)
        {
            if (!_nodeConnections.ContainsKey(host))
                throw new System.Net.Sockets.SocketException();

            if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Getting list of files from node {username}@{host}: {directory}/*.{fileExtension.TrimStart('.')}");
            string command = $"ls -l --full-time \"{directory}\"/*.{fileExtension.TrimStart('.')}";
            string output = await ExecuteNodeCommandAsync(command, host, username, verbose);

            var files = new List<LinuxFileInfo>();

            // Skip "total xxx"
            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Regex regex = new Regex(
                @"^(?<perm>\S+)\s+" +
                @"(?<links>\d+)\s+" +
                @"(?<owner>\S+)\s+" +
                @"(?<group>\S+)\s+" +
                @"(?<size>\d+)\s+" +
                @"(?<date>\d{4}-\d{2}-\d{2})\s+" +
                @"(?<time>\d{2}:\d{2}:\d{2}\.\d+)\s+" +
                @"(?<offset>[+-]\d{4})\s+" +
                @"(?<name>.+)$");

            foreach (string line in lines)
            {
                if (line.StartsWith("total "))
                    continue;

                Match match = regex.Match(line);
                if (!match.Success)
                    continue;

                string time = match.Groups["time"].Value;
                int dot = time.IndexOf('.');
                if (dot >= 0 && time.Length - dot - 1 > 7)
                {
                    time = time.Substring(0, dot + 8);
                }
                string offset = match.Groups["offset"].Value;
                offset = offset.Insert(offset.Length - 2, ":");
                string timestamp = $"{match.Groups["date"].Value} {time} {offset}";

                files.Add(new LinuxFileInfo
                {
                    Permissions = match.Groups["perm"].Value,
                    HardLinks = int.Parse(match.Groups["links"].Value),
                    Owner = match.Groups["owner"].Value,
                    Group = match.Groups["group"].Value,
                    Size = long.Parse(match.Groups["size"].Value),
                    LastWriteTime = DateTimeOffset.ParseExact(
                        timestamp,
                        "yyyy-MM-dd HH:mm:ss.fffffff zzz",
                        CultureInfo.InvariantCulture),
                    Name = match.Groups["name"].Value
                });
            }

            return files;
        }
        public async Task<bool> DeleteNodeFile(string nodeFilePath, string host, bool verbose = false)
        {
            if (!_nodeConnections.ContainsKey(host))
                throw new System.Net.Sockets.SocketException();

            try
            {
                DownloadResult result = new DownloadResult(new ClusterFileIOCommand(nodeFilePath.Split().Last(), "", ClusterFileIOCommandType.Delete, deleteAfter: true));
                await DeleteFileAsync(_nodeConnections[host].Sftp, result, verbose);
                return result.DeleteSucceeded;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error deleting {nodeFilePath}: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> MoveNodeFile(string currentFilePath, string newFilePath, string host, string username, bool verbose = false)
        {
            if (!_nodeConnections.ContainsKey(host))
                throw new System.Net.Sockets.SocketException();

            try
            {
                CancellationToken renameToken = new CancellationToken();
                await _nodeConnections[host].Sftp.RenameFileAsync(currentFilePath.Replace('\\', '/'), newFilePath.Replace('\\', '/'), renameToken);
                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully renamed {host} file: {currentFilePath} → {newFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error renaming {host} file: {currentFilePath} → {newFilePath}: {ex.Message}");
                throw;
            }
        }

        // Asynchronous SSH File Methods
        public async Task<bool> CopyHubToNode(string hubFilePath, string nodeFilePath, string host, string username, bool verbose = false)
        {
            string cmd = $"scp \"{hubFilePath}\" {username}@{host}:\"{nodeFilePath}\"";
            await ExecuteHubCommandAsync(cmd, verbose);
            if (await NodeFileExists(nodeFilePath, host, verbose))//, username, verbose))
            {
                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully copied {hubFilePath} to {username}@{host}:{nodeFilePath}");
                return true;
            }
            else
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Copy operation may have failed, remote file not found: {username}@{host}:{nodeFilePath}");
                return false;
            }
        }
        public async Task<bool> CopyNodeToHub(string nodeFilePath, string hubFilePath, string host, string username, bool verbose = false)
        {
            string cmd = $"scp {username}@{host}:\"{nodeFilePath}\" \"{hubFilePath}\"";
            await ExecuteHubCommandAsync(cmd, verbose);
            if (await HubFileExists(hubFilePath, verbose))
            {
                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully copied {username}@{host}:{nodeFilePath} to {hubFilePath}");
                return true;
            }
            else
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Copy operation may have failed, remote file not found: {hubFilePath}");
                return false;
            }
        }

        // Asynchronous SFTP File Transfer Methods
        public async Task<DownloadResult> PCtoHubAsync(string hubFilePath, string localDirectory, ClusterFileIOCommand command, bool verbose = false)
        {
            await CheckConnectionAsync();

            ClusterFileIOCommand newCommand = new ClusterFileIOCommand(hubFilePath.Split('/', '\\').Last(), Path.GetDirectoryName(hubFilePath)!.Replace('\\', '/'), localDirectory, command);

            return await PCtoHubAsync(newCommand, verbose);
        }
        public async Task<DownloadResult> PCtoHubAsync(ClusterFileIOCommand command, bool verbose = false)
        {
            await CheckConnectionAsync();

            DownloadResult result = new DownloadResult(command);

            return await DownloadAsync(_sftpClient, result, verbose);
        }
        public async Task<List<DownloadResult>> PCtoHubAsync(List<string> hubFilePaths, string localDirectory, ClusterFileIOCommand command, bool verbose = false)
        {
            await CheckConnectionAsync();
            List<ClusterFileIOCommand> newCommands = new List<ClusterFileIOCommand>();

            foreach (string hubFilePath in hubFilePaths)
            {
                ClusterFileIOCommand newCommand = new ClusterFileIOCommand(hubFilePath.Split('/', '\\').Last(), Path.GetDirectoryName(hubFilePath)!.Replace('\\', '/'), localDirectory, command);
                newCommands.Add(newCommand);
            }

            return await PCtoHubAsync(newCommands, verbose);
        }
        public async Task<List<DownloadResult>> PCtoHubAsync(List<ClusterFileIOCommand> commands, bool verbose = false)
        {
            await CheckConnectionAsync();

            return await DownloadAsync(_sftpClient, commands, verbose);
        }

        public async Task<DownloadResult> PCtoNodeAsync(string hubFilePath, string localDirectory, ClusterFileIOCommand command, string host, bool verbose = false)
        {
            await CheckConnectionAsync();

            ClusterFileIOCommand newCommand = new ClusterFileIOCommand(hubFilePath.Split('/', '\\').Last(), Path.GetDirectoryName(hubFilePath)!.Replace('\\', '/'), localDirectory, command);

            return await PCtoNodeAsync(newCommand, host, verbose);
        }
        public async Task<DownloadResult> PCtoNodeAsync(ClusterFileIOCommand command, string host, bool verbose = false)
        {
            await CheckConnectionAsync();

            DownloadResult result = new DownloadResult(command);

            return await DownloadAsync(_nodeConnections[host].Sftp, result);
        }
        public async Task<List<DownloadResult>> PCtoNodeAsync(List<string> nodeFilePaths, string localDirectory, ClusterFileIOCommand command, string host, bool verbose = false)
        {
            await CheckConnectionAsync();
            List<ClusterFileIOCommand> newCommands = new List<ClusterFileIOCommand>();

            foreach (string nodeFilePath in nodeFilePaths)
            {
                ClusterFileIOCommand newCommand = new ClusterFileIOCommand(nodeFilePath.Split('/', '\\').Last(), Path.GetDirectoryName(nodeFilePath)!.Replace('\\', '/'), localDirectory, command);
                newCommands.Add(newCommand);
            }

            return await PCtoNodeAsync(newCommands, host, verbose);
        }
        public async Task<List<DownloadResult>> PCtoNodeAsync(List<ClusterFileIOCommand> commands, string host, bool verbose = false)
        {
            await CheckConnectionAsync();

            return await DownloadAsync(_nodeConnections[host].Sftp, commands, verbose);
        }
        #endregion
    }
}