using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class PCtoHubAsync_NormalOperation : CommunicatorTestBase
    {
        [Fact]
        public async Task PCtoHubAsync_ReturnsDownloadResult()
        {
            string remoteFile = await CreateHubFile(_com, GetRemoteTempFilePath());
            string localFile = GetRemoteTempFilePath();
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand(remoteFile, localFile, ClusterFileIOCommandType.Download), false);

            // Expected Result
            Assert.IsType<DownloadResult>(result);
        }

        [Fact]
        public async Task PCtoHubAsync_WithVerboseTrue_ReturnsDownloadResult()
        {
            string remoteFile = await CreateHubFile(_com, GetRemoteTempFilePath());
            string localFile = GetRemoteTempFilePath();
            // Steps
            var result = await _com.PCtoHubAsync(new ClusterFileIOCommand(remoteFile, localFile, ClusterFileIOCommandType.Download), true);

            // Expected Result
            Assert.IsType<DownloadResult>(result);
        }

        [Fact]
        public async Task PCtoHubAsync_CommandReflectsInput()
        {
            // Setup
            string remotePath = await CreateHubFile(_com, GetRemoteTempFilePath());
            string localPath = GetLocalTempFilePath();
            ClusterFileIOCommand inputCmd = new ClusterFileIOCommand(remotePath, localPath, ClusterFileIOCommandType.Download);

            // Steps
            var result = await _com.PCtoHubAsync(inputCmd, false);

            // Expected Result
            Assert.Equal(inputCmd, result.Command);
        }
    }

    public class PCtoNodesAsync_NormalOperation : CommunicatorTestBase
    {
        [Fact]
        public async Task PCtoNodesAsync_ReturnsList()
        {
            // Steps
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            var result = await _com.PCtoNodeAsync(new List<ClusterFileIOCommand>(), node1Host);

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<DownloadResult>>(result);
        }

        [Fact]
        public async Task PCtoNodesAsync_WithEmptyList_ReturnsEmptyList()
        {
            // Steps
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);
            var result = await _com.PCtoNodeAsync(new List<ClusterFileIOCommand>(), node1Host);

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<DownloadResult>>(result);
        }

        [Fact]
        public async Task PCtoNodesAsync_ResultsCountMatchesInputCount()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            string remoteFile1 = await CreateNodeFile(_com, node1Host, node1User, node1Pass, GetRemoteTempFilePath());
            string remoteFile2 = await CreateNodeFile(_com, node1Host, node1User, node1Pass, GetRemoteTempFilePath());
            string localFile1 = GetLocalTempFilePath();
            string localFile2 = GetLocalTempFilePath();

            ClusterFileIOCommand cmd1 = new ClusterFileIOCommand(remoteFile1, localFile1, ClusterFileIOCommandType.Download);
            ClusterFileIOCommand cmd2 = new ClusterFileIOCommand(remoteFile2, localFile2, ClusterFileIOCommandType.Download);

            List<ClusterFileIOCommand> commands = new List<ClusterFileIOCommand> { cmd1, cmd2 };

            // Steps
            var results = await _com.PCtoNodeAsync(commands, node1Host);

            // Expected Result
            Assert.Equal(commands.Count, results.Count);
        }

        [Fact]
        public async Task PCtoNodesAsync_EachResultHasCommand()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            string remoteFile1 = await CreateNodeFile(_com, node1Host, node1User, node1Pass, GetRemoteTempFilePath());
            string remoteFile2 = await CreateNodeFile(_com, node1Host, node1User, node1Pass, GetRemoteTempFilePath());
            string localFile1 = GetLocalTempFilePath();
            string localFile2 = GetLocalTempFilePath();

            ClusterFileIOCommand cmd1 = new ClusterFileIOCommand(remoteFile1, localFile1, ClusterFileIOCommandType.Download);
            ClusterFileIOCommand cmd2 = new ClusterFileIOCommand(remoteFile2, localFile2, ClusterFileIOCommandType.Download);

            List<ClusterFileIOCommand> commands = new List<ClusterFileIOCommand> { cmd1, cmd2 };

            // Steps
            var results = await _com.PCtoNodeAsync(commands, node1Host);

            // Expected Result
            Assert.Equal(cmd1, results[0].Command);
            Assert.Equal(cmd2, results[1].Command);
        }
    }
}
