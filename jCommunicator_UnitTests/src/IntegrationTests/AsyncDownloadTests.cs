using System;
using System.IO;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class AsyncDownloadTests : CommunicatorTestBase
    {
        private const string TempDirectory = "/tmp/";

        public AsyncDownloadTests()
        {
            Assert.True(_communicator!.Connect());

            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);
            _communicator.AddNodeTunnel(_node2Host, _node2User, _node2Pass);
        }

        [Fact]
        public async Task CopyNodeToPCAsync_FromNode1_CorrectContents()
        {
            string node1File = $"{TempDirectory}Node1_{Guid.NewGuid():N}.txt";

            CreateNodeFile(_communicator!, _node1Host, _node1User, node1File, "Node One");

            try
            {
                string localFile = Path.GetTempFileName();
                DownloadResult result = await _communicator.CopyNodeToPCAsync(node1File, localFile, _node1Host);
                Assert.True(result.Success);
                Assert.Null(result.Exception);
                Assert.True(File.Exists(localFile));
                string contents = File.ReadAllText(localFile).Trim();
                Assert.Equal("Node One", contents);
            }
            finally
            {
                //_communicator.DeleteNodeFile(node1File, _node1Host);
            }
        }

        [Fact]
        public async Task CopyNodeToPCAsync_MissingRemoteFile_ReturnsFileNotFound()
        {
            string remote = $"{TempDirectory}{Guid.NewGuid():N}.txt";
            string local = Path.GetTempFileName();

            DownloadResult result =
                await _communicator.CopyNodeToPCAsync(remote, local, _node1Host);

            Assert.False(result.Success);
            Assert.False(result.FileExists);
            Assert.False(result.DownloadSucceeded);
            Assert.False(result.DeleteSucceeded);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task CopyNodeToPCAsync_RemovesRemoteFile()
        {
            string remote = $"{TempDirectory}{Guid.NewGuid():N}.txt";

            CreateNodeFile(_communicator, _node1Host, _node1User, remote, "abc");

            string local = Path.GetTempFileName();

            DownloadResult result =
                await _communicator.CopyNodeToPCAsync(remote, local, _node1Host);

            Assert.True(result.Success);

            Assert.False(
                _communicator.NodeFileExists(remote, _node1Host));
        }

        [Fact]
        public async Task CopyNodeToPCAsync_ReturnsMetadata()
        {
            string text = "Hello World";
            string remote = $"{TempDirectory}{Guid.NewGuid():N}.txt";

            CreateNodeFile(_communicator, _node1Host, _node1User, remote, text);

            string local = Path.GetTempFileName();

            DownloadResult result = await _communicator.CopyNodeToPCAsync(remote, local, _node1Host);

            Assert.Equal(text.Length + 1, result.FileSize); // +1 for newline character

            Assert.True(result.LastWriteTime > DateTime.Now.AddMinutes(-5));
        }

        [Fact]
        public async Task CopyNodeToPCAsync_CreatesDirectory()
        {
            string remote = $"{TempDirectory}{Guid.NewGuid():N}.txt";

            CreateNodeFile(_communicator, _node1Host, _node1User, remote, "abc");

            string folder =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString());

            string local =
                Path.Combine(folder, "test.txt");

            DownloadResult result =
                await _communicator.CopyNodeToPCAsync(remote, local, _node1Host);

            Assert.True(result.Success);

            Assert.True(Directory.Exists(folder));
            Assert.True(File.Exists(local));
        }

        [Fact]
        public async Task CopyNodeToPCAsync_MultipleDownloads()
        {
            var tasks = new List<Task<DownloadResult>>();

            for (int i = 0; i < 10; i++)
            {
                string remote =
                    $"{TempDirectory}{Guid.NewGuid():N}.txt";

                CreateNodeFile(
                    _communicator,
                    _node1Host,
                    _node1User,
                    remote,
                    $"File {i}");

                string local =
                    Path.Combine(
                        Path.GetTempPath(),
                        Guid.NewGuid() + ".txt");

                tasks.Add(
                    _communicator.CopyNodeToPCAsync(
                        remote,
                        local,
                        _node1Host));
            }

            DownloadResult[] results =
                await Task.WhenAll(tasks);

            Assert.All(results,
                r => Assert.True(r.Success));
        }
    }
}