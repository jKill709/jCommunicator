using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCommunicator
{
    public class DownloadResult
    {
        public string RemotePath { get; set; } = "";
        public string LocalPath { get; set; } = "";

        public bool FileExists { get; set; }
        public bool DownloadSucceeded { get; set; }
        public bool DeleteSucceeded { get; set; }

        public long FileSize { get; set; }
        public DateTime LastWriteTime { get; set; }

        public Exception? Exception { get; set; }

        public bool Success =>
            FileExists &&
            DownloadSucceeded &&
            DeleteSucceeded &&
            Exception == null;

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
