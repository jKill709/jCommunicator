using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ErrorHandling_Group1_ArgumentNullException : CommunicatorTestBase
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithNullHost_ShouldThrowArgumentNullException()
        {
            // Setup

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(() => _com.AddNodeTunnelAsync(null, "user", "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithEmptyHost_ShouldThrowArgumentNullException()
        {
            // Setup

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(() => _com.AddNodeTunnelAsync("", "user", "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithNullUsername_ShouldThrowArgumentNullException()
        {
            // Setup

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(() => _com.AddNodeTunnelAsync("10.0.0.11", null, "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithNullPassword_ShouldThrowArgumentNullException()
        {
            // Setup

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(() => _com.AddNodeTunnelAsync("10.0.0.11", "user", null));
        }
    }

    public class ErrorHandling_Group2_InvalidOperationException : CommunicatorTestBase
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.ConnectAsync());

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));
        }

        [Fact]
        public async Task ExecuteHubCommandAsync_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.ExecuteHubCommandAsync("echo test"));
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.ExecuteNodeCommandAsync("echo test", "10.0.0.11", "user"));
        }

        [Fact]
        public async Task NodeFileExists_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.NodeFileExists("/path/to/file.txt", "10.0.0.11"));
        }

        [Fact]
        public async Task NodeFileLastModified_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.NodeFileLastModified("/path/to/file.txt", "10.0.0.11"));
        }

        [Fact]
        public async Task GetListOfNodeFiles_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.GetListOfNodeFiles("/path/to/", ".txt.", "192.0.2.1", "nonexistent"));
        }

        [Fact]
        public async Task DeleteNodeFile_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.DeleteNodeFile("/path/to/file.txt", "10.0.0.11"));
        }

        [Fact]
        public async Task MoveNodeFile_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.MoveNodeFile("/path/to/file.txt", "/path/to/newname.txt", "192.0.2.1", "nonexistent"));
        }

        [Fact]
        public async Task PingNodeAsync_WhenHubUnreachable_ShouldThrowInvalidOperationException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<InvalidOperationException>(() => badCom.PingNodeAsync("10.0.0.11"));
        }
    }

    public class ErrorHandling_Group3_FileNotFoundException
    {
        [Fact]
        public async Task PCtoHubAsync_WithLocalFileMissing_ShouldThrowFileNotFoundException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                com.PCtoHubAsync(new ClusterFileIOCommand("nonexistent.txt", "/local/path/", ClusterFileIOCommandType.Upload), false));
        }

        [Fact]
        public async Task PCtoHubAsync_WithRemoteFileMissingCheckExistsTrue_ShouldThrowFileNotFoundException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                com.PCtoHubAsync(new ClusterFileIOCommand("/remote/path/nonexistent.txt", "/local/output/", ClusterFileIOCommandType.Download, checkExists: true), false));
        }
    }

    public class ErrorHandling_Group4_ExceptionWrapping
    {
        [Fact]
        public async Task PCtoHubAsync_WithInvalidPath_ShouldThrowException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<Exception>(() =>
                com.PCtoHubAsync(new ClusterFileIOCommand("/invalid/path/that/does/not/exist/file.txt", "/local/output/", ClusterFileIOCommandType.Download), false));
        }

        [Fact]
        public async Task ExecuteHubCommandAsync_WithInvalidPath_ShouldThrowException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<Exception>(() =>
                com.ExecuteHubCommandAsync("ls /invalid/path/that/does/not/exist"));
        }
    }

    public class ErrorHandling_Group5_ResultBasedFailure
    {
        [Fact]
        public void PCtoHubAsync_WithInvalidRemotePath_ReturnsSuccessFalse()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoHubAsync(new ClusterFileIOCommand("/invalid/path/that/does/not/exist/file.txt", "/local/output/", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.False(result.IsCompletedSuccessfully);
        }

        [Fact]
        public void PCtoHubAsync_WithInvalidRemotePath_ReturnsNonNullException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoHubAsync(new ClusterFileIOCommand("/invalid/path/that/does/not/exist/file.txt", "/local/output/", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.NotNull(result.Exception);
        }

        [Fact]
        public void PCtoHubAsync_WithInvalidRemotePath_ExceptionMessageContainsContext()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoHubAsync(new ClusterFileIOCommand("/invalid/path/that/does/not/exist/file.txt", "/local/output/", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.NotNull(result.Exception);
            Assert.True(result.Exception.Message.Contains("not found") || result.Exception.Message.Contains("Remote file"));
        }
    }

    public class ErrorHandling_Group6_TunnelFailureReturnsZero
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithInvalidCredentials_ReturnsPortZero()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            int port = await com.AddNodeTunnelAsync("192.0.2.1", "nonexistent_user", "wrong_password");

            // Expected Result
            Assert.Equal(0, port);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithInvalidHost_ReturnsPortZero()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            int port = await com.AddNodeTunnelAsync("192.0.2.1", "user", "pass");

            // Expected Result
            Assert.Equal(0, port);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithInvalidCredentials_NoExceptionThrown()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await com.AddNodeTunnelAsync("192.0.2.1", "nonexistent_user", "wrong_password");
        }
    }

    public class ErrorHandling_Group6_LoggingContract
    {
        [Fact]
        public async Task ExecuteHubCommandAsync_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.ExecuteHubCommandAsync("echo test 2>&1");

            // Expected Result
            Assert.Contains("test", result);
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.ExecuteNodeCommandAsync("echo test", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.Contains("test", result);
        }

        [Fact]
        public async Task PingNodeAsync_ReturnsBoolean_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PingNodeAsync("10.0.0.11");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void NodeFileExists_ReturnsBoolean_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.NodeFileExists("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void NodeFileLastModified_ReturnsDateTimeNullable_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.NodeFileLastModified("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.IsType<DateTime?>(result);
        }

        [Fact]
        public void GetListOfNodeFiles_ReturnsList_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.GetListOfNodeFiles("/tmp/", ".txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<List<Renci.SshNet.Sftp.SftpFile>>(result);
        }

        [Fact]
        public void DeleteNodeFile_ReturnsBoolean_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.DeleteNodeFile("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void MoveNodeFile_ReturnsBoolean_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.MoveNodeFile("/tmp/test.txt", "/tmp/moved.txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public void AddNodeTunnelAsync_ReturnsInt_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.AddNodeTunnelAsync("10.0.0.11", "camcpp", "cam");

            // Expected Result
            Assert.IsType<int>(result);
        }

        [Fact]
        public void PCtoHubAsync_ReturnsDownloadResult_UsesLoggerInstance()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoHubAsync(new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.IsType<DownloadResult>(result);
        }
    }
}
