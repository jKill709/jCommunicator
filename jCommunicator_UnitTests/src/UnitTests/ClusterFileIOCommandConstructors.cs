using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ClusterFileIOCommandConstructors_CloneConstructors
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

    public class ClusterFileIOCommandConstructors_PathParsingAndNormalization
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

    public class ClusterFileIOCommandConstructor_NormalOperation
    {
        [Theory]
        [InlineData(ClusterFileIOCommandType.Exists, true, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Attributes, false, true, false, false)]
        [InlineData(ClusterFileIOCommandType.Download, false, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Upload, false, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Move, false, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Delete, false, false, true, false)]
        public void Constructor__WithPathPathTypeBoolsFFFF_ShouldInitializeMInimumProperties(ClusterFileIOCommandType type, bool checkExists, bool getAttributes, bool deleteAfter, bool checkSize)
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";

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
            Assert.Equal(command.checkExists, checkExists);
            Assert.Equal(command.getAttributes, getAttributes);
            Assert.Equal(command.deleteAfter, deleteAfter);
            Assert.Equal(command.checkSize, checkSize);
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(true, true, true, true)]
        public void Constructor__WithPathPathTypeBoolsFFFF_ShouldInitializeExplicitProperties(bool checkExists, bool getAttributes, bool deleteAfter, bool checkSize)
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "\\local\\path\\file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            var command = new ClusterFileIOCommand(remotePath, localPath, type, checkExists, getAttributes, deleteAfter, checkSize);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.Equal(command.checkExists, checkExists);
            Assert.Equal(command.getAttributes, getAttributes);
            Assert.Equal(command.deleteAfter, deleteAfter);
            Assert.Equal(command.checkSize, checkSize);
        }

        [Theory]
        [InlineData(ClusterFileIOCommandType.Exists, true, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Attributes, false, true, false, false)]
        [InlineData(ClusterFileIOCommandType.Download, false, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Upload, false, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Move, false, false, false, false)]
        [InlineData(ClusterFileIOCommandType.Delete, false, false, true, false)]
        public void Constructor__WithDirDirFileNameTypeBoolsFFFF_ShouldInitializeMInimumProperties(ClusterFileIOCommandType type, bool checkExists, bool getAttributes, bool deleteAfter, bool checkSize)
        {
            // Setup
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            string fileName = "file.txt";

            // Steps
            var command = new ClusterFileIOCommand(remoteDir, localDir, fileName, type, false, false, false, false);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.Equal(command.checkExists, checkExists);
            Assert.Equal(command.getAttributes, getAttributes);
            Assert.Equal(command.deleteAfter, deleteAfter);
            Assert.Equal(command.checkSize, checkSize);
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(true, true, true, true)]
        public void Constructor__WithDirDirFileNameTypeBoolsFFFF_ShouldInitializeExplicitProperties(bool checkExists, bool getAttributes, bool deleteAfter, bool checkSize)
        {
            // Setup
            string remoteDir = "/remote/path";
            string localDir = "\\local\\path";
            string fileName = "file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;


            // Steps
            var command = new ClusterFileIOCommand(remoteDir, localDir, fileName, type, checkExists, getAttributes, deleteAfter, checkSize);

            // Expected Result
            Assert.Equal("/remote/path", command.RemoteDir);
            Assert.Equal("file.txt", command.RemoteFileName);
            Assert.Equal("/remote/path/file.txt", command.RemotePath);
            Assert.Equal("\\local\\path", command.LocalDir);
            Assert.Equal("file.txt", command.LocalFileName);
            Assert.Equal("\\local\\path\\file.txt", command.LocalPath);
            Assert.Equal(type, command.Type);
            Assert.Equal(command.checkExists, checkExists);
            Assert.Equal(command.getAttributes, getAttributes);
            Assert.Equal(command.deleteAfter, deleteAfter);
            Assert.Equal(command.checkSize, checkSize);
        }
    }

    public class ClusterFileIOCommandConstructors_BadInputs
    {
        [Fact]
        public void InvalidType_ThrowsArgumentException()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";
            int invalidValue = -1;

            // Steps
            Assert.Throws<ArgumentOutOfRangeException>(() => new ClusterFileIOCommand(remotePath, localPath, (ClusterFileIOCommandType)invalidValue, false, false, false, false));
        }

        [Fact]
        public void NullRemotePath_ThrowsArgumentNullException()
        {
            // Setup
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            Assert.Throws<System.ArgumentNullException>(() => new ClusterFileIOCommand(null, localPath, type, false, false, false, false));
        }

        [Fact]
        public void EmptyRemotePath_ThrowsArgumentException()
        {
            // Setup
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            Assert.Throws<ArgumentOutOfRangeException>(() => new ClusterFileIOCommand("", localPath, type, false, false, false, false));
        }
    }
}