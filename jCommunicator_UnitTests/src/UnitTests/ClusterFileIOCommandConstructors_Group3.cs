using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ClusterFileIOCommandConstructors_Group3
    {
        [Fact]
        public void Constructor_CloneFromExistingInstance_RemotePathLocalPathOther_ShouldCreateIndependentClone()
        {
            // Setup
            string sourceRemotePath = "/remote/path/file.txt";
            string sourceLocalPath = "/local/path/file.txt";
            var sourceCommand = new ClusterFileIOCommand(sourceRemotePath, sourceLocalPath, ClusterFileIOCommandType.Download, true, true, true, false);

            // Steps
            var clonedCommand = new ClusterFileIOCommand(sourceRemotePath, sourceLocalPath, sourceCommand);

            // Expected Result
            Assert.Equal(sourceCommand.RemoteDir, clonedCommand.RemoteDir);
            Assert.Equal(sourceCommand.RemoteFileName, clonedCommand.RemoteFileName);
            Assert.Equal(sourceCommand.RemotePath, clonedCommand.RemotePath);
            Assert.Equal(sourceCommand.LocalDir, clonedCommand.LocalDir);
            Assert.Equal(sourceCommand.LocalFileName, clonedCommand.LocalFileName);
            Assert.Equal(sourceCommand.LocalPath, clonedCommand.LocalPath);
            Assert.Equal(sourceCommand.Type, clonedCommand.Type);
            Assert.Equal(sourceCommand.checkExists, clonedCommand.checkExists);
            Assert.Equal(sourceCommand.getAttributes, clonedCommand.getAttributes);
            Assert.Equal(sourceCommand.deleteAfter, clonedCommand.deleteAfter);
            Assert.Equal(sourceCommand.checkSize, clonedCommand.checkSize);

            // New instance is independent from source
            clonedCommand.checkExists = false;
            Assert.False(clonedCommand.checkExists);
            Assert.True(sourceCommand.checkExists);
        }

        [Fact]
        public void Constructor_CloneFromExistingInstance_FileNameRemoteDirLocalDirOther_ShouldCreateIndependentClone()
        {
            // Setup
            string sourceFileName = "file.txt";
            string sourceRemoteDir = "/remote/path/";
            string sourceLocalDir = "/local/path/";
            var sourceCommand = new ClusterFileIOCommand(sourceFileName, sourceRemoteDir, sourceLocalDir, ClusterFileIOCommandType.Upload, true, false, false, false);

            // Steps
            var clonedCommand = new ClusterFileIOCommand(sourceFileName, sourceRemoteDir, sourceLocalDir, sourceCommand);

            // Expected Result
            Assert.Equal(sourceCommand.RemoteDir, clonedCommand.RemoteDir);
            Assert.Equal(sourceCommand.RemoteFileName, clonedCommand.RemoteFileName);
            Assert.Equal(sourceCommand.RemotePath, clonedCommand.RemotePath);
            Assert.Equal(sourceCommand.LocalDir, clonedCommand.LocalDir);
            Assert.Equal(sourceCommand.LocalFileName, clonedCommand.LocalFileName);
            Assert.Equal(sourceCommand.LocalPath, clonedCommand.LocalPath);
            Assert.Equal(sourceCommand.Type, clonedCommand.Type);
            Assert.Equal(sourceCommand.checkExists, clonedCommand.checkExists);
            Assert.Equal(sourceCommand.getAttributes, clonedCommand.getAttributes);
            Assert.Equal(sourceCommand.deleteAfter, clonedCommand.deleteAfter);
            Assert.Equal(sourceCommand.checkSize, clonedCommand.checkSize);

            // New instance is independent from source
            clonedCommand.checkExists = false;
            Assert.False(clonedCommand.checkExists);
            Assert.True(sourceCommand.checkExists);
        }
    }
}