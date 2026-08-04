using Renci.SshNet.Sftp;

namespace jCommunicator
{
    public class DownloadResult
    {
        public ClusterFileIOCommand Command { get; set; }

        public SftpFileAttributes Attributes { get; set; }

        public bool FileExists { get; set; }
        public bool MainProcedureSucceeded { get; set; }
        public bool DeleteSucceeded { get; set; }

        public Exception? Exception { get; set; }

        public bool Success => FileExists && MainProcedureSucceeded && DeleteSucceeded && Exception == null;

        public DownloadResult(ClusterFileIOCommand command)
        {
            Command = command;
        }

        public DownloadResult(string fileName, string remoteDir, string localDir, ClusterFileIOCommand command)
        {
            Command = command;
        }

        public override string ToString()
        {
            return
                $"Remote: {Command.RemotePath}\n" +
                $"Local : {Command.LocalPath}\n" +
                $"Exists: {FileExists}\n" +
                $"Size  : {Attributes.Size}\n" +
                $"Date  : {Attributes.LastWriteTime}\n" +
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

        public bool checkExists { get; }
        public bool getAttributes { get; }
        public bool deleteAfter { get; }
        public bool checkSize { get; }

        public ClusterFileIOCommand (string remotePath, string localPath, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool download = false, bool upload = false, bool deleteAfter = false, bool checkSize = false)
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
            }
        }
        public ClusterFileIOCommand(string fileName, string remoteDir, string localDir, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool download = false, bool upload = false, bool deleteAfter = false, bool checkSize = false)
        {
            SetPaths(fileName, remoteDir, localDir);

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
        public ClusterFileIOCommand(string fileName, string remoteDir, string localDir, ClusterFileIOCommand other)
        {
            SetPaths(fileName, remoteDir, localDir);

            this.Type = other.Type;
            this.checkExists = other.checkExists;
            this.getAttributes = other.getAttributes;
            this.deleteAfter = other.deleteAfter;
            this.checkSize = other.checkSize;
        }

        private void SetPaths(string remotePath, string localPath)
        {
            RemoteFileName = Path.GetFileName(remotePath);
            RemoteDir = Path.GetDirectoryName(remotePath)!.Replace('\\', '/');
            LocalFileName = Path.GetFileName(localPath);
            LocalDir = Path.GetDirectoryName(localPath)!;

        }
        private void SetPaths(string fileName, string remoteDir, string localDir)
        {
            RemoteFileName = fileName;
            RemoteDir = remoteDir.Replace('\\', '/');
            LocalFileName = fileName;
            LocalDir = localDir;
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
