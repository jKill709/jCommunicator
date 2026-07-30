using System;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class ConnectionTests : CommunicatorTestBase
    {
        [Fact]
        public void Connect_Succeeds()
        {
            // Arrange

            // Act
            _communicator!.Connect();

            // Assert
            Assert.True(_communicator.IsConnected);
        }

        [Fact]
        public void Disconnect_ClearsConnection()
        {
            // Arrange
            _communicator!.Connect();

            // Act
            _communicator.Disconnect();

            // Assert
            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void Connect_Disconnect_Reconnect_Succeeds()
        {
            // Arrange
            _communicator!.Connect();

            // Act
            _communicator.Disconnect();
            _communicator.Connect();

            // Assert
            Assert.True(_communicator.IsConnected);
        }

        [Fact]
        public void Disconnect_WhenNotConnected_DoesNotThrow()
        {
            // Arrange

            // Act
            var ex = Record.Exception(() => _communicator!.Disconnect());

            // Assert
            Assert.Null(ex);
            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void Connect_WhenAlreadyConnected_DoesNotThrow()
        {
            // Arrange
            _communicator!.Connect();

            // Act
            var ex = Record.Exception(() => _communicator.Connect());

            // Assert
            Assert.Null(ex);
            Assert.True(_communicator.IsConnected);
        }

        [Fact]
        public void CheckSSHDevice_ValidHost_ReturnsSuccess()
        {
            // Arrange

            // Act
            SSHCheckResult result = _communicator!.checkSSHDevice(false);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Exception);
        }

        [Fact]
        public void CheckSSHDevice_InvalidHost_ReturnsFailure()
        {
            // Arrange
            var bad = new Communicator(
                "DefinitelyNotARealHost.local",
                _hubUser,
                _hubPass);

            // Act
            SSHCheckResult result = bad.checkSSHDevice(false);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void Connect_InvalidHost_Throws()
        {
            // Arrange
            var bad = new Communicator(
                "DefinitelyNotARealHost.local",
                _hubUser,
                _hubPass);

            // Act / Assert
            Assert.ThrowsAny<Exception>(() => bad.Connect());
        }

        [Fact]
        public void Dispose_WhenConnected_DisconnectsCleanly()
        {
            // Arrange
            _communicator!.Connect();

            // Act
            _communicator.Dispose();

            // Assert
            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void Dispose_WhenNeverConnected_DoesNotThrow()
        {
            // Arrange

            // Act
            var ex = Record.Exception(() => _communicator!.Dispose());

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void Dispose_CanBeCalledTwice()
        {
            // Arrange
            _communicator!.Connect();

            // Act
            _communicator.Dispose();

            var ex = Record.Exception(() => _communicator.Dispose());

            // Assert
            Assert.Null(ex);
        }
    }
}