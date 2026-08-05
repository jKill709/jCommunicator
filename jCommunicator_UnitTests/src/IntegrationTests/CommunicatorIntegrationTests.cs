using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class CommunicatorIntegrationTests : CommunicatorTestBase
    {
        public CommunicatorIntegrationTests()
        {
            Assert.NotNull(_com);
        }

        #region Connection

        [Fact]
        public async Task Connect_ShouldConnect()
        {
            bool connected = await _com.ConnectAsync();

            Assert.True(connected);
            Assert.True(_com.IsConnected);

            // TODO: Verify SSH and SFTP sessions are active.
        }

        [Fact]
        public async Task Disconnect_ShouldDisconnect()
        {
            await _com.ConnectAsync();

            await _com.DisconnectAsync();

            Assert.False(_com.IsConnected);

            // TODO: Verify tunnels are disposed.
        }

        [Fact]
        public async Task CheckConnection_ShouldReconnectIfNecessary()
        {
            await _com.CheckConnectionAsync();

            Assert.True(_com.IsConnected);

            // TODO: Verify reconnect logic.
        }

        [Fact]
        public async Task CheckSSHDevice_ShouldReturnSuccess()
        {
            var result = await _com.checkSSHDeviceAsync(false);

            Assert.True(result.Success);

            // TODO: Verify timing and exception values.
        }

        #endregion

        #region Node Tunnels

        [Fact]
        public async Task AddNodeTunnel_ShouldCreateTunnel()
        {
            await _com.ConnectAsync();

            int port = await _com.AddNodeTunnelAsync(
                node1Host,
                node1User,
                node1Pass);

            Assert.True(port > 0);

            // TODO: Verify forwarded port and SFTP connection.
        }

        #endregion

        #region Commands

        [Fact]
        public async Task ExecuteHubCommand_ShouldExecute()
        {
            string output = await _com.ExecuteHubCommandAsync("echo Hello");

            Assert.Equal("Hello", output.Trim());

            // TODO: Add additional command tests.
        }

        [Fact]
        public async Task ExecuteNodeCommand_ShouldExecute()
        {
            string output = await _com.ExecuteNodeCommandAsync(
                "echo Hello",
                node1Host,
                node1User);

            Assert.Equal("Hello", output.Trim());

            // TODO: Verify execution through tunnel.
        }

        [Fact]
        public async Task PingNode_ShouldReturnTrue()
        {
            bool result = await _com.PingNodeAsync(node1Host);

            Assert.True(result);

            // TODO: Test unreachable node.
        }

        #endregion

        #region Hub Files

        [Fact]
        public async Task HubFileExists_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task HubFileLastModified_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task GetListOfHubFiles_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task DeleteHubFile_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task MoveHubFile_ShouldExecute()
        {
            // TODO
        }

        #endregion

        #region Node Files

        [Fact]
        public async Task NodeFileExists_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task NodeFileLastModified_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task GetListOfNodeFiles_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task DeleteNodeFile_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task MoveNodeFile_ShouldExecute()
        {
            // TODO
        }

        #endregion

        #region Copy Operations

        [Fact]
        public async Task CopyHubToNode_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task CopyNodeToHub_ShouldExecute()
        {
            // TODO
        }

        #endregion

        #region SFTP Transfers

        [Fact]
        public async Task PCtoHubAsync_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task PCtoHubBatchAsync_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task PCtoNodeAsync_ShouldExecute()
        {
            // TODO
        }

        [Fact]
        public async Task PCtoNodeBatchAsync_ShouldExecute()
        {
            // TODO
        }

        #endregion
    }
}