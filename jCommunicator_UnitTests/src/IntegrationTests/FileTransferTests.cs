using System;
using System.IO;
using System.Linq;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    /// <summary>
    /// Integration tests for all file transfer methods.
    ///
    /// These tests require:
    ///     - Running Hub
    ///     - Running Node
    ///     - SSH enabled
    ///     - SFTP enabled
    ///
    /// Every test creates unique temporary files and cleans up after itself.
    /// </summary>
    public class FileTransferTests : CommunicatorTestBase
    {
        private const string HubTempDirectory = "/tmp/";
        private const string NodeTempDirectory = "/tmp/";

        #region Helpers

        private static string RandomName()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string CreateLocalTestFile(string contents)
        {
            string file = Path.Combine(
                Path.GetTempPath(),
                $"jCommunicatorTest_{RandomName()}.txt");

            File.WriteAllText(file, contents);

            return file;
        }

        private static byte[] CreateRandomBytes(int length)
        {
            Random r = new();
            byte[] bytes = new byte[length];
            r.NextBytes(bytes);
            return bytes;
        }

        private static string CreateLocalBinaryFile(int size)
        {
            string file = Path.Combine(
                Path.GetTempPath(),
                $"jCommunicatorBinary_{RandomName()}.bin");

            File.WriteAllBytes(file, CreateRandomBytes(size));

            return file;
        }

        private static void AssertFilesEqual(string expected, string actual)
        {
            Assert.True(File.Exists(expected));
            Assert.True(File.Exists(actual));

            byte[] a = File.ReadAllBytes(expected);
            byte[] b = File.ReadAllBytes(actual);

            Assert.Equal(a.Length, b.Length);
            Assert.True(a.SequenceEqual(b));
        }

        #endregion

        [Fact]
        public void CopyPCToHub_TextFile()
        {
            _communicator!.Connect();

            string local = CreateLocalTestFile("Hello from PC");
            string hub = $"{HubTempDirectory}{RandomName()}.txt";

            try
            {
                _communicator.CopyPCtoHub(local, hub);

                Assert.True(_communicator.HubFileExists(hub));
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                File.Delete(local);
            }
        }

        [Fact]
        public void CopyPCToHub_EmptyFile()
        {
            _communicator!.Connect();

            string local = CreateLocalTestFile(String.Empty);
            string hub = $"{HubTempDirectory}{RandomName()}.txt";

            try
            {
                _communicator.CopyPCtoHub(local, hub);

                Assert.True(_communicator.HubFileExists(hub));
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                File.Delete(local);
            }
        }

        [Fact]
        public void CopyPCToHub_BinaryFile()
        {
            _communicator!.Connect();

            string local = CreateLocalBinaryFile(4096);
            string hub = $"{HubTempDirectory}{RandomName()}.bin";

            try
            {
                _communicator.CopyPCtoHub(local, hub);

                Assert.True(_communicator.HubFileExists(hub));
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                File.Delete(local);
            }
        }

        [Fact]
        public void CopyHubToPC_TextFile()
        {
            _communicator!.Connect();

            string hub = $"{HubTempDirectory}{RandomName()}.txt";
            string local = Path.Combine(Path.GetTempPath(), $"{RandomName()}.txt");

            CreateHubFile(_communicator, hub, "Hub File");

            try
            {
                _communicator.CopyHubToPC(hub, local);

                Assert.True(File.Exists(local));
                Assert.Equal("Hub File\n", File.ReadAllText(local).Replace("\r\n", "\n"));
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                if (File.Exists(local))
                    File.Delete(local);
            }
        }

        [Fact]
        public void CopyHubToNode_TextFile()
        {
            _communicator!.Connect();

            _communicator.AddNodeSFTP(_node1Host, _node1User);

            string hub = $"{HubTempDirectory}{RandomName()}.txt";
            string node = $"{NodeTempDirectory}{RandomName()}.txt";

            CreateHubFile(_communicator, hub, "Hub To Node");

            try
            {
                _communicator.CopyHubToNode(
                    hub,
                    node,
                    _node1Host,
                    _node1User);

                Assert.True(
                    _communicator.NodeFileExists(
                        node,
                        _node1Host));
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                if (_communicator.NodeFileExists(node, _node1Host))
                    _communicator.DeleteNodeFile(node, _node1Host);
            }
        }

        [Fact]
        public void CopyNodeToPC_TextFile()
        {
            _communicator!.Connect();

            _communicator.AddNodeSFTP(_node1Host, _node1User);

            string node = $"{NodeTempDirectory}{RandomName()}.txt";
            string local = Path.Combine(Path.GetTempPath(), $"{RandomName()}.txt");

            CreateNodeFile(
                _communicator,
                _node1Host,
                _node1User,
                node,
                "Node File");

            try
            {
                _communicator.CopyNodeToPC(
                    node,
                    local,
                    _node1Host);

                Assert.True(File.Exists(local));
                Assert.Equal("Node File\n", File.ReadAllText(local).Replace("\r\n", "\n"));
            }
            finally
            {
                if (_communicator.NodeFileExists(node, _node1Host))
                    _communicator.DeleteNodeFile(node, _node1Host);

                if (File.Exists(local))
                    File.Delete(local);
            }
        }

        [Fact]
        public void CopyPCToNode_TextFile()
        {
            _communicator!.Connect();

            _communicator.AddNodeSFTP(_node1Host, _node1User);

            string local = CreateLocalTestFile("PC To Node");
            string node = $"{NodeTempDirectory}{RandomName()}.txt";

            try
            {
                _communicator.CopyPCtoNode(
                    local,
                    node,
                    _node1Host);

                Assert.True(
                    _communicator.NodeFileExists(
                        node,
                        _node1Host));
            }
            finally
            {
                if (_communicator.NodeFileExists(node, _node1Host))
                    _communicator.DeleteNodeFile(node, _node1Host);

                File.Delete(local);
            }
        }

        [Fact]
        public void RoundTrip_PC_Hub_PC_PreservesContents()
        {
            _communicator!.Connect();

            string original = CreateLocalBinaryFile(32768);

            string hub = $"{HubTempDirectory}{RandomName()}.bin";

            string downloaded = Path.Combine(
                Path.GetTempPath(),
                $"{RandomName()}.bin");

            try
            {
                _communicator.CopyPCtoHub(original, hub);

                _communicator.CopyHubToPC(hub, downloaded);

                AssertFilesEqual(original, downloaded);
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                if (File.Exists(original))
                    File.Delete(original);

                if (File.Exists(downloaded))
                    File.Delete(downloaded);
            }
        }

        [Fact]
        public void RoundTrip_PC_Node_PC_PreservesContents()
        {
            _communicator!.Connect();

            _communicator.AddNodeSFTP(_node1Host, _node1User);

            string original = CreateLocalBinaryFile(32768);

            string node = $"{NodeTempDirectory}{RandomName()}.bin";

            string downloaded = Path.Combine(
                Path.GetTempPath(),
                $"{RandomName()}.bin");

            try
            {
                _communicator.CopyPCtoNode(
                    original,
                    node,
                    _node1Host);

                _communicator.CopyNodeToPC(
                    node,
                    downloaded,
                    _node1Host);

                AssertFilesEqual(original, downloaded);
            }
            finally
            {
                if (_communicator.NodeFileExists(node, _node1Host))
                    _communicator.DeleteNodeFile(node, _node1Host);

                if (File.Exists(original))
                    File.Delete(original);

                if (File.Exists(downloaded))
                    File.Delete(downloaded);
            }
        }

        [Fact]
        public void RoundTrip_PC_Hub_Node_PC_PreservesContents()
        {
            _communicator!.Connect();

            _communicator.AddNodeSFTP(_node1Host, _node1User);

            string original = CreateLocalBinaryFile(65536);

            string hub = $"{HubTempDirectory}{RandomName()}.bin";
            string node = $"{NodeTempDirectory}{RandomName()}.bin";

            string downloaded = Path.Combine(
                Path.GetTempPath(),
                $"{RandomName()}.bin");

            try
            {
                _communicator.CopyPCtoHub(original, hub);

                _communicator.CopyHubToNode(
                    hub,
                    node,
                    _node1Host,
                    _node1User);

                _communicator.CopyNodeToPC(
                    node,
                    downloaded,
                    _node1Host);

                AssertFilesEqual(original, downloaded);
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                if (_communicator.NodeFileExists(node, _node1Host))
                    _communicator.DeleteNodeFile(node, _node1Host);

                if (File.Exists(original))
                    File.Delete(original);

                if (File.Exists(downloaded))
                    File.Delete(downloaded);
            }
        }

        [Fact]
        public void CopyLargeFile_OneMegabyte()
        {
            _communicator!.Connect();

            string original = CreateLocalBinaryFile(1024 * 1024);

            string hub = $"{HubTempDirectory}{RandomName()}.bin";

            string downloaded = Path.Combine(
                Path.GetTempPath(),
                $"{RandomName()}.bin");

            try
            {
                _communicator.CopyPCtoHub(original, hub);
                _communicator.CopyHubToPC(hub, downloaded);

                AssertFilesEqual(original, downloaded);
            }
            finally
            {
                if (_communicator.HubFileExists(hub))
                    _communicator.DeleteHubFile(hub);

                if (File.Exists(original))
                    File.Delete(original);

                if (File.Exists(downloaded))
                    File.Delete(downloaded);
            }
        }
    }
}