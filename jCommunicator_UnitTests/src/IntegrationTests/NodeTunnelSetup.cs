using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class AddNodeTunnelAsync_NormalOperation : CommunicatorTestBase
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithValidCredentials_ShouldReturnPortGreaterThanZero()
        {
            // Steps
            int port = await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Expected Result
            Assert.True(port > 0);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_ReturnsInt()
        {
            // Steps
            int port = await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Expected Result
            Assert.IsType<int>(port);
        }

        [Fact]
        public async Task PingNodeAsync_ReturnsBool()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.PingNodeAsync(node1Host);

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task NodeFileExists_ReturnsBool()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.NodeFileExists("/tmp/test.txt", node1Host);

            // Expected Result
            Assert.IsType<bool>(result);
        }
        [Fact]
        public async Task NodeFileLastModified_ReturnsDateTimeNullable()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string remotePath = await CreateNodeFile(_com, node1Host, node1User, GetRemoteTempFilePath());

            // Steps
            var result = await _com.NodeFileLastModified(remotePath, node1Host);

            // Expected Result
            Assert.IsType<System.DateTime>(result);
        }

        [Fact]
        public async Task GetListOfNodeFiles_ReturnsList()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.GetListOfNodeFiles("/tmp/", ".txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<LinuxFileInfo>>(result);
        }

        [Fact]
        public async Task DeleteNodeFile_ReturnsBool()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string remotePath = await CreateNodeFile(_com, node1Host, node1User, GetRemoteTempFilePath());

            // Steps
            var result = await _com.DeleteNodeFile(remotePath, node1Host);

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task MoveNodeFile_ReturnsBool()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            string remotePath = await CreateNodeFile(_com, node1Host, node1User, GetRemoteTempFilePath());
            string remoteDestinationPath = GetRemoteTempFilePath();

            // Steps
            var result = await _com.MoveNodeFile(remotePath, remoteDestinationPath, node1Host, node1User);

            // Expected Result
            Assert.IsType<bool>(result);
        }
    }

    public class AddNodeTunnelAsync_WithBadCredentials : CommunicatorTestBase
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithNullHost_ShouldThrowArgumentNullException()
        {
            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _com.AddNodeTunnelAsync(null, "user", "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithEmptyHost_ShouldThrowArgumentNullException()
        {
            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _com.AddNodeTunnelAsync("", "user", "pass"));
        }
    

        [Fact]
        public async Task AddNodeTunnelAsync_WithNullUsername_ShouldThrowArgumentNullException()
        {
            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _com.AddNodeTunnelAsync("10.0.0.11", null, "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithEmptyUsername_ShouldThrowArgumentNullException()
        {
            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _com.AddNodeTunnelAsync("10.0.0.11", "", "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithNullPassword_ShouldThrowArgumentNullException()
        {
            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _com.AddNodeTunnelAsync("10.0.0.11", "user", null));
        }
    }
}
