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
            int port = _communicator.AddNodeSFTP(_node1Host, _node1User);

            Assert.Equal(2200, port);
        }

        [Fact]
        public void AddSecondNode_Returns2201()
        {
            int first = _communicator.AddNodeSFTP(_node1Host, _node1User);
            int second = _communicator.AddNodeSFTP(_node2Host, _node2User);

            Assert.Equal(2200, first);
            Assert.Equal(2201, second);
        }

        [Fact]
        public void AddedNode_CanAccessFiles()
        {
            _communicator.AddNodeSFTP(_node1Host, _node1User);

            bool exists = _communicator.NodeFileExists(
                "/etc/passwd",
                _node1Host);

            Assert.True(exists);
        }

        [Fact]
        public void MultipleNodes_CanBothAccessFiles()
        {
            _communicator.AddNodeSFTP(_node1Host, _node1User);
            _communicator.AddNodeSFTP(_node2Host, _node2User);

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node1Host));

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node2Host));
        }

        [Fact]
        public void AddDuplicateNode_DoesNotThrow()
        {
            _communicator.AddNodeSFTP(_node1Host, _node1User);

            Exception ex = Record.Exception(() =>
                _communicator.AddNodeSFTP(_node1Host, _node1User));

            Assert.Null(ex);
        }

        [Fact]
        public void AddDuplicateNode_ReturnsSamePort()
        {
            int first = _communicator.AddNodeSFTP(_node1Host, _node1User);
            int second = _communicator.AddNodeSFTP(_node1Host, _node1User);

            Assert.Equal(first, second);
        }

        [Fact]
        public void AddNode_WithEmptyHostname_Throws()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                _communicator.AddNodeSFTP(
                    "",
                    _node1User);
            });
        }

        [Fact]
        public void AddNode_WithNullHostname_Throws()
        {
            Assert.ThrowsAny<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625
                _communicator.AddNodeSFTP(
                    null,
                    _node1User);
#pragma warning restore CS8625
            });
        }

        [Fact]
        public void AddNode_WithEmptyUsername_Throws()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                _communicator.AddNodeSFTP(
                    _node1Host,
                    "");
            });
        }

        [Fact]
        public void Reconnect_RebuildsNodeConnections()
        {
            _communicator.AddNodeSFTP(_node1Host, _node1User);

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
            _communicator.AddNodeSFTP(_node1Host, _node1User);
            _communicator.AddNodeSFTP(_node2Host, _node2User);

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node1Host));

            Assert.True(
                _communicator.NodeFileExists("/etc/passwd", _node2Host));
        }
    }
}