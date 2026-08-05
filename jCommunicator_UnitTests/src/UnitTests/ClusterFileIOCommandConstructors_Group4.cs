using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ClusterFileIOCommandConstructors_Group4
    {
        [Fact]
        public void RemoteDir_IsNormalizedToForwardSlashes()
        {
            // Setup
            string remotePath = "~\\path\\to\\file.txt";
            string localPath = "C:\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Expected Result
            Assert.Equal("~/path/to", command.RemoteDir);
        }

        [Fact]
        public void LocalPath_UsesPathCombineForConstruction()
        {
            // Setup
            string remotePath = "~/path/to/file.txt";
            string localPath = "C:/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Expected Result
            Assert.Equal("C:\\local\\path\\file.txt", command.LocalPath);
        }

        [Fact]
        public void RemotePath_EqualsRemoteDir_Plus_ForumSlash_Plus_RemoteFileName()
        {
            // Setup
            string remotePath = "~\\path\\to\\file.txt";
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Expected Result
            Assert.Equal("~/path/to/file.txt", command.RemotePath);
        }

        [Fact]
        public void LocalFileName_EqualsFinalComponentOfLocalPath()
        {
            // Setup
            string remotePath = "C:\\path\\to\\file.txt";
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Expected Result
            Assert.Equal("file.txt", command.LocalFileName);
        }

        [Fact]
        public void RemoteFileName_EqualsFinalComponentOfRemotePath()
        {
            // Setup
            string remotePath = "C:\\path\\to\\file.txt";
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Expected Result
            Assert.Equal("file.txt", command.RemoteFileName);
        }
    }
}