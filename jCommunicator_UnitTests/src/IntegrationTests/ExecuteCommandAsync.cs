using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class ExecuteHubCommandAsync_NormalOperation() : CommunicatorTestBase
    {
        [Fact]
        public async Task ExecuteHubCommandAsync_WithoutVerbose_ReturnsString()
        {
            // Steps
            var result = await _com.ExecuteHubCommandAsync("echo test");

            // Expected Result
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task ExecuteHubCommandAsync_WithVerboseFalse_ReturnsString()
        {
            // Steps
            var result = await _com.ExecuteHubCommandAsync("echo test", false);

            // Expected Result
            Assert.IsType<string>(result);
        }
        [Fact]
        public async Task ExecuteHubCommandAsync_ReturnsCommandOutput()
        {
            // Steps
            string result = await _com.ExecuteHubCommandAsync("echo 'test'");

            // Expected Result
            Assert.Contains("test", result);
        }

        [Fact]
        public async Task ExecuteHubCommandAsync_CommandExecutesSuccessfully()
        {
            // Steps
            string result = await _com.ExecuteHubCommandAsync("ls /tmp/");

            // Expected Result
            Assert.NotEmpty(result);
        }
    }

    public class ExecuteNodeCommandAsync_NormalOperation() : CommunicatorTestBase
    {
        [Fact]
        public async Task ExecuteNodeCommandAsync_ReturnsString()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.ExecuteNodeCommandAsync("echo test", node1Host, node1User);

            // Expected Result
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_WithVerboseTrue_ReturnsString()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            var result = await _com.ExecuteNodeCommandAsync("echo test", node1Host, node1User, true);

            // Expected Result
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_ReturnsCommandOutput()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            string result = await _com.ExecuteNodeCommandAsync("echo test", node1Host, node1User);

            // Expected Result
            Assert.Contains("test", result);
        }

        [Fact]
        public async Task ExecuteNodeCommandAsync_CommandExecutesSuccessfully()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            // Steps
            string result = await _com.ExecuteNodeCommandAsync("ls /tmp/", node1Host, node1User);

            // Expected Result
            Assert.NotEmpty(result);
        }
    }
}
