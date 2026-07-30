using System;
using System.IO;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    /// <summary>
    /// Integration tests involving multiple nodes connected through the same Hub.
    ///
    /// These tests require:
    ///     - Hub online
    ///     - Node1 online
    ///     - Node2 online
    ///     - SSH enabled on all devices
    ///
    /// These tests intentionally use the real hardware and perform no mocking.
    /// </summary>
    public class MultiNodeTests : CommunicatorTestBase
    {
        private const string TempDirectory = "/tmp/";

        public MultiNodeTests()
        {
            Assert.True(_communicator!.Connect());

            _communicator.AddNodeSFTP(_node1Host, _node1User);
            _communicator.AddNodeSFTP(_node2Host, _node2User);
        }

        [Fact]
        public void SameFile_ToMultipleNodes()
        {
            string localFile = Path.GetTempFileName();
            string contents = Guid.NewGuid().ToString();

            File.WriteAllText(localFile, contents);

            string node1File = $"{TempDirectory}MultiNode_Node1_{Guid.NewGuid():N}.txt";
            string node2File = $"{TempDirectory}MultiNode_Node2_{Guid.NewGuid():N}.txt";

            try
            {
                _communicator!.CopyPCtoNode(localFile, node1File, _node1Host);
                _communicator.CopyPCtoNode(localFile, node2File, _node2Host);

                Assert.True(_communicator.NodeFileExists(node1File, _node1Host));
                Assert.True(_communicator.NodeFileExists(node2File, _node2Host));

                string node1Contents =
                    _communicator.ExecuteNodeCommand($"cat {node1File}", _node1Host, _node1User).Trim();

                string node2Contents =
                    _communicator.ExecuteNodeCommand($"cat {node2File}", _node2Host, _node2User).Trim();

                Assert.Equal(contents, node1Contents);
                Assert.Equal(contents, node2Contents);
            }
            finally
            {
                File.Delete(localFile);

                _communicator.DeleteNodeFile(node1File, _node1Host);
                _communicator.DeleteNodeFile(node2File, _node2Host);
            }
        }

        [Fact]
        public void IndependentNodeOperations()
        {
            string node1File = $"{TempDirectory}Node1_{Guid.NewGuid():N}.txt";
            string node2File = $"{TempDirectory}Node2_{Guid.NewGuid():N}.txt";

            try
            {
                CreateNodeFile(_communicator!, _node1Host, _node1User, node1File, "Node One");
                CreateNodeFile(_communicator!, _node2Host, _node2User, node2File, "Node Two");

                Assert.True(_communicator.NodeFileExists(node1File, _node1Host));
                Assert.True(_communicator.NodeFileExists(node2File, _node2Host));

                _communicator.DeleteNodeFile(node1File, _node1Host);

                Assert.False(_communicator.NodeFileExists(node1File, _node1Host));
                Assert.True(_communicator.NodeFileExists(node2File, _node2Host));
            }
            finally
            {
                _communicator.DeleteNodeFile(node1File, _node1Host);
                _communicator.DeleteNodeFile(node2File, _node2Host);
            }
        }

        [Fact]
        public void ParallelCommands()
        {
            string result1 =
                _communicator!.ExecuteNodeCommand("hostname", _node1Host, _node1User).Trim();

            string result2 =
                _communicator.ExecuteNodeCommand("hostname", _node2Host, _node2User).Trim();

            Assert.NotEmpty(result1);
            Assert.NotEmpty(result2);

            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void Node1Operations_DoNotAffectNode2()
        {
            string node1File = $"{TempDirectory}Isolation1_{Guid.NewGuid():N}.txt";
            string node2File = $"{TempDirectory}Isolation2_{Guid.NewGuid():N}.txt";

            try
            {
                CreateNodeFile(_communicator!, _node1Host, _node1User, node1File, "AAA");
                CreateNodeFile(_communicator!, _node2Host, _node2User, node2File, "BBB");

                _communicator.MoveNodeFile(
                    node1File,
                    node1File + ".moved",
                    _node1Host,
                    _node1User);

                Assert.False(_communicator.NodeFileExists(node1File, _node1Host));
                Assert.True(_communicator.NodeFileExists(node1File + ".moved", _node1Host));

                Assert.True(_communicator.NodeFileExists(node2File, _node2Host));

                string node2Contents =
                    _communicator.ExecuteNodeCommand(
                        $"cat {node2File}",
                        _node2Host,
                        _node2User).Trim();

                Assert.Equal("BBB", node2Contents);
            }
            finally
            {
                _communicator.DeleteNodeFile(node1File, _node1Host);
                _communicator.DeleteNodeFile(node1File + ".moved", _node1Host);
                _communicator.DeleteNodeFile(node2File, _node2Host);
            }
        }

        [Fact]
        public void SequentialOperationsAcrossNodes()
        {
            string node1Result =
                _communicator!.ExecuteNodeCommand("echo Node1", _node1Host, _node1User).Trim();

            string node2Result =
                _communicator.ExecuteNodeCommand("echo Node2", _node2Host, _node2User).Trim();

            string node1Again =
                _communicator.ExecuteNodeCommand("echo Again", _node1Host, _node1User).Trim();

            Assert.Equal("Node1", node1Result);
            Assert.Equal("Node2", node2Result);
            Assert.Equal("Again", node1Again);
        }

        [Fact]
        public void BothNodesRemainRegisteredAfterMultipleOperations()
        {
            for (int i = 0; i < 10; i++)
            {
                string result1 =
                    _communicator!.ExecuteNodeCommand(
                        "echo test",
                        _node1Host,
                        _node1User).Trim();

                string result2 =
                    _communicator.ExecuteNodeCommand(
                        "echo test",
                        _node2Host,
                        _node2User).Trim();

                Assert.Equal("test", result1);
                Assert.Equal("test", result2);
            }
        }
    }
}