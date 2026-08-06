using jCommunicator;
using jCommunicator.Tests;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class ClusterTest_EndToEnd : CommunicatorTestBase
    {
        [Fact]
        public async Task End_To_End()
        {
            // Setup
            await _com.AddNodeTunnelAsync(node1Host, node1User, node1Pass);

            string originalText = Guid.NewGuid().ToString();

            string pcSource = GetLocalTempFilePath();
            await File.WriteAllTextAsync(pcSource, originalText);

            string hubFile = GetRemoteTempFilePath();
            string nodeFromHubFile = GetRemoteTempFilePath();
            string nodeFromPCFile = GetRemoteTempFilePath();

            string pcHubCopyBack = GetLocalTempFilePath();
            string pcNodeCopyBack = GetLocalTempFilePath();

            //
            // PC -> Hub
            //
            var pcToHub = await _com.PCtoHubAsync(new ClusterFileIOCommand(hubFile, pcSource, ClusterFileIOCommandType.Upload));

            Assert.True(pcToHub.MainProcedureSucceeded);
            Assert.True(await _com.HubFileExists(hubFile));

            //
            // Hub -> Node
            //
            Assert.True(await _com.CopyHubToNode(hubFile, nodeFromHubFile, node1Host, node1User));
            Assert.True(await _com.NodeFileExists(nodeFromHubFile, node1Host));

            //
            // Hub -> PC
            //
            var hubToPC = await _com.PCtoHubAsync(new ClusterFileIOCommand(hubFile, pcHubCopyBack, ClusterFileIOCommandType.Download));

            Assert.True(hubToPC.MainProcedureSucceeded);
            Assert.True(File.Exists(pcHubCopyBack));
            Assert.Equal(originalText, await File.ReadAllTextAsync(pcHubCopyBack));

            //
            // PC -> Node
            //
            var pcToNode = await _com.PCtoNodeAsync(
                new ClusterFileIOCommand(nodeFromPCFile, pcSource, ClusterFileIOCommandType.Upload),
                node1Host);

            Assert.True(pcToNode.MainProcedureSucceeded);
            Assert.True(await _com.NodeFileExists(nodeFromPCFile, node1Host));

            //
            // Node -> PC
            //
            var nodeToPC = await _com.PCtoNodeAsync(
                new ClusterFileIOCommand(nodeFromPCFile, pcNodeCopyBack, ClusterFileIOCommandType.Download),
                node1Host);

            Assert.True(nodeToPC.MainProcedureSucceeded);
            Assert.True(File.Exists(pcNodeCopyBack));
            Assert.Equal(originalText, await File.ReadAllTextAsync(pcNodeCopyBack));
        }
    }
}