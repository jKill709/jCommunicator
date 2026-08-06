using Renci.SshNet.Sftp;

namespace jCommunicator
{
    public class DownloadResult
    {
        public ClusterFileIOCommand Command { get; set; }

        public SftpFileAttributes? Attributes { get; set; }

        public bool FileExists { get; set; }
        public bool MainProcedureSucceeded { get; set; }
        public bool DeleteSucceeded { get; set; }

        public Exception? Exception { get; set; }

        public bool Success => FileExists && MainProcedureSucceeded && DeleteSucceeded && Exception == null;

        public DownloadResult(ClusterFileIOCommand command)
        {
            Command = command;
        }
        public DownloadResult(string remotePath, string localPath, ClusterFileIOCommand command)
        {
            Command = new ClusterFileIOCommand(remotePath, localPath, command);
        }
        public DownloadResult(string fileName, string remoteDir, string localDir, ClusterFileIOCommand command)
        {
            Command = new ClusterFileIOCommand(fileName, remoteDir, localDir, command);
        }

        public override string ToString()
        {
            return
                $"Remote: {Command.RemotePath}\n" +
                $"Local : {Command.LocalPath}\n" +
                $"Exists: {FileExists}\n" +
                $"Size  : {Attributes?.Size.ToString() ?? "N/A"}\n" +
                $"Date  : {Attributes?.LastWriteTime.ToString() ?? "N/A"}\n" +
                $"Downloaded: {MainProcedureSucceeded}\n" +
                $"Deleted   : {DeleteSucceeded}\n" +
                $"Success   : {Success}\n" +
                $"{(Exception != null ? Exception.Message : "")}";
        }
    }

    public struct ClusterFileIOCommand
    {
        public string RemoteDir { get; set; } = "";
        public string RemoteFileName { get; set; } = "";
        public string RemotePath => RemoteDir + '/' + RemoteFileName;

        public string LocalDir { get; set; } = "";
        public string LocalFileName { get; set; } = "";
        public string LocalPath => System.IO.Path.Combine(LocalDir, LocalFileName);

        public ClusterFileIOCommandType Type { get; set;  }

        public bool checkExists { get; set; }
        public bool getAttributes { get; set; }
        public bool deleteAfter { get; set; }
        public bool checkSize { get; set; }

        public ClusterFileIOCommand (string remotePath, string localPath, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool deleteAfter = false, bool checkSize = false)
        {
            SetPaths(remotePath, localPath);

            this.Type = type;
            this.checkExists = checkExists;
            this.getAttributes = getAttributes;
            this.deleteAfter = deleteAfter;
            this.checkSize = checkSize;

            switch (type)
            {
                case ClusterFileIOCommandType.Exists:
                {
                    this.checkExists = true;
                    break;
                }
                case ClusterFileIOCommandType.Attributes:
                {
                    this.getAttributes = true;
                        break;
                }
                case ClusterFileIOCommandType.Delete:
                {
                    this.deleteAfter = true;
                    break;
                }
                case ClusterFileIOCommandType.Download:
                {
                    break;
                }
                case ClusterFileIOCommandType.Upload:
                {
                    break;
                }
                case ClusterFileIOCommandType.Move:
                {
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported command type: {type}");
            }
        }
        public ClusterFileIOCommand(string remoteDir, string localDir, string fileName, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool deleteAfter = false, bool checkSize = false)
        {
            SetPaths(remoteDir, localDir, fileName);

            this.Type = type;
            this.checkExists = checkExists;
            this.getAttributes = getAttributes;
            this.deleteAfter = deleteAfter;
            this.checkSize = checkSize;

            switch (type)
            {
                case ClusterFileIOCommandType.Exists:
                    this.checkExists = true;
                    break;
                case ClusterFileIOCommandType.Attributes:
                    this.getAttributes = true;
                    break;
                case ClusterFileIOCommandType.Delete:
                    this.deleteAfter = true;
                    break;
            }
        }
        public ClusterFileIOCommand(string remotePath, string localPath, ClusterFileIOCommand other)
        {
            SetPaths(remotePath, localPath);

            this.Type = other.Type;
            this.checkExists = other.checkExists;
            this.getAttributes = other.getAttributes;
            this.deleteAfter = other.deleteAfter;
            this.checkSize = other.checkSize;
        }
        public ClusterFileIOCommand(string remoteDir, string localDir, string fileName, ClusterFileIOCommand other)
        {
            SetPaths(remoteDir, localDir, fileName);

            this.Type = other.Type;
            this.checkExists = other.checkExists;
            this.getAttributes = other.getAttributes;
            this.deleteAfter = other.deleteAfter;
            this.checkSize = other.checkSize;
        }

        private void SetPaths(string remotePath, string localPath)
        {
            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath), "Remote path cannot be null.");
            } 
            else if (string.IsNullOrWhiteSpace(remotePath))
            {
                throw new ArgumentOutOfRangeException(nameof(remotePath), "Remote path cannot be empty.");
            }

            RemoteFileName = Path.GetFileName(remotePath);
            RemoteDir = Path.GetDirectoryName(remotePath)!.Replace('\\', '/');
            LocalFileName = Path.GetFileName(localPath);
            LocalDir = Path.GetDirectoryName(localPath) ?? "";

        }
        private void SetPaths(string remoteDir, string localDir, string fileName)
        {
            if (remoteDir == null)
            {
                throw new ArgumentNullException(nameof(remoteDir), "Remote path cannot be null.");
            }
            else if (string.IsNullOrWhiteSpace(remoteDir))
            {
                throw new ArgumentOutOfRangeException(nameof(remoteDir), "Remote path cannot be empty.");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName), "Remote path cannot be null.");
            }
            else if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentOutOfRangeException(nameof(fileName), "Remote path cannot be empty.");
            }

            RemoteFileName = fileName;
            RemoteDir = remoteDir.Replace('\\', '/').TrimEnd('/');
            LocalFileName = fileName;
            LocalDir = localDir.TrimEnd('\\');
        }
    }

    public enum ClusterFileIOCommandType
    {
        Exists,
        Attributes,
        Download,
        Upload,
        Move,
        Delete
    }
}
