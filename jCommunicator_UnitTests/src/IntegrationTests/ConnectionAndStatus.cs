using jCommunicator;
using jCommunicator.Tests;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class ConnectionAndStatus_ConnectAsync : CommunicatorTestBase
    {
        [Fact]
        public async Task ConnectAsync_WithValidCredentials_ShouldReturnTrue()
        {
            // Setup

            // Steps
            bool result = await _com.ConnectAsync();

            // Expected Result
            Assert.True(result);
        }

        [Fact]
        public async Task ConnectAsync_WithValidCredentials_ShouldSetIsConnectedToTrue()
        {
            // Setup

            // Steps
            await _com.ConnectAsync();
            bool isConnected = _com.IsConnected;

            // Expected Result
            Assert.True(isConnected);
        }

        [Fact]
        public async Task ConnectAsync_ReturnsTaskOfBool()
        {
            // Setup

            // Steps
            var result = _com.ConnectAsync();

            // Expected Result
            Assert.IsType<Task<bool>>(result);
        }
    }

    public class ConnectionAndStatus_IsConnected : CommunicatorTestBase
    {
        [Fact]
        public void IsConnected_InitialState_ShouldBeFalse()
        {
            // Setup
            Communicator newCom = new Communicator(hubHost, hubUser, hubPass);

            // Steps
            bool isConnected = newCom.IsConnected;

            // Expected Result
            Assert.False(isConnected);
        }

        [Fact]
        public async Task IsConnected_AfterConnect_ShouldBeTrue()
        {
            // Setup

            // Steps
            await _com.ConnectAsync();
            bool isConnected = _com.IsConnected;

            // Expected Result
            Assert.True(isConnected);
        }

        [Fact]
        public async Task IsConnected_AfterDisconnect_ShouldBeFalse()
        {
            // Setup

            // Steps
            await _com.ConnectAsync();
            bool isConnectedAfterConnect = _com.IsConnected;
            await _com.DisconnectAsync();
            bool isConnectedAfterDisconnect = _com.IsConnected;

            // Expected Result
            Assert.True(isConnectedAfterConnect);
            Assert.False(isConnectedAfterDisconnect);
        }

        [Fact]
        public async Task IsConnected_Persistence_ShouldRemainTrueUntilDisconnect()
        {
            // Setup

            // Steps
            await _com.ConnectAsync();

            bool isConnectedAfterConnect = _com.IsConnected;
            bool isConnectedAfterMultipleChecks = _com.IsConnected;

            await _com.DisconnectAsync();

            bool isConnectedAfterDisconnect = _com.IsConnected;

            // Expected Result
            Assert.True(isConnectedAfterConnect);
            Assert.True(isConnectedAfterMultipleChecks);
            Assert.False(isConnectedAfterDisconnect);
        }

        [Fact]
        public void IsConnected_ReturnsBool()
        {
            // Setup
            Communicator com = new Communicator("Hub1.local", "camcpp", "cam");

            // Steps
            bool isConnected = com.IsConnected;

            // Expected Result
            Assert.IsType<bool>(isConnected);
        }
    }

    public class ConnectionAndStatus_CheckConnectionAsync : CommunicatorTestBase
    {
        [Fact]
        public async Task CheckConnectionAsync_WhenAlreadyConnected_ShouldPassImmediately()
        {
            // Setup
            Communicator newCom = new Communicator(hubHost, hubUser, hubPass);

            // Steps
            await newCom.ConnectAsync();
            await newCom.CheckConnectionAsync();

            // Expected Result
            Assert.True(newCom.IsConnected);
        }

        [Fact]
        public async Task CheckConnectionAsync_WhenNotConnected_ShouldAutoConnect()
        {
            // Setup
            Communicator com = new Communicator(hubHost, hubUser, hubPass);

            // Steps
            await com.ConnectAsync();
            await com.DisconnectAsync();
            await com.CheckConnectionAsync();

            // Expected Result
            Assert.True(com.IsConnected);
        }
    }
}
