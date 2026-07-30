using jCommunicator;
using Xunit;

namespace jCommunicator.Tests.Unit
{
    public class CommunicatorStateTests : CommunicatorTestBase, IDisposable
    {
        [Fact]
        public void InitialState_IsDisconnected()
        {
            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void Disconnect_WhenNeverConnected_DoesNotThrow()
        {
            var ex = Record.Exception(() => _communicator.Disconnect());

            Assert.Null(ex);
            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void Dispose_WhenNeverConnected_DoesNotThrow()
        {
            var ex = Record.Exception(() => _communicator.Dispose());

            Assert.Null(ex);
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            _communicator.Dispose();

            var ex = Record.Exception(() => _communicator.Dispose());

            Assert.Null(ex);
        }

        [Fact]
        public void Disconnect_AfterDispose_DoesNotThrow()
        {
            _communicator.Dispose();

            var ex = Record.Exception(() => _communicator.Disconnect());

            Assert.Null(ex);
        }

        [Fact]
        public void IsConnected_RemainsFalse_AfterDispose()
        {
            _communicator.Dispose();

            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void RepeatedDisconnect_DoesNotThrow()
        {
            _communicator.Disconnect();

            var ex = Record.Exception(() => _communicator.Disconnect());

            Assert.Null(ex);
            Assert.False(_communicator.IsConnected);
        }

        [Fact]
        public void Dispose_DoesNotChangeDisconnectedState()
        {
            Assert.False(_communicator.IsConnected);

            _communicator.Dispose();

            Assert.False(_communicator.IsConnected);
        }
    }
}