using mLogger;
using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Sftp;
using System.IO.Enumeration;

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
    public class Communicator : IDisposable
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

        private readonly object _lock = new object();

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
            Disconnect();
        }

        //Connection Methods
        public bool Connect()
        {
            lock (_lock)
            {
                if (IsConnected) return true;

                // Initialize SSH client and node tunnels
                _sshClient = new SshClient(_host, _username, _password);

                logger.Log(LogLevel.INFO, "Communicator", $"Connecting to Cluster Hub at {_username}@{_host}. Please wait...");
                _sshClient.Connect();
                _sftpClient = new SftpClient(_host, _username, _password);
                _sftpClient.Connect();
                RebuildNodeTunnels();
                logger.Log(LogLevel.INFO, "Communicator", $"Connected to {_host}");

                return true;
            }
        }
        public void Disconnect()
        {
            lock (_lock)
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
        }
        public SSHCheckResult checkSSHDevice(bool verbose)
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
                bool connected = Connect();
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
        public int AddNodeTunnel(string nodeHost, string nodeUsername, string nodePassword, bool verbose = false)
        {
            if (nodeHost == null || nodeHost == "")
                throw new ArgumentNullException("nodeHost passed as null");
            if (nodeUsername == null || nodeUsername == "")
                throw new ArgumentNullException("nodeUsername passed as null");

            if (!IsConnected)
            {
                Connect();
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

                var sftp = new SftpClient("127.0.0.1", localPort, nodeUsername, nodePassword);
                sftp.Connect();
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
        private void RebuildNodeTunnels(bool verbose = false)
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
                    node.Sftp = new SftpClient("127.0.0.1", node.LocalPort, node.Username, node.Password);
                    node.Sftp.Connect();

                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Rebuilt tunnel and SFTP for {node.Host}");
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Failed to rebuild tunnel for {node.Host}: {ex.Message}");
                }
            }
        }
        /*private async Task<DownloadResult> DownloadAsync(string remoteFile, string localFile, string host)
        {
            var result = new DownloadResult()
            {
                //RemotePath = remoteFile,
                //LocalPath = localFile
                LocalFileName = localFile.Split('\\').Last(),
                LocalDir = Path.GetDirectoryName(localFile)!,
                RemoteFileName = remoteFile,
                RemoteDir = Path.GetDirectoryName(remoteFile)!
            };

            try
            {
                NodeInfo node;

                // Determine if hub or node transaction based on host prefix
                if ("hub" ==  host.ToLower().Substring(0, 3))
                {
                    node = new NodeInfo { Host = host,
                                          LocalPort = 22,
                                          Username = _username,
                                          Password = _password };
                }
                else
                {
                    node = _nodeConnections[host];
                }

                //Create and connect SFTP client
                using var _client = new SftpClient(node.ConnectionHost, node.LocalPort, node.Username, node.Password);
                _client.Connect();


                // Check if the remote file exists
                if (!_client.Exists(remoteFile))
                {
                    result.FileExists = false;
                    return result;
                }

                result.FileExists = true;

                // Get file attributes for size and last write time
                SftpFileAttributes info = _client.GetAttributes(remoteFile);

                result.FileSize = info.Size;
                result.LastWriteTime = info.LastWriteTime;

                // Transfer the file to the local path
                string? directory = Path.GetDirectoryName(localFile);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await using FileStream fs = File.Create(localFile);

                await _client.DownloadAsync(remoteFile, fs);

                await fs.FlushAsync();

                result.DownloadSucceeded = true;

                // Delete the remote file
                _client.DeleteFile(remoteFile);

                result.DeleteSucceeded = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }*/


        private async Task<List<DownloadResult>> DownloadBatchAsync(SftpClient sftp, List<string> remoteFiles, string localFileDirectory, string nonDeleteFileName = "", bool verbose = false)
        {
            var results = new List<DownloadResult>();

            foreach (string remoteFile in remoteFiles)
            {
                string fileName = Path.GetFileName(remoteFile);
                DownloadResult result = new DownloadResult(fileName, Path.GetDirectoryName(remoteFile)!, localFileDirectory);
                results.Add(result);
            }            

            foreach (DownloadResult result in results)
            {                
                await DownloadFileAsync(sftp, result, nonDeleteFileName, verbose);
            }

            return results;
        }
        private static async Task<DownloadResult> DownloadFileAsync(SftpClient sftp, DownloadResult result, string nonDeleteFileName = "", bool verbose = false)
        {
            try
            {
                // Check if the remote file exists
                await CheckExists(sftp, result, verbose);
                if (result.FileExists)
                {
                    // Get file attributes for size and last write time
                    GetAttributes(sftp, result, verbose);

                    // Transfer the file to the local path
                    await DownloadFile(sftp, result, verbose);
                    if (result.DownloadSucceeded && result.RemoteFileName != nonDeleteFileName)
                    {
                        // Delete the remote file unles it is the nonDeleteFile
                        DeleteFile(sftp, result, verbose);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        private static async Task CheckExists(SftpClient sftp, DownloadResult result, bool verbose = false)
        {
            result.FileExists = await sftp.ExistsAsync(result.RemotePath);
            if (verbose)
            {    
                if (result.FileExists)
                    Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Remote file exists: {result.RemotePath}");
                else
                    Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Remote file does not exist: {result.RemotePath}");
            }
        }
        private static void GetAttributes(SftpClient sftp, DownloadResult result, bool verbose = false)
        {
            if (!result.FileExists)
            {
                result.FileSize = 0;
                result.LastWriteTime = DateTime.MinValue;
            }

            // Get file attributes for size and last write time
            SftpFileAttributes info = sftp.GetAttributes(result.RemotePath);

            result.FileSize = info.Size;
            result.LastWriteTime = info.LastWriteTime;
            if (verbose)
            {
                Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Remote file attributes for {result.RemotePath}: Size={result.FileSize} bytes, LastWriteTime={result.LastWriteTime}");
            }
        }
        private static async Task DownloadFile(SftpClient _client, DownloadResult result, bool verbose = false)
        {
            if (!string.IsNullOrEmpty(result.LocalDir))
            {
                Directory.CreateDirectory(result.LocalDir);
            }

            await using FileStream fs = File.Create(result.LocalPath);

            await _client.DownloadAsync(result.RemotePath, fs);

            await fs.FlushAsync();

            result.DownloadSucceeded = true;

            Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Downloaded {result.RemotePath} → {result.LocalPath} ({result.FileSize} bytes)");
        }
        private static void DeleteFile(SftpClient _client, DownloadResult result, bool verbose = false)
        {
            _client.DeleteFile(result.RemotePath);

            result.DeleteSucceeded = true;

            Logger.Instance.Log(LogLevel.INFO, "Communicator", $"Deleted remote file: {result.RemotePath}");
        }

        //Hub File Methods
        public bool HubFileExists(string hubFilePath, bool verbose = false)
        {
            try
            {
                string result = ExecuteHubCommand($"[ -f \"{hubFilePath}\" ] && echo \"exists\" || echo \"missing\"", verbose);
                if (result.Contains("exists"))
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Hub file exists: {hubFilePath}");
                }
                else
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Hub file missing: {hubFilePath}");
                }
                return result.Contains("exists");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error checking existence of {hubFilePath}: {ex.Message}");
                throw;
            }
        }
        public DateTime HubFileLastModified(string pathVariable)
        {
            if (string.IsNullOrWhiteSpace(pathVariable))
                throw new ArgumentException("Path cannot be null or empty.", nameof(pathVariable));

            // Escape the path in case it contains spaces or special characters
            string escapedPath = pathVariable.Replace("\"", "\\\"");
            string command = $"stat -c %Y \"{escapedPath}\""; // %Y = epoch time (seconds since 1970-01-01)

            try
            {
                string result = ExecuteHubCommand(command);

                if (!long.TryParse(result.Trim(), out long epochSeconds))
                    throw new FormatException($"Unexpected response from stat command: '{result}'");

                // Convert from Unix epoch seconds to local DateTime
                return DateTimeOffset.FromUnixTimeSeconds(epochSeconds).LocalDateTime;
            }
            catch (FileNotFoundException ex)
            {
                return DateTime.MinValue;
            }
        }
        public string[] GetListOfHubFiles(string directory, string fileExtension)
        {
            // Normailze path components
            directory = directory.Replace("\\", "/");
            if (directory.EndsWith("/"))
            {
                directory = directory.TrimEnd('/');
            }
            if (!fileExtension.StartsWith('.'))
                fileExtension = '.' + fileExtension;

            // Request list of files in directory
            string cmd = $"ls -1 \"{directory}\"/*{fileExtension} 2>/dev/null";
            string result = ExecuteHubCommand(cmd);

            return result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool DeleteHubFile(string hubFilePath, bool verbose = true)
        {
            try
            {
                // Check if file exists before deletion
                if (!HubFileExists(hubFilePath, false))
                {
                    if (verbose) logger.Log(LogLevel.WARN, "Communicator", $"File not found before deletion: {hubFilePath}");
                    return false;
                }

                // Attempt deletion
                ExecuteHubCommand($"rm -f \"{hubFilePath}\"", false);

                // Verify deletion
                if (!HubFileExists(hubFilePath, false))
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully deleted {hubFilePath}");
                    return true;
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Failed to delete {hubFilePath}");
                    return false;
                }

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error deleting {hubFilePath}: {ex.Message}");
                throw;
            }
        }
        public bool MoveHubFile(string currentFilePath, string newFilePath, bool verbose = false)
        {
            try
            {
                // Check if source file exists
                if (!HubFileExists(currentFilePath, verbose))
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Source file not found: {currentFilePath}");
                    return false;
                }

                // Check if target already exists
                if (HubFileExists(newFilePath, verbose))
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Target file already exists: {newFilePath}");
                    return false;
                }

                // Attempt rename
                ExecuteHubCommand($"mv \"{currentFilePath}\" \"{newFilePath}\"", verbose);

                // Verify rename
                if (!HubFileExists(currentFilePath, verbose) || HubFileExists(newFilePath, verbose))
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully renamed {currentFilePath} → {newFilePath}");
                    return true;
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Failed to rename {currentFilePath} → {newFilePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error renaming {currentFilePath} → {newFilePath}: {ex.Message}");
                throw;
            }
        }


        //Node File Methods
        public bool NodeFileExists(string nodeFilePath, string host, bool verbose = false)
        {
            if (!_nodeConnections.TryGetValue(host, out var node))
                throw new InvalidOperationException($"Node {host} not initialized.");

            try
            {
                if (node.Sftp == null || !node.Sftp.IsConnected)
                {
                    node.Sftp.Connect();
                    if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Reconnected SFTP to node {host}");
                }

                bool exists = node.Sftp.Exists(nodeFilePath);
                if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Checked existence of {host}:{nodeFilePath} → {(exists ? "exists" : "does not exist")}");
                return exists;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Failed to check file existence on {host}:{nodeFilePath}: {ex.Message}");
                return false;
            }
        }
        public DateTime? NodeFileLastModified(string nodeFilePath, string host, bool verbose = false)
        {
            if (!_nodeConnections.TryGetValue(host, out var node))
                throw new InvalidOperationException($"Node {host} not initialized.");

            try
            {
                if (node.Sftp == null || !node.Sftp.IsConnected)
                {
                    node.Sftp.Connect();
                    if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Reconnected SFTP to node {host}");
                }

                var attrs = node.Sftp.GetAttributes(nodeFilePath);
                DateTime lastModified = attrs.LastWriteTime;

                if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Last modified {host}:{nodeFilePath} → {lastModified:yyyy-MM-dd HH:mm:ss}");
                return lastModified;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Failed to get last modified time for {host}:{nodeFilePath}: {ex.Message}");
                return null;
            }
        }
        public string[] GetListOfNodeFiles(string directory, string fileExtension, string host, string username)
        {
            // Normailze path components
            directory = directory.Replace("\\", "/");
            if (directory.EndsWith("/"))
            {
                directory = directory.TrimEnd('/');
            }
            if (!fileExtension.StartsWith('.'))
                fileExtension = '.' + fileExtension;

            // Ask remote system for a list of .log files
            string cmd = $"ls -1 \"{directory}\"/*{fileExtension} 2>/dev/null";
            string result = ExecuteNodeCommand(cmd, host, username);

            return result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool DeleteNodeFile(string nodeFilePath, string host, bool verbose = false)
        {
            if (!_nodeConnections.TryGetValue(host, out var node))
                throw new InvalidOperationException($"Node {host} not initialized.");

            try
            {
                if (node.Sftp == null || !node.Sftp.IsConnected)
                {
                    node.Sftp.Connect();
                    if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Reconnected SFTP to node {host}");
                }

                if (node.Sftp.Exists(nodeFilePath))
                {
                    node.Sftp.DeleteFile(nodeFilePath);
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Deleted {host}:{nodeFilePath}");
                    return true;
                }
                else
                {
                    if (verbose) logger.Log(LogLevel.WARN, "Communicator", $"File not found on {host}:{nodeFilePath} (nothing to delete)");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Failed to delete file on {host}:{nodeFilePath}: {ex.Message}");
                return false;
            }
        }
        public bool MoveNodeFile(string currentFilePath, string newFilePath, string host, string username, bool verbose = false)
        {
                if (!_nodeConnections.TryGetValue(host, out var node))
                    throw new InvalidOperationException($"Node {host} not initialized.");

            try
                {
                if (node.Sftp == null || !node.Sftp.IsConnected)
                {
                    node.Sftp.Connect();
                    if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Reconnected SFTP to node {host}");
                }

                if (!node.Sftp.Exists(currentFilePath))
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Move failed: source file not found on {host}:{currentFilePath}");
                    return false;
                }

                node.Sftp.RenameFile(currentFilePath, newFilePath);

                if (node.Sftp.Exists(newFilePath))
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Moved {host}:{currentFilePath} → {newFilePath}");
                    return true;
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Move may have failed: destination not found {host}:{newFilePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Failed to move file on {host}:{currentFilePath} → {newFilePath}: {ex.Message}");
                return false;
            }
        }


        //Command Methods
        public bool PingNode(string host, bool verbose = false)
        {
            try
            {
                if (!IsConnected)
                {
                    Connect();
                    if (!IsConnected)
                    {
                        logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before checking node connection.");
                        return false;
                    }
                }

                string cmd = $"ping -c 1 -W 2 {host} >/dev/null 2>&1 && echo connected || echo disconnected";
                var result = ExecuteHubCommand(cmd, verbose).Trim();

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
        public string ExecuteHubCommand(string command, bool verbose = false)
        {
            if (!IsConnected)
            {
                Connect();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before executing command.");
                    throw new InvalidOperationException("Not connected.");
                }
            }

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
        public string ExecuteNodeCommand(string cmd, string host, string username, bool verbose = false)
        {
            if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Preparing to execute command on node {username}@{host}: {cmd}");

            // Escape quotes in command for safety
            string escapedCmd = cmd.Replace("\"", "\\\"");

            // Build SSH command that runs on the Hub to connect to the Node
            string nodeCommand = $"ssh -o BatchMode=yes {username}@{host} \"{escapedCmd}\"";
            if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"{username}: Executing via SSH-> {cmd}");
            return ExecuteHubCommand(nodeCommand, verbose);
        }

        //File Transfer Methods
        public bool CopyHubToNode(string hubFilePath, string nodeFilePath, string host, string username, bool verbose = false)
        {
            string cmd = $"scp \"{hubFilePath}\" {username}@{host}:\"{nodeFilePath}\"";
            ExecuteHubCommand(cmd, verbose);
            if (NodeFileExists(nodeFilePath, host, verbose))//, username, verbose))
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
        public bool CopyPCtoHub(string PCFilePath, string HubFilePath, bool verbose = false)
        {
            if (!IsConnected)
            {
                Connect();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before executing node command.");
                    throw new InvalidOperationException("Not connected to hub.");
                }
            }

            if (!File.Exists(PCFilePath))
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Local file not found: {PCFilePath}");
                return false;
            }

            try
            {
                using (var fs = new FileStream(PCFilePath, FileMode.Open))
                {
                    using (var scp = new ScpClient(_host, _username, _password))
                    {
                        scp.Connect();
                        if (scp.IsConnected)
                        {
                            scp.Upload(fs, HubFilePath);
                        }
                        else
                        {
                            logger.Log(LogLevel.ERROR, "Communicator", "SCP client failed to connect.");
                            throw new InvalidOperationException("SCP client not connected.");
                        }
                    }
                }

                // Verify file presence on remote side
                if (HubFileExists(HubFilePath, verbose))
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully copied {PCFilePath} → {HubFilePath}");
                    return true;
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Copy operation may have failed, remote file not found: {HubFilePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error copying {PCFilePath} → {HubFilePath}: {ex.Message}");
                throw;
            }
        }
        public bool CopyHubToPC(string HubFilePath, string PCFilePath, bool verbose = false)
        {
            if (!IsConnected)
            {
                Connect();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before executing node command.");
                    throw new InvalidOperationException("Not connected to hub.");
                }
            }

            try
            {
                // Check remote file existence
                if (!HubFileExists(HubFilePath, verbose))
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Remote file not found: {HubFilePath}");
                    return false;
                }

                // Copy file from remote
                using (var fs = new FileStream(PCFilePath, FileMode.Create))
                {
                    using (var scp = new ScpClient(_host, _username, _password))
                    {
                        scp.Connect();
                        if (scp.IsConnected)
                        {
                            scp.Download(HubFilePath, fs);
                        }
                        else
                        {
                            logger.Log(LogLevel.ERROR, "Communicator", "SCP client failed to connect.");
                            throw new InvalidOperationException("SCP client not connected.");
                        }
                        scp.Disconnect();
                    }
                }

                // Verify local file exists and has content
                var localFileInfo = new FileInfo(PCFilePath);
                if (localFileInfo.Exists)
                {
                    if (localFileInfo.Length > 0)
                    {
                        if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully copied {HubFilePath} → {PCFilePath} ({localFileInfo.Length} bytes)");
                        return true;
                    }
                    else
                    {
                        logger.Log(LogLevel.ERROR, "Communicator", $"Copied file is empty: {PCFilePath}");
                        return false;
                    }
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Copy failed: {HubFilePath} → {PCFilePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error copying {HubFilePath} → {PCFilePath}: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> CopyHubToPCAsync(string HubFilePath, string PCFilePath, bool verbose = false)
        {
            if (!IsConnected)
            {
                Connect();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before executing node command.");
                    throw new InvalidOperationException("Not connected to hub.");
                }
            }
            DownloadResult result = new DownloadResult(HubFilePath, PCFilePath);
            try
            {
                await DownloadFileAsync(_sftpClient, result, result.RemoteFileName, verbose);

                return result.DownloadSucceeded;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Error copying {HubFilePath} → {PCFilePath}: {ex.Message}");
                throw;
            }
        }
        public async Task<List<DownloadResult>> CopyBatchHubToPCAsync(List<string> hubFilePaths, string localDirectory, string nonDeleteFileName = "", bool verbose = false)
        {
            NodeInfo node = new NodeInfo { Host = _host,
                                           LocalPort = 22,
                                           Username = _username,
                                           Password = _password };

            //Create and connect SFTP client
            //using var sftp = new SftpClient(node.ConnectionHost, node.LocalPort, node.Username, node.Password);
            //CancellationToken token = new CancellationToken();
            //await sftp.ConnectAsync(token);

            return await DownloadBatchAsync(_sftpClient, hubFilePaths, localDirectory, nonDeleteFileName);
        }
        public bool CopyNodeToPC(string nodeFilePath, string PCfilePath, string nodeName, bool verbose = false)
        {
            if (!_nodeConnections.TryGetValue(nodeName, out var node))
                throw new InvalidOperationException($"Node {nodeName} not initialized.");

            if (node.Sftp == null || !node.Sftp.IsConnected)
            {
                node.Sftp.Connect();
                if (verbose) logger.Log(LogLevel.DEBUG, "Communicator", $"Reconnected SFTP to node {nodeName}");
            }

            try
            {
                using (var fs = File.Create(PCfilePath))
                {
                    node.Sftp.DownloadFile(nodeFilePath, fs);
                }
                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Downloaded {nodeName}:{nodeFilePath} → {PCfilePath}");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Failed to download file from {nodeName}:{nodeFilePath} → {PCfilePath}: {ex.Message}");
                throw;
            }

            var localFileInfo = new FileInfo(PCfilePath);
            if (localFileInfo.Exists)
            {
                if (localFileInfo.Length > 0)
                {
                    if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully downloaded {nodeName}:{nodeFilePath} → {PCfilePath}");
                    return true;
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator", $"Download resulted in empty file: {PCfilePath}");
                    return false;
                }
            }
            else
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Download failed, local file not found: {PCfilePath}");
                return false;
            }
            //if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Downloaded {nodeName}:{nodeFilePath} → {PCfilePath}");
        }
        public async Task<DownloadResult> CopyNodeToPCAsync(string nodeFilePath, string PCfilePath, string nodeName, bool verbose = false)
        {
            DownloadResult result = new DownloadResult(nodeFilePath, PCfilePath);
            NodeInfo node = _nodeConnections[nodeName];
            //Create and connect SFTP client
            //using var sftp = new SftpClient(node.ConnectionHost, node.LocalPort, node.Username, node.Password);
            //CancellationToken token = new CancellationToken();
            //await sftp.ConnectAsync(token);

            return await DownloadFileAsync(node.Sftp, result);
        }
        public async Task<List<DownloadResult>> CopyBatchNodeToPCAsync(List<string> nodeFilePaths, string localDirectory, string host, string nonDeleteFileName = "", bool verbose = false)
        {
            NodeInfo node = _nodeConnections[host];
            //Create and connect SFTP client
            //using var sftp = new SftpClient(node.ConnectionHost, node.LocalPort, node.Username, node.Password);
            //CancellationToken token = new CancellationToken();
            //await sftp.ConnectAsync(token);

            return await DownloadBatchAsync(node.Sftp, nodeFilePaths, localDirectory, nonDeleteFileName, verbose);
        }
        public bool CopyPCtoNode(string PCfilePath, string nodeFilePath, string host, bool verbose = false)
        {
            if (!IsConnected)
            {
                Connect();
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator", "Failed to connect before executing command.");
                    throw new InvalidOperationException("Not connected.");
                }
            }

            if (!_nodeConnections.TryGetValue(host, out var node))
                throw new InvalidOperationException($"Node {host} not initialized.");

            using (var fs = File.OpenRead(PCfilePath))
            {
                node.Sftp.UploadFile(fs, nodeFilePath, true);
            }
            if (NodeFileExists(nodeFilePath, host, verbose))//, node.Username, verbose))
            {
                if (verbose) logger.Log(LogLevel.INFO, "Communicator", $"Successfully uploaded {PCfilePath} → {host}:{nodeFilePath}");
                return true;
            }
            else
            {
                logger.Log(LogLevel.ERROR, "Communicator", $"Upload may have failed, remote file not found: {host}:{nodeFilePath}");
                return false;
            }
        }


        //testMethod
        public void testFileMethods(string PCfilePath) //, string host, string username)
        {
            logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "Starting Self-Test");
            try
            {
                if (!IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", "Cluster Not conntected");
                    return;
                }
                if (!_nodeConnections.First().Value.Sftp.IsConnected)
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", $"{_nodeConnections.First().Value.Username} sftp session not connected");
                    return;
                }

                string host = _nodeConnections.First().Value.Host;
                string username = _nodeConnections.First().Value.Username;

                string hubFilePath = $"/tmp/{Path.GetFileName(PCfilePath)}";
                string nodeHubFilePath = $"/tmp/fromHub{Path.GetFileName(PCfilePath)}";
                string pcHubCopyBackPath = PCfilePath.Substring(0, PCfilePath.Length - 4) + "_HubCopyback.txt";

                string nodePCFilePath = $"/tmp/fromPC{Path.GetFileName(PCfilePath)}";
                string pcNodeCopyBackPath = PCfilePath.Substring(0, PCfilePath.Length - 4) + "_Nodecopyback.txt";

                if (HubFileExists(hubFilePath))
                {
                    logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", $"Deleting hub file {hubFilePath} before test.");
                    DeleteHubFile(hubFilePath);
                }
                if (NodeFileExists(nodeHubFilePath, host))
                {
                    logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", $"Deleting node file {nodeHubFilePath} before test.");
                    DeleteNodeFile(nodeHubFilePath, host);
                }
                if (NodeFileExists(nodePCFilePath, host))
                {
                    logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", $"Deleting node file {nodePCFilePath} before test.");
                    DeleteNodeFile(nodePCFilePath, host);
                }
                if (File.Exists(pcHubCopyBackPath))
                {
                    logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", $"Deleting PC file {pcHubCopyBackPath} before test.");
                    File.Delete(pcHubCopyBackPath);
                }
                if (File.Exists(pcNodeCopyBackPath))
                {
                    logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", $"Deleting PC file {pcNodeCopyBackPath} before test.");
                    File.Delete(pcNodeCopyBackPath);
                }
                // Copy PC to Hub
                logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "Starting test:");
                logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "   Copy PC to Hub");
                CopyPCtoHub(PCfilePath, hubFilePath);
                if (HubFileExists(hubFilePath))
                {
                    logger.Log(LogLevel.INFO, "Communicator_selfTestMethod", "       Success");
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", "       Fail");
                    return;
                }

                // Copy Hub to Node
                logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "   Copy Hub to Node");
                CopyHubToNode(hubFilePath, nodeHubFilePath, host, username);
                if (NodeFileExists(nodeHubFilePath, host))
                {
                    logger.Log(LogLevel.INFO, "Communicator_selfTestMethod", "       Success");
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", "       Fail");
                    return;
                }
                // Copy Hub to PC
                logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "   Copy Hub to PC");
                CopyHubToPC(hubFilePath, pcHubCopyBackPath);
                if (File.Exists(pcHubCopyBackPath))
                {
                    logger.Log(LogLevel.INFO, "Communicator_selfTestMethod", "       Success");
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", "       Fail");
                    return;
                }

                // Copy PC to Node (to a different temp name)
                logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "   Copy PC to Node");
                CopyPCtoNode(PCfilePath, nodePCFilePath, host);
                if (NodeFileExists(nodePCFilePath, host))//, username))
                {
                    logger.Log(LogLevel.INFO, "Communicator_selfTestMethod", "       Success");
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", "       Fail");
                }

                // Copy Node back to PC (different name)
                logger.Log(LogLevel.DEBUG, "Communicator_selfTestMethod", "   Copy Node back to PC");
                CopyNodeToPC(nodePCFilePath, pcNodeCopyBackPath, host);
                if (File.Exists(pcNodeCopyBackPath))
                {
                    logger.Log(LogLevel.INFO, "Communicator_selfTestMethod", "       Success");
                }
                else
                {
                    logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", "       Fail");
                }

                //DeleteHubFile(hubFilePath);
                //DeleteHubFile(hubCopyBackPath);
                //DeleteNodeFile(nodeFilePath, host, username);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, "Communicator_selfTestMethod", $"Test encountered error: {ex.Message}");
            }
        }
    }
}