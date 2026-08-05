using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class HubFileOperations_NormalOperations : CommunicatorTestBase
    {
        [Fact]
        public async Task HubFileExists_ReturnsBool()
        {
            // Steps
            var result = await _com.HubFileExists("/tmp/test.txt");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task HubFileLastModified_ReturnsDateTime()
        {
            // Steps
            var result = await _com.HubFileLastModified("/tmp/test.txt");

            // Expected Result
            Assert.IsType<DateTime>(result);
        }

        [Fact]
        public async Task HubFileLastModified_WithUnreachableHub_ReturnsMinValue()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            DateTime result = await com.HubFileLastModified("/tmp/test.txt");

            // Expected Result
            Assert.Equal(DateTime.MinValue, result);
        }
    }

    public class HubFileOperations_Group3_GetListOfHubFiles_ReturnsList
    {
        [Fact]
        public void GetListOfHubFiles_ReturnsList()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.GetListOfHubFiles("/tmp/", ".txt");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<Renci.SshNet.Sftp.SftpFile>>(result);
        }

        [Fact]
        public async Task GetListOfHubFiles_WithUnreachableHub_ReturnsEmptyList()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            var result = await com.GetListOfHubFiles("/tmp/", ".txt");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<Renci.SshNet.Sftp.SftpFile>>(result);
        }
    }

    public class HubFileOperations_Group4_DeleteHubFile_ReturnsBool
    {
        [Fact]
        public void DeleteHubFile_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.DeleteHubFile("/tmp/test.txt");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task DeleteHubFile_WithUnreachableHub_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            bool result = await com.DeleteHubFile("/tmp/test.txt");

            // Expected Result
            Assert.False(result);
        }
    }

    public class HubFileOperations_Group5_MoveHubFile_ReturnsBool
    {
        [Fact]
        public void MoveHubFile_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            var result = com.MoveHubFile("/tmp/test.txt", "/tmp/moved.txt");

            // Expected Result
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task MoveHubFile_WithUnreachableHub_ReturnsFalse()
        {
            // Setup
            Communicator com = new Communicator("192.0.2.1", "nonexistent", "wrongpass");

            // Steps
            bool result = await com.MoveHubFile("/tmp/test.txt", "/tmp/moved.txt");

            // Expected Result
            Assert.False(result);
        }
    }
}
