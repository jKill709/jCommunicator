using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.UnitTests
{
    public class ClusterFileIOCommandConstructors_Group5
    {
        [Fact]
        public void InvalidType_ThrowsArgumentException()
        {
            // Setup
            string remotePath = "/remote/path/file.txt";
            string localPath = "/local/path/file.txt";
            int invalidValue = -1;

            // Steps
            Assert.Throws<ArgumentOutOfRangeException>(() => new ClusterFileIOCommand(remotePath, localPath, (ClusterFileIOCommandType)invalidValue, false, false, false, false));
        }

        [Fact]
        public void NullRemotePath_ThrowsArgumentNullException()
        {
            // Setup
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            Assert.Throws<System.ArgumentNullException>(() => new ClusterFileIOCommand(null, localPath, type, false, false, false, false));
        }

        [Fact]
        public void EmptyRemotePath_ThrowsArgumentException()
        {
            // Setup
            string localPath = "/local/path/file.txt";
            ClusterFileIOCommandType type = ClusterFileIOCommandType.Download;

            // Steps
            Assert.Throws<ArgumentOutOfRangeException>(() => new ClusterFileIOCommand("", localPath, type, false, false, false, false));
        }
    }
}