using jCommunicator;
using mLogger;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace jCommunicator.Tests
{
    public class CommunicatorTestBase : IDisposable
    {
        // --- Configuration ---
        protected const string hubHost = "Hub1.local";  // Change to your actual Hub IP
        protected const string hubUser = "camcpp";      // Change to your actual Hub User
        protected const string hubPass = "cam";         // Change to your actual Hub _hubPass
        protected const string node1Host = "10.0.0.11"; // Change to your actual Hub IP
        protected const string node1User = "camcpp";    // Change to your actual Hub User
        protected const string node1Pass = "cam";       // Change to your actual Hub _hubPass
        protected const string node2Host = "10.0.0.12"; // Change to your actual Hub IP
        protected const string node2User = "camcpp";    // Change to your actual Hub User
        protected const string node2Pass = "cam";       // Change to your actual Hub _hubPass
        protected const string remoteTempDirectory = "/tmp/";
        protected const string localTempDirectory = "C:\\Windows\\Temp\\";

        // --- State ---
        protected Communicator _com;

        public Logger _logger = Logger.Instance;
        public InMemorySink _logSink = new InMemorySink();

        public CommunicatorTestBase()
        {
            _logger.Initialize("CommunicatorTestBase");
            _logger.AddSink(_logSink);

            _com = new Communicator(hubHost, hubUser, hubPass);

            Task.Run(async () => await _com.ConnectAsync()).Wait();
        }
        public void Dispose()
        {
            if (_com != null)
                _com.DisconnectAsync();
        }

        protected string GetRemoteTempFilePath()
        {
            return $"{remoteTempDirectory}TestFile_{Guid.NewGuid():N}.txt";
        }
        protected string GetLocalTempFilePath()
        {
            return $"{localTempDirectory}TestFile_{Guid.NewGuid():N}.txt";
        }
        protected async Task<string> CreateHubFile(Communicator com, string path, string contents = "Contents")
        {
            await com.ExecuteHubCommandAsync($"echo '{contents}' > {path}");
            return path;
        }
        protected async Task<string> CreateNodeFile(Communicator com, string host, string username, string path, string contents = "Contents")
        {
            await com.ExecuteNodeCommandAsync($"echo '{contents}' > {path}", host, username);
            return path;
        }

        protected DownloadResult CreateDownloadResult(string remotePath, string localPath, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool download = false, bool upload = false, bool deleteAfter = false, bool checkSize = false)
        {
            return new DownloadResult(remotePath, localPath, new ClusterFileIOCommand(remotePath, localPath, type, checkExists, getAttributes, deleteAfter, checkSize));
        }

        protected DownloadResult CreateDownloadResult(string fileName, string remoteDir, string localDir, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool download = false, bool upload = false, bool deleteAfter = false, bool checkSize = false)
        {
            return new DownloadResult(fileName, remoteDir, localDir, new ClusterFileIOCommand(fileName, remoteDir, localDir, type, checkExists, getAttributes, deleteAfter, checkSize));
        }
    }
}
