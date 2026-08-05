using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class PCtoHubAsync_Group1_ReturnsDownloadResult
    {
        [Fact]
        public void PCtoHubAsync_ReturnsDownloadResult()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoHubAsync(new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.IsType<DownloadResult>(result);
        }

        [Fact]
        public void PCtoHubAsync_WithVerboseFalse_ReturnsDownloadResult()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoHubAsync(new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.IsType<DownloadResult>(result);
        }
    }

    public class PCtoHubAsync_Group2_DownloadResult_CommandMatchesInput
    {
        [Fact]
        public async Task PCtoHubAsync_CommandReflectsInput()
        {
            // Setup
            ClusterFileIOCommand inputCmd = new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PCtoHubAsync(inputCmd, false);

            // Expected Result
            Assert.Equal(inputCmd, result.Command);
        }
    }

    public class PCtoHubAsync_Group3_DownloadResult_FileExistsReflectsPresence
    {
        [Fact]
        public async Task PCtoHubAsync_FileExistsReflectsRemotePresence()
        {
            // Setup
            ClusterFileIOCommand inputCmd = new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PCtoHubAsync(inputCmd, false);

            // Expected Result
            Assert.Equal(result.FileExists, result.Command.checkExists);
        }
    }

    public class PCtoHubAsync_Group4_DownloadResult_MainProcedureSucceededReflectsTransfer
    {
        [Fact]
        public async Task PCtoHubAsync_MainProcedureSucceededReflectsTransfer()
        {
            // Setup
            ClusterFileIOCommand inputCmd = new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PCtoHubAsync(inputCmd, false);

            // Expected Result
            Assert.True(result.MainProcedureSucceeded);
        }
    }

    public class PCtoHubAsync_Group5_DownloadResult_SuccessCombinesAllChecks
    {
        [Fact]
        public async Task PCtoHubAsync_SuccessReflectsOverallOutcome()
        {
            // Setup
            ClusterFileIOCommand inputCmd = new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PCtoHubAsync(inputCmd, false);

            // Expected Result
            Assert.True(result.Success);
        }
    }

    public class PCtoHubAsync_Group6_DownloadResult_CommandReflectsInputCommand()
    {
        [Fact]
        public async Task PCtoHubAsync_CommandReflectsInputCommand()
        {
            // Setup
            ClusterFileIOCommand inputCmd = new ClusterFileIOCommand("/tmp/test.txt", "/local/path/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PCtoHubAsync(inputCmd, false);

            // Expected Result
            Assert.Equal(inputCmd.Type, result.Command.Type);
        }
    }

    public class PCtoNodesAsync_Group1_ReturnsList
    {
        [Fact]
        public void PCtoNodesAsync_ReturnsList()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PCtoNodeAsync(new List<ClusterFileIOCommand>(), "10.0.0.11");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<DownloadResult>>(result);
        }

        [Fact]
        public async Task PCtoNodesAsync_WithEmptyList_ReturnsEmptyList()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = await com.PCtoNodeAsync(new List<ClusterFileIOCommand>(), "10.0.0.11");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<DownloadResult>>(result);
        }
    }

    public class PCtoNodesAsync_Group2_ResultsCountEqualsCommandsCount()
    {
        [Fact]
        public async Task PCtoNodesAsync_ResultsCountMatchesInputCount()
        {
            // Setup
            ClusterFileIOCommand cmd1 = new ClusterFileIOCommand("/tmp/test1.txt", "/local/path1/", ClusterFileIOCommandType.Download);
            ClusterFileIOCommand cmd2 = new ClusterFileIOCommand("/tmp/test2.txt", "/local/path2/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var commands = new List<ClusterFileIOCommand> { cmd1, cmd2 };
            var results = await com.PCtoNodeAsync(commands, "10.0.0.11");

            // Expected Result
            Assert.Equal(commands.Count, results.Count);
        }
    }

    public class PCtoNodesAsync_Group3_EachResultPopulatedCorrectly()
    {
        [Fact]
        public async Task PCtoNodesAsync_EachResultHasCommand()
        {
            // Setup
            ClusterFileIOCommand cmd1 = new ClusterFileIOCommand("/tmp/test1.txt", "/local/path1/", ClusterFileIOCommandType.Download);
            ClusterFileIOCommand cmd2 = new ClusterFileIOCommand("/tmp/test2.txt", "/local/path2/", ClusterFileIOCommandType.Download);
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var commands = new List<ClusterFileIOCommand> { cmd1, cmd2 };
            var results = await com.PCtoNodeAsync(commands, "10.0.0.11");

            // Expected Result
            Assert.Equal(cmd1, results[0].Command);
            Assert.Equal(cmd2, results[1].Command);
        }
    }
}
