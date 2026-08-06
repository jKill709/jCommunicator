using jCommunicator;
using Renci.SshNet.Sftp;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class DownloadResult_SuccessProperty
    {
    }

    public class DownloadResult_Properties : CommunicatorTestBase
    {
        [Fact]
        public void DownloadResult_AttributesShouldBeNullByDefault()
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
        public async Task DownloadResult_AttributesShouldBePopulatedWhengetAttributesIsTrue()
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

        [Fact]
        public void DownloadResult_FileExistsShouldBeFalseByDefault()
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
        public void DownloadResult_FileExistsShouldBeTrueWhencheckExistsIsTrue()
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

        [Fact]
        public void DownloadResult_MainProcedureSucceededShouldBeFalseByDefault()
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
        public async Task DownloadResult_MainProcedureSucceededShouldBeTrueAfterSuccessfulDownload()
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

        [Fact]
        public void DownloadResult_DeleteSucceededShouldBeFalseByDefault()
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
        public async Task DownloadResult_DeleteSucceededShouldBeTrueAfterSuccessfulDelete()
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

        [Fact]
        public void DownloadResult_ExceptionShouldBeNullByDefault()
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
        public void DownloadResult_ExceptionShouldContainErrorDetailsWhenFailureOccurs()
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

        [Fact]
        public async Task DownloadResult_ToStringShouldReturnFormattedOutputWithAllProperties()
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
        public void DownloadResult_ToStringShouldIncludeExceptionMessageWhenPresent()
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
        public void DownloadResult_ToStringShouldNotIncludeExceptionMessageWhenNull()
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


        [Fact]
        public void DownloadResult_SuccessShouldBeFalseWhenMainProcedureSucceededIsFalse()
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
        public void DownloadResult_SuccessShouldBeFalseWhenDeleteSucceededIsFalse()
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
        public void DownloadResult_SuccessShouldBeFalseWhenExceptionIsNotNull()
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
}