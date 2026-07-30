using Xunit;

namespace jCommunicator.Tests.Unit
{
    public class CommunicatorConstructorTests : CommunicatorTestBase
    {
        [Fact]
        public void Constructor_CreatesInstance()
        {
            // Arrange & Act
            var communicator = new Communicator(_hubHost, _hubUser, _hubPass);

            // Assert
            Assert.NotNull(communicator);
        }

        [Fact]
        public void Constructor_IsInitiallyDisconnected()
        {
            // Arrange & Act
            var communicator = new Communicator(_hubHost, _hubUser, _hubPass);

            // Assert
            Assert.False(communicator.IsConnected);
        }

        [Fact]
        public void Constructor_CanCreateMultipleIndependentInstances()
        {
            // Arrange & Act
            var com1 = new Communicator(_hubHost, _hubUser, _hubPass);
            var com2 = new Communicator(_hubHost, _hubUser, _hubPass);

            // Assert
            Assert.NotSame(com1, com2);
            Assert.False(com1.IsConnected);
            Assert.False(com2.IsConnected);
        }

        [Fact]
        public void Constructor_DisposeImmediately_DoesNotThrow()
        {
            // Arrange
            var communicator = new Communicator(_hubHost, _hubUser, _hubPass);

            // Act & Assert
            var ex = Record.Exception(() => communicator.Dispose());

            Assert.Null(ex);
        }

        [Fact]
        public void Constructor_DisposeTwice_DoesNotThrow()
        {
            // Arrange
            var communicator = new Communicator(_hubHost, _hubUser, _hubPass);

            // Act
            communicator.Dispose();
            var ex = Record.Exception(() => communicator.Dispose());

            // Assert
            Assert.Null(ex);
        }
    }
}