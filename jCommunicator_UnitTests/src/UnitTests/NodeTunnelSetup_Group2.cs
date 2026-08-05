using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class NodeTunnelSetup_Group1_AddNodeTunnelAsync_ReturnsPort
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithValidCredentials_ShouldReturnPortGreaterThanZero()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            int port = await com.AddNodeTunnelAsync("10.0.0.11", "camcpp", "cam");

            // Expected Result
            Assert.True(port > 0);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_ReturnsInt()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            int port = await com.AddNodeTunnelAsync("10.0.0.11", "camcpp", "cam");

            // Expected Result
            Assert.IsType<int>(port);
        }
    }

    public class NodeTunnelSetup_Group2_AddNodeTunnelAsync_WithNullHost
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithNullHost_ShouldThrowArgumentNullException()

        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await com.AddNodeTunnelAsync(null, "user", "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithEmptyHost_ShouldThrowArgumentNullException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await com.AddNodeTunnelAsync("", "user", "pass"));
        }
    }

    public class NodeTunnelSetup_Group3_AddNodeTunnelAsync_WithNullUsername
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithNullUsername_ShouldThrowArgumentNullException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await com.AddNodeTunnelAsync("10.0.0.11", null, "pass"));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithEmptyUsername_ShouldThrowArgumentNullException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await com.AddNodeTunnelAsync("10.0.0.11", "", "pass"));
        }
    }

    public class NodeTunnelSetup_Group4_AddNodeTunnelAsync_WithNullPassword
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WithNullPassword_ShouldThrowArgumentNullException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await com.AddNodeTunnelAsync("10.0.0.11", "user", null));
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WithEmptyPassword_ShouldThrowArgumentNullException()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await com.AddNodeTunnelAsync("10.0.0.11", "user", ""));
        }
    }

    public class NodeTunnelSetup_Group5_AddNodeTunnelAsync_WithUnreachableHub
    {
        [Fact]
        public async Task AddNodeTunnelAsync_WhenHubUnreachable_ShouldReturnZero()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            int port = await com.AddNodeTunnelAsync("10.0.0.11", "user", "pass");

            // Expected Result
            Assert.Equal(0, port);
        }

        [Fact]
        public async Task AddNodeTunnelAsync_WhenHubUnreachable_NoExceptionThrown()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            await com.AddNodeTunnelAsync("10.0.0.11", "user", "pass");
        }
    }

    public class NodeTunnelSetup_Group6_PingNodeAsync_ReturnsBool
    {
        [Fact]
        public void PingNodeAsync_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.PingNodeAsync("10.0.0.11");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task PingNodeAsync_WithUnreachableHub_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            bool result = await com.PingNodeAsync("10.0.0.11");

            // Expected Result
            Assert.False(result);
        }
    }

    public class NodeTunnelSetup_Group7_PingNodeAsync_WithInvalidCredentials
    {
        [Fact]
        public async Task PingNodeAsync_WithInvalidCredentials_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            bool result = await com.PingNodeAsync("192.0.2.1");

            // Expected Result
            Assert.False(result);
        }
    }

    public class NodeTunnelSetup_Group8_NodeFileExists_ReturnsBool
    {
        [Fact]
        public void NodeFileExists_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.NodeFileExists("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task NodeFileExists_WithUnreachableHub_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            bool result = await com.NodeFileExists("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.False(result);
        }
    }

    public class NodeTunnelSetup_Group9_NodeFileLastModified_ReturnsDateTimeNullable
    {
        [Fact]
        public void NodeFileLastModified_ReturnsDateTimeNullable()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.NodeFileLastModified("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.IsType<DateTime?>(result);
        }

        [Fact]
        public async Task NodeFileLastModified_WithUnreachableHub_ReturnsNull()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            DateTime? result = await com.NodeFileLastModified("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.Null(result);
        }
    }

    public class NodeTunnelSetup_Group10_GetListOfNodeFiles_ReturnsList
    {
        [Fact]
        public void GetListOfNodeFiles_ReturnsList()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.GetListOfNodeFiles("/tmp/", ".txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<Renci.SshNet.Sftp.SftpFile>>(result);
        }

        [Fact]
        public void GetListOfNodeFiles_WithUnreachableHub_ReturnsEmptyList()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            var result = com.GetListOfNodeFiles("/tmp/", ".txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List< Renci.SshNet.Sftp.SftpFile>> (result);
        }
    }

    public class NodeTunnelSetup_Group11_DeleteNodeFile_ReturnsBool
    {
        [Fact]
        public void DeleteNodeFile_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.DeleteNodeFile("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task DeleteNodeFile_WithUnreachableHub_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            bool result = await com.DeleteNodeFile("/tmp/test.txt", "10.0.0.11");

            // Expected Result
            Assert.False(result);
        }
    }

    public class NodeTunnelSetup_Group12_MoveNodeFile_ReturnsBool
    {
        [Fact]
        public void MoveNodeFile_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.MoveNodeFile("/tmp/test.txt", "/tmp/moved.txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task MoveNodeFile_WithUnreachableHub_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            bool result = await com.MoveNodeFile("/tmp/test.txt", "/tmp/moved.txt", "10.0.0.11", "camcpp");

            // Expected Result
            Assert.False(result);
        }
    }
}
