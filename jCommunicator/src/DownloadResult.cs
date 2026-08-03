namespace jCommunicator
{
    public class DownloadResult
    {
        public string RemoteDir { get; set; } = "";
        public string RemoteFileName { get; set; } = "";
        public string RemotePath => RemoteDir + '/' + RemoteFileName;

        public string LocalDir { get; set; } = "";
        public string LocalFileName { get; set; } = "";
        public string LocalPath => System.IO.Path.Combine(LocalDir, LocalFileName);

        public long FileSize { get; set; }
        public DateTime LastWriteTime { get; set; }

        public bool FileExists { get; set; }
        public bool DownloadSucceeded { get; set; }
        public bool DeleteSucceeded { get; set; }

        public Exception? Exception { get; set; }

        public bool Success => FileExists && DownloadSucceeded && DeleteSucceeded && Exception == null;

        public DownloadResult(string remotePath, string localPath)
        {
            RemoteFileName = Path.GetFileName(remotePath);
            RemoteDir = Path.GetDirectoryName(remotePath)!.Replace('\\', '/');
            LocalFileName = Path.GetFileName(localPath);
            LocalDir = Path.GetDirectoryName(localPath)!;
        }

        public DownloadResult(string fileName, string remoteDir, string localDir)
        {
            RemoteFileName = fileName;
            RemoteDir = remoteDir.Replace('\\', '/');
            LocalFileName = fileName;
            LocalDir = localDir;
        }

        public override string ToString()
        {
            return
                $"Remote: {RemotePath}\n" +
                $"Local : {LocalPath}\n" +
                $"Exists: {FileExists}\n" +
                $"Size  : {FileSize}\n" +
                $"Date  : {LastWriteTime}\n" +
                $"Downloaded: {DownloadSucceeded}\n" +
                $"Deleted   : {DeleteSucceeded}\n" +
                $"Success   : {Success}\n" +
                $"{(Exception != null ? Exception.Message : "")}";
        }
    }
}
