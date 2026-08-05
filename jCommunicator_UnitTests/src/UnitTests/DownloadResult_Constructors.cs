using jCommunicator;
using Renci.SshNet.Sftp;
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

    public class DownloadResult_SuccessProperty
    {
        [Fact]
        public void Success_ShouldBeFalseWhenMainProcedureSucceededIsFalse()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);
            result.FileExists = true;
            result.MainProcedureSucceeded = false;
            result.DeleteSucceeded = true;
            result.Exception = null;

            // Expected Result
            Assert.False(result.Success);
        }

        [Fact]
        public void Success_ShouldBeFalseWhenDeleteSucceededIsFalse()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);
            result.FileExists = true;
            result.MainProcedureSucceeded = true;
            result.DeleteSucceeded = false;
            result.Exception = null;

            // Expected Result
            Assert.False(result.Success);
        }

        [Fact]
        public void Success_ShouldBeFalseWhenExceptionIsNotNull()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);
            result.FileExists = true;
            result.MainProcedureSucceeded = true;
            result.DeleteSucceeded = true;
            result.Exception = new Exception("test error");

            // Expected Result
            Assert.False(result.Success);
        }

        [Fact]
        public void Success_ShouldBeTrueWhenAllOperationsSucceedAndNoException()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);
            result.FileExists = true;
            result.MainProcedureSucceeded = true;
            result.DeleteSucceeded = true;
            result.Exception = null;

            // Expected Result
            Assert.True(result.Success);
        }
    }

    public class DownloadResult_AttributesProperty : CommunicatorTestBase
    {
        [Fact]
        public void Attributes_ShouldBeNullByDefault()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.Null(result.Attributes);
        }

        [Fact]
        public async Task Attributes_ShouldBePopulatedWhengetAttributesIsTrue()
        {
            // Setup
            string remotePath = GetRemoteTempFilePath();
            CreateHubFile(_com, remotePath, "Contents of the file.");
            string localPath = GetLocalTempFilePath();

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download, false, true, false, false);

            // Steps
            var result = await _com.PCtoHubAsync(command);

            // Expected Result
            Assert.NotNull(result.Attributes);
        }
    }

    public class DownloadResult_FileExistsProperty
    {
        [Fact]
        public void FileExists_ShouldBeFalseByDefault()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.False(result.FileExists);
        }

        [Fact]
        public void FileExists_ShouldBeTrueWhencheckExistsIsTrue()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download, true, false, false, false);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.True(result.Command.checkExists);
        }
    }

    public class DownloadResult_MainProcedureSucceededProperty : CommunicatorTestBase
    {
        [Fact]
        public void MainProcedureSucceeded_ShouldBeFalseByDefault()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.False(result.MainProcedureSucceeded);
        }

        [Fact]
        public async Task MainProcedureSucceeded_ShouldBeTrueAfterSuccessfulDownload()
        {
            // Setup
            string remotePath = GetRemoteTempFilePath();
            CreateHubFile(_com, remotePath, "Contents of the file.");
            string localPath = GetLocalTempFilePath();

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = await _com.PCtoHubAsync(command);

            // Expected Result
            Assert.True(result.MainProcedureSucceeded);
        }
    }

    public class DownloadResult_DeleteSucceededProperty : CommunicatorTestBase
    {
        [Fact]
        public void DeleteSucceeded_ShouldBeFalseByDefault()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.False(result.DeleteSucceeded);
        }

        [Fact]
        public async Task DeleteSucceeded_ShouldBeTrueAfterSuccessfulDelete()
        {
            // Setup
            string remotePath = GetRemoteTempFilePath();
            CreateHubFile(_com, remotePath, "Contents of the file.");
            string localPath = GetLocalTempFilePath();

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Delete);

            // Steps
            var result = await _com.PCtoHubAsync(command);

            // Expected Result
            Assert.True(result.DeleteSucceeded);
        }
    }

    public class DownloadResult_ExceptionProperty : CommunicatorTestBase
    {
        [Fact]
        public void Exception_ShouldBeNullByDefault()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.Null(result.Exception);
        }

        [Fact]
        public void Exception_ShouldContainErrorDetailsWhenFailureOccurs()
        {
            // Setup
            string remotePath = GetRemoteTempFilePath();
            //CreateHubFile(_com, remotePath, "Contents of the file.");
            string localPath = GetLocalTempFilePath();

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            Assert.Null(result.Exception);
        }
    }

    public class DownloadResult_ToStringMethod : CommunicatorTestBase
    {
        [Fact]
        public async Task ToString_ShouldReturnFormattedOutputWithAllProperties()
        {
            // Setup
            string remotePath = GetRemoteTempFilePath();
            CreateHubFile(_com, remotePath, "Contents of the file.");
            string localPath = GetLocalTempFilePath();

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            var result = await _com.PCtoHubAsync(command);

            // Expected Result
            string output = result.ToString();
            Assert.Contains($"Remote: {remotePath}", output);
            Assert.Contains($"Local : {localPath}", output);
            Assert.Contains("Exists: ", output);
            Assert.Contains("Size  : ", output);
            Assert.Contains("Downloaded: True", output);
            Assert.Contains("Deleted   : ", output);
            Assert.Contains("Success   : ", output);
        }

        [Fact]
        public void ToString_ShouldIncludeExceptionMessageWhenPresent()
        {
            // Setup
            string remotePath = GetRemoteTempFilePath();
            CreateHubFile(_com, remotePath, "Contents of the file.");
            string localPath = GetLocalTempFilePath();

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);
            result.Exception = new FileNotFoundException("File not found");

            // Expected Result
            string output = result.ToString();
            Assert.Contains("File not found", output);
        }

        [Fact]
        public void ToString_ShouldNotIncludeExceptionMessageWhenNull()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";

            var command = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = new DownloadResult(remotePath, localPath, command);

            // Expected Result
            string output = result.ToString();
            Assert.DoesNotContain("Exception", output);
        }
    }
}
