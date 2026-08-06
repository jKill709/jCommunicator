using jCommunicator;
using jCommunicator.Tests;
using Xunit;

namespace jCommunicator.Tests.Integration
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
            // Setup
            string remotePath = await CreateHubFile(_com, GetRemoteTempFilePath());

            // Steps
            var result = await _com.HubFileLastModified(remotePath);

            // Expected Result
            Assert.IsType<DateTime>(result);
        }

        [Fact]
        public async Task GetListOfHubFiles_ReturnsList()
        {
            // Steps
            var result = await _com.GetListOfHubFiles("/tmp/", ".txt");

            // Expected Result
            Assert.IsType<System.Collections.Generic.List<LinuxFileInfo>>(result);
        }
    
        [Fact]
        public async Task DeleteHubFile_ReturnsBool()
        {
            // Setup
            string remotePath = await CreateHubFile(_com, GetRemoteTempFilePath());

            // Steps
            var result = await _com.DeleteHubFile(remotePath);

            // Expected Result
            Assert.IsType<bool>(result);
        }
    
        [Fact]
        public async Task MoveHubFile_ReturnsBool()
        {
            // Setup
            string remotePath = await CreateHubFile(_com, GetRemoteTempFilePath());
            string destinationPath = GetRemoteTempFilePath();

            // Steps
            var result = await _com.MoveHubFile(remotePath, destinationPath);

            // Expected Result
            Assert.IsType<bool>(result);
        }
    }
}
