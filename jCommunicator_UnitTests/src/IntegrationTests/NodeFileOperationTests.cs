using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class NodeFileOperationTests : CommunicatorTestBase
    {
        // --- Configuration ---
        private readonly string _testDirectory = "/tmp";

        // --- State ---
        private readonly Communicator _communicator;

        private readonly List<string> _createdFiles = new();


        public NodeFileOperationTests()
        {
            logger.Initialize("NodeFileOperationTests");

            _communicator = new Communicator(
                _hubHost,
                _hubUser,
                _hubPass);

            _communicator.Connect();

            // Establish SFTP tunnel
            _communicator.AddNodeTunnel(
                _node1Host,
                _node1User,
                _node1Pass);
        }


        private string CreateNodeFile(
            string path,
            string contents = "Contents")
        {
            return _communicator.ExecuteNodeCommand(
                $"echo '{contents}' > {path}",
                _node1Host,
                _node1User);
        }


        private string NewNodeFile(string extension = ".txt")
        {
            string path =
                $"{_testDirectory}/communicator_test_{Guid.NewGuid()}{extension}";

            _createdFiles.Add(path);

            return path;
        }


        [Fact]
        public void NodeFileExists_FileExists_ReturnsTrue()
        {
            string file = NewNodeFile();

            CreateNodeFile(file);

            bool exists =
                _communicator.NodeFileExists(
                    file,
                    _node1Host);

            Assert.True(exists);
        }


        [Fact]
        public void NodeFileExists_FileMissing_ReturnsFalse()
        {
            string file =
                $"{_testDirectory}/missing_{Guid.NewGuid()}.txt";

            bool exists =
                _communicator.NodeFileExists(
                    file,
                    _node1Host);

            Assert.False(exists);
        }


        [Fact]
        public void NodeFileLastModified_ReturnsRecentTime()
        {
            string file = NewNodeFile();

            CreateNodeFile(file);

            DateTime? modified =
                _communicator.NodeFileLastModified(
                    file,
                    _node1Host);


            Assert.True(
                modified <= DateTime.Now);

            Assert.True(
                modified > DateTime.Now.AddMinutes(-5));
        }


        [Fact]
        public void GetListOfNodeFiles_ReturnsCreatedFile()
        {
            string file = NewNodeFile();

            CreateNodeFile(file);


            var files =
                _communicator.GetListOfNodeFiles(
                    _testDirectory,
                    ".txt",
                    _node1Host,
                    _node1User);


            Assert.Contains(
                files,
                f => f.Contains(file));
        }


        [Fact]
        public void DeleteNodeFile_RemovesFile()
        {
            string file = NewNodeFile();

            CreateNodeFile(file);


            Assert.True(
                _communicator.NodeFileExists(
                    file,
                    _node1Host));


            _communicator.DeleteNodeFile(
                file,
                _node1Host);


            Assert.False(
                _communicator.NodeFileExists(
                    file,
                    _node1Host));
        }


        [Fact]
        public void DeleteNodeFile_MissingFile_DoesNotThrow()
        {
            string file =
                $"{_testDirectory}/missing_{Guid.NewGuid()}.txt";


            Exception? exception = null;

            try
            {
                _communicator.DeleteNodeFile(
                    file,
                    _node1Host);
            }
            catch (Exception ex)
            {
                exception = ex;
            }


            Assert.Null(exception);
        }


        [Fact]
        public void MoveNodeFile_RenamesFile()
        {
            string source = NewNodeFile();
            string destination = NewNodeFile();


            CreateNodeFile(
                source,
                "Move Test");


            _communicator.MoveNodeFile(
                source,
                destination,
                _node1Host,
                _node1User);


            Assert.False(
                _communicator.NodeFileExists(
                    source,
                    _node1Host));


            Assert.True(
                _communicator.NodeFileExists(
                    destination,
                    _node1Host));
        }


        [Fact]
        public void MoveNodeFile_InvalidSource_Throws()
        {
            string source =
                $"{_testDirectory}/does_not_exist.txt";

            string destination =
                NewNodeFile();


            Assert.ThrowsAny<Exception>(() =>
            {
                _communicator.MoveNodeFile(
                    source,
                    destination,
                    _node1Host,
                    _node1User);
            });
        }


        [Fact]
        public void NodeFileOperations_HandleEmptyFile()
        {
            string file = NewNodeFile();


            CreateNodeFile(
                file,
                "");


            Assert.True(
                _communicator.NodeFileExists(
                    file,
                    _node1Host));


            DateTime? modified =
                _communicator.NodeFileLastModified(
                    file,
                    _node1Host);


            Assert.NotEqual(
                default,
                modified);
        }


        [Fact]
        public void NodeFileOperations_HandleLargeFile()
        {
            string file = NewNodeFile();


            // ~10 MB file
            _communicator.ExecuteNodeCommand(
                $"dd if=/dev/zero of={file} bs=1M count=10 status=none",
                _node1Host,
                _node1User);


            Assert.True(
                _communicator.NodeFileExists(
                    file,
                    _node1Host));
        }


        [Fact]
        public void NodeFileOperations_WithInvalidNode_Throws()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                _communicator.NodeFileExists(
                    "/tmp/test.txt",
                    "invalid-node");
            });
        }


        public void Dispose()
        {
            // Cleanup test files
            foreach (string file in _createdFiles)
            {
                try
                {
                    _communicator.DeleteNodeFile(
                        file,
                        _node1Host);
                }
                catch
                {
                    // Cleanup should never fail the test
                }
            }


            _communicator.Disconnect();
        }
    }
}