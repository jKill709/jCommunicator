using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ClusterFileIOCommandConstructors_Group1
    {
        [Fact]
        public void Constructor_WithRemotePathLocalPathTypeCheckExistsFalseGetAttributesFalseDownloadTrueUploadFalseDeleteAfterFalseCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, false, false);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.False(command.checkExists);
            Assert.False(command.getAttributes);
            Assert.True(command.Type == ClusterFileIOCommandType.Download);
            Assert.False(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithRemotePathLocalPathTypeCheckExistsTrueGetAttributesTrueDownloadTrueUploadFalseDeleteAfterTrueCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Upload;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, true, true, true, false);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.True(command.checkExists);
            Assert.True(command.getAttributes);
            Assert.True(command.Type == ClusterFileIOCommandType.Upload);
            Assert.True(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithRemotePathLocalPathTypeCheckExistsFalseGetAttributesFalseDownloadFalseUploadTrueDeleteAfterFalseCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Move;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, false, true, false);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.False(command.checkExists);
            Assert.False(command.getAttributes);
            Assert.True(command.Type == ClusterFileIOCommandType.Move);
            Assert.True(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithRemotePathLocalPathTypeCheckExistsTrueGetAttributesFalseDownloadTrueUploadTrueDeleteAfterFalseCheckSizeFalse_ShouldInitializeAllProperties()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Delete;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, true, false, false, false);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.True(command.checkExists);
            Assert.False(command.getAttributes);
            Assert.True(command.Type == ClusterFileIOCommandType.Delete);
            Assert.True(command.deleteAfter);
            Assert.False(command.checkSize);
        }

        [Fact]
        public void Constructor_WithRemotePathLocalPathTypeCheckExistsFalseGetAttributesTrueDownloadTrueUploadFalseDeleteAfterFalseCheckSizeTrue_ShouldInitializeAllProperties()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Attributes;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, false, true, false, false);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.False(command.checkExists);
            Assert.True(command.getAttributes);
            Assert.False(command.deleteAfter);
            Assert.False(command.checkSize);
        }
    }
}