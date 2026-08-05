using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ExecuteHubCommandAsync_Group1_ReturnsString()
    {
        [Fact]
        public void ExecuteHubCommandAsync_ReturnsString()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.ExecuteHubCommandAsync("echo test");

            // Expected Result
            Assert.IsType<string>(result);
        }

        [Fact]
        public void ExecuteHubCommandAsync_WithVerboseFalse_ReturnsString()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.ExecuteHubCommandAsync("echo test", false);

            // Expected Result
            Assert.IsType<string>(result);
        }
    }

    public class ExecuteHubCommandAsync_Group2_ReturnsOutputFromCommand()
    {
        [Fact]
        public async Task ExecuteHubCommandAsync_ReturnsCommandOutput()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            string result = await com.ExecuteHubCommandAsync("echo 'test'");

            // Expected Result
            Assert.Contains("test", result);
        }
    }

    public class ExecuteHubCommandAsync_Group3_CommandExecutesSuccessfully()
    {
        [Fact]
        public async Task ExecuteHubCommandAsync_CommandExecutesSuccessfully()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            string result = await com.ExecuteHubCommandAsync("ls /tmp/");

            // Expected Result
            Assert.NotEmpty(result);
        }
    }

    public class ExecuteNodeCommandAsync_Group1_ReturnsString()
    {
        [Fact]
        public void ExecuteNodeCommandAsync_ReturnsString()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.ExecuteNodeCommandAsync("echo test", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<string>(result);
        }

        [Fact]
        public void ExecuteNodeCommandAsync_WithVerboseFalse_ReturnsString()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.ExecuteNodeCommandAsync("echo test", "10.0.0.11", "camcpp", false);

            // Expected Result
            Assert.IsType<string>(result);
        }
    }

    public class ExecuteNodeCommandAsync_Group2_ReturnsOutputFromCommand()
    {
        [Fact]
        public async Task ExecuteNodeCommandAsync_ReturnsCommandOutput()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            string result = await com.ExecuteNodeCommandAsync("echo 'test'", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.Contains("test", result);
        }
    }

    public class ExecuteNodeCommandAsync_Group3_CommandExecutesSuccessfully()
    {
        [Fact]
        public async Task ExecuteNodeCommandAsync_CommandExecutesSuccessfully()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            string result = await com.ExecuteNodeCommandAsync("ls /tmp/", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.NotEmpty(result);
        }
    }

    public class ExecuteNodeCommandAsync_Group4_RequiresActiveTunnel()
    {
        [Fact]
        public async Task ExecuteNodeCommandAsync_WithoutTunnel_ShouldThrowException()
        {
            // Setup - Create communicator without connecting to hub
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await com.ExecuteNodeCommandAsync("echo test", "10.0.0.11", "camcpp"));
        }
    }
}
