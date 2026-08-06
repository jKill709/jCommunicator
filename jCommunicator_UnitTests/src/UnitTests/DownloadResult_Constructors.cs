using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class DownloadResult_Constructors
    {
        [Fact]
        public void Constructor_WithFileNameRemoteDirLocalDirCommand_ShouldInitializeAllProperties()
        {
            // Setup
            string fileName = "test.txt";
            string remoteDir = "/remote/path";
            string localDir = "/local/path";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            var command = new ClusterFileIOCommand(fileName, remoteDir, localDir, type, false, false, false, false);

            // Steps
            var result = new DownloadResult(fileName, remoteDir, localDir, command);

            // Expected Result
            Assert.Equal(command, result.Command);
            Assert.Null(result.Attributes);
            Assert.False(result.FileExists);
            Assert.False(result.MainProcedureSucceeded);
            Assert.False(result.DeleteSucceeded);
            Assert.Null(result.Exception);
            Assert.False(result.Success);
        }

        [Fact]
        public void Constructor_WithRemotePathLocalPathCommand_ShouldInitializeAllProperties()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Upload;

            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.Equal(command, result.Command);
            Assert.Null(result.Attributes);
            Assert.False(result.FileExists);
            Assert.False(result.MainProcedureSucceeded);
            Assert.False(result.DeleteSucceeded);
            Assert.Null(result.Exception);
            Assert.False(result.Success);
        }

        [Fact]
        public void Constructor_ShouldPreserveCommandReferenceEquality()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            var command = new ClusterFileIOCommand(remotePath, localPath, type, true, true, true, false);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.Equal(command, result.Command);
        }

        [Fact]
        public void Constructor_WithCheckExistsFlag_ShouldSetcheckExistsToTrue()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Exists, true, false, false, false);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.True(result.Command.checkExists);
        }

        [Fact]
        public void Constructor_WithGetAttributesFlag_ShouldSetgetAttributesToTrue()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Attributes, false, true, false, false);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.True(result.Command.getAttributes);
        }

        [Fact]
        public void Constructor_WithDeleteFlag_ShouldSetdeleteAfterToTrue()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Delete, false, false, false, true);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.True(result.Command.deleteAfter);
        }

        [Fact]
        public void Constructor_WithCheckSizeFlag_ShouldSetcheckSizeToTrue()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download, false, false, false, false);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.False(result.Command.checkSize);
        }
    }

}