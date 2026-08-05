using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ClusterFileIOCommandConstructors_Group2
    {
        [Fact]
        public void Constructor_WithFileNameRemoteDirLocalDirTypeCheckExistsFalseGetAttributesFalseDownloadTrueUploadFalseDeleteAfterFalseCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string fileName = "file.txt";
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(fileName, remoteDir, localDir, type, false, false, false, false);

            // Expected Result
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.False(command.checkExists);
            Assert.False(command.getAttributes);
            Assert.False(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithFileNameRemoteDirLocalDirTypeCheckExistsTrueGetAttributesTrueDownloadTrueUploadFalseDeleteAfterTrueCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string fileName = "file.txt";
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Upload;

            // Steps
            var command = new ClusterFileIOCommand(fileName, remoteDir, localDir, type, true, true, true, false);

            // Expected Result
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.True(command.checkExists);
            Assert.True(command.getAttributes);
            Assert.True(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithFileNameRemoteDirLocalDirTypeCheckExistsFalseGetAttributesFalseDownloadFalseUploadTrueDeleteAfterFalseCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string fileName = "file.txt";
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Move;

            // Steps
            var command = new ClusterFileIOCommand(fileName, remoteDir, localDir, type, false, false, false, false);

            // Expected Result
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.False(command.checkExists);
            Assert.False(command.getAttributes);
            Assert.False(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithFileNameRemoteDirLocalDirTypeCheckExistsTrueGetAttributesFalseDownloadTrueUploadTrueDeleteAfterFalseCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string fileName = "file.txt";
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Delete;

            // Steps
            var command = new ClusterFileIOCommand(fileName, remoteDir, localDir, type, true, false, false, false);

            // Expected Result
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.True(command.checkExists);
            Assert.False(command.getAttributes);
            Assert.True(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithFileNameRemoteDirLocalDirTypeCheckExistsFalseGetAttributesTrueDownloadTrueUploadFalseDeleteAfterFalseCheckSizeTrue_ShouldInitializeAllProperties()
        {
            // Setup
            string fileName = "file.txt";
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Attributes;

            // Steps
            var command = new ClusterFileIOCommand(fileName, remoteDir, localDir, type, false, true, false, false);

            // Expected Result
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.False(command.checkExists);
            Assert.True(command.getAttributes);
            Assert.False(command.deleteAfter);
            Assert.False(command.checkSize);
        }
    }
}