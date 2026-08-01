using jCommunicator.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCommunicator.Tests.Integration
{
    public class NodeRegistrationTests : CommunicatorTestBase, IDisposable
    {
        public NodeRegistrationTests()
        {
            _communicator = new Communicator(_hubHost, _hubUser, _hubPass);
            _communicator.Connect();
        }

        public void Dispose()
        {
            _communicator?.Disconnect();
            _communicator?.Dispose();
        }

        [Fact]
        public void AddFirstNode_Returns2200()
        {
            int port = _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);

            Assert.Equal(2200, port);
        }

        [Fact]
        public void AddSecondNode_Returns2201()
        {
            int first = _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);
            int second = _communicator.AddNodeTunnel(_node2Host, _node2User, _node2Pass);

            Assert.Equal(2200, first);
            Assert.Equal(2201, second);
        }

        [Fact]
        public void AddedNode_CanAccessFiles()
        {
            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);

            bool exists = _communicator.NodeFileExists(
                "/etc/passwd",
                _node1Host);

            Assert.True(exists);
        }

        [Fact]
        public void MultipleNodes_CanBothAccessFiles()
        {
            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);
            _communicator.AddNodeTunnel(_node2Host, _node2User, _node2Pass);

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node1Host));

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node2Host));
        }

        [Fact]
        public void AddDuplicateNode_DoesNotThrow()
        {
            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);

            Exception ex = Record.Exception(() =>
                _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass));

            Assert.Null(ex);
        }

        [Fact]
        public void AddDuplicateNode_ReturnsSamePort()
        {
            int first = _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);
            int second = _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);

            Assert.Equal(first, second);
        }

        [Fact]
        public void AddNode_WithEmptyHostname_Throws()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                _communicator.AddNodeTunnel(
                    "",
                    _node1User,
                    _node1Pass);
            });
        }

        [Fact]
        public void AddNode_WithNullHostname_Throws()
        {
            Assert.ThrowsAny<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625
                _communicator.AddNodeTunnel(
                    null,
                    _node1User,
                    _node1Pass);
#pragma warning restore CS8625
            });
        }

        [Fact]
        public void AddNode_WithEmptyUsername_Throws()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                _communicator.AddNodeTunnel(
                    _node1Host,
                    "",
                    _node1Pass);
            });
        }

        [Fact]
        public void Reconnect_RebuildsNodeConnections()
        {
            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);

            _communicator.Disconnect();
            _communicator.Connect();

            bool exists = _communicator.NodeFileExists(
                "/etc/passwd",
                _node1Host);

            Assert.True(exists);
        }

        [Fact]
        public void RegisterTwoNodes_InSequence_BothRemainAccessible()
        {
            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);
            _communicator.AddNodeTunnel(_node2Host, _node2User, _node2Pass);

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node1Host));

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node2Host));
        }
    }
}