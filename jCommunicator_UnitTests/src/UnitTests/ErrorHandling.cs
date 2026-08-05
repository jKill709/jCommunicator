using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ErrorHandling_WithBadConstructorArgs : CommunicatorTestBase
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

    public class ErrorHandling_WhenHubUnreachable : CommunicatorTestBase
    {
        [Fact]
        public async Task AddNodeTunnelAsync_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));
        }

        [Fact]
        public async Task ExecuteHubCommandAsync_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.ExecuteHubCommandAsync("echo test"));
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.ExecuteNodeCommandAsync("echo test", "10.0.0.11", "user"));
        }

        [Fact]
        public async Task NodeFileExists_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.NodeFileExists("/path/to/file.txt", "10.0.0.11"));
        }

        [Fact]
        public async Task NodeFileLastModified_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.NodeFileLastModified("/path/to/file.txt", "10.0.0.11"));
        }

        [Fact]
        public async Task GetListOfNodeFiles_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.GetListOfNodeFiles("/path/to/", ".txt.", "192.0.2.1", "nonexistent"));
        }

        [Fact]
        public async Task DeleteNodeFile_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.DeleteNodeFile("/path/to/file.txt", "10.0.0.11"));
        }

        [Fact]
        public async Task MoveNodeFile_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.MoveNodeFile("/path/to/file.txt", "/path/to/newname.txt", "192.0.2.1", "nonexistent"));
        }

        [Fact]
        public async Task PingNodeAsync_ShouldThrowSocketException()
        {
            // Setup
            Communicator badCom = new Communicator("192.0.2.1", "nonexistent", "wrongpass");
            await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(() => badCom.AddNodeTunnelAsync("10.0.0.11", "user", "pass"));

            // Steps
            Assert.False(await  badCom.PingNodeAsync("10.0.0.11"));
        }
    }

    public class ErrorHandling_NonexistantHubPaths : CommunicatorTestBase
    {
        [Fact]
        public async Task PCtoHubAsync_WithInvalidPath_ShouldThrowException()
        {
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand("/invalid/path/that/does/not/exist/file.txt", "/local/output/", ClusterFileIOCommandType.Download), false);

            Assert.IsType<UnauthorizedAccessException>(result.Exception);
        }

        [Fact]
        public async Task ExecuteHubCommandAsync_WithInvalidPath_ShouldReturnEmptyString()
        {
            // Steps
            var result = await _com.ExecuteHubCommandAsync("ls /invalid/path/that/does/not/exist");
            Assert.Equal("", result);
        }

        [Fact]
        public async Task PCtoHubAsync_WithInvalidRemotePath_ReturnsSuccessFalse()
        {
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand("/tmp/invalid/path/that/does/not/exist/file.txt", "C:\\tmp\\output\\file.txt", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.False(result.MainProcedureSucceeded);
        }

        [Fact]
        public async Task PCtoHubAsync_WithInvalidRemotePath_ReturnsNonNullException()
        {
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand("/tmp/invalid/path/that/does/not/exist/file.txt", "C:\\tmp\\output\\file.txt", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.NotNull(result.Exception);
        }

        [Fact]
        public async Task PCtoHubAsync_WithInvalidRemotePath_ExceptionMessageContainsContext()
        {
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand("/tmp/invalid/path/that/does/not/exist/file.txt", "C:\\tmp\\output\\file.txt", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.NotNull(result.Exception);
            Assert.True(result.Exception.Message.Contains("No such file"));
        }
    }

    public class ErrorHandling_AddNodeTunnelWithBadCredentials : CommunicatorTestBase
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithInvalidCredentials_ReturnsPortZero()
        {
            // Steps
            int port = await _com.AddNodeTunnelAsync("192.0.2.1", "nonexistent_user", "wrong_password");

            // Expected Result
            Assert.Equal(0, port);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithInvalidHost_ReturnsPortZero()
        {
            // Steps
            int port = await _com.AddNodeTunnelAsync("192.0.2.1", "user", "pass");

            // Expected Result
            Assert.Equal(0, port);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithInvalidCredentials_NoExceptionThrown()
        {
            // Setup
            await _com.AddNodeTunnelAsync("192.0.2.1", "nonexistent_user", "wrong_password");
        }
    }

    public class Communicator_LoggingContract : CommunicatorTestBase
    {
        [Fact]
        public async Task ExecuteHubCommandAsync_UsesLoggerInstance()
        {
            // Steps
            await _com.ExecuteHubCommandAsync("echo test 2>&1", true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains("test"));
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_UsesLoggerInstance()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.ExecuteNodeCommandAsync("echo test", node1Host, node1User, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains("test"));
        }

        [Fact]
        public async Task PingNodeAsync_UsesLoggerInstance()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.PingNodeAsync(node1Host, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains("ping"));
        }

        [Fact]
        public async Task NodeFileLastModified_UsesLoggerInstance()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string remoteFile = await CreateNodeFile(_com, node1Host, node1User, GetRemoteTempFilePath());

            // Steps
            await _com.NodeFileLastModified(remoteFile, node1Host, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains(remoteFile));
        }

        [Fact]
        public async Task GetListOfNodeFiles_UsesLoggerInstance()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string tempDir = "/tmp/";

            // Steps
            await _com.GetListOfNodeFiles(tempDir, ".txt", node1Host, node1User, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains(tempDir));
        }

        [Fact]
        public async Task DeleteNodeFile_UsesLoggerInstance()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string remoteFile = await CreateNodeFile(_com, node1Host, node1User, GetRemoteTempFilePath());

            // Steps
            var result = await _com.DeleteNodeFile(remoteFile, node1Host, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains(remoteFile));
        }

        [Fact]
        public async Task MoveNodeFile_UsesLoggerInstance()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string remoteFile = await CreateNodeFile(_com, node1Host, node1User, GetRemoteTempFilePath());
            string localPath = GetRemoteTempFilePath();

            // Steps
            var result = await _com.MoveNodeFile(remoteFile, localPath, node1Host, node1User, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains(remoteFile));
            Assert.Contains(_logSink.Logs, log => log.Contains(localPath));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_UsesLoggerInstance()
        {
            // Setup_
            int result = await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass, true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains(result.ToString()));
        }

        [Fact]
        public async Task PCtoHubAsync_UsesLoggerInstance()
        {
            string remotePath = await CreateHubFile(_com, GetRemoteTempFilePath());
            string localPath = GetLocalTempFilePath();
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download), true);

            // Expected Result
            Assert.Contains(_logSink.Logs, log => log.Contains(remotePath));
            Assert.Contains(_logSink.Logs, log => log.Contains(localPath));
        }
    }
}
