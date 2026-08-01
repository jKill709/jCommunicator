using Xunit;

namespace jCommunicator.Tests.Integration
{
    /// <summary>
    /// Integration tests for remote command execution.
    ///
    /// These tests require:
    ///     - A running Hub
    ///     - SSH enabled
    ///     - Node 1 reachable
    ///     - Valid credentials
    ///
    /// These are intentionally integration tests and should not be run on
    /// systems without the expected hardware.
    /// </summary>
    public class CommandExecutionTests : CommunicatorTestBase
    {
        public CommandExecutionTests()
        {
            _communicator!.Connect();
            _communicator.AddNodeTunnel(_node1Host, _node1User, _node1Pass);
        }

        #region Hub Commands

        [Fact]
        public void ExecuteHubCommand_Echo_ReturnsExpectedString()
        {
            string result = _communicator!.ExecuteHubCommand("echo test");

            Assert.Equal("test", result.Trim());
        }

        [Fact]
        public void ExecuteHubCommand_WhoAmI_ReturnsConfiguredUser()
        {
            string result = _communicator!.ExecuteHubCommand("whoami");

            Assert.Equal(_hubUser, result.Trim());
        }

        [Fact]
        public void ExecuteHubCommand_Pwd_ReturnsDirectory()
        {
            string result = _communicator!.ExecuteHubCommand("pwd");

            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.StartsWith("/", result.Trim());
        }

        [Fact]
        public void ExecuteHubCommand_Hostname_ReturnsNonEmpty()
        {
            string result = _communicator!.ExecuteHubCommand("hostname");

            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void ExecuteHubCommand_MultiLineOutput_ReturnsAllLines()
        {
            string result = _communicator!.ExecuteHubCommand("printf 'A\nB\nC\n'");

            string[] lines = result
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(3, lines.Length);
            Assert.Equal("A", lines[0]);
            Assert.Equal("B", lines[1]);
            Assert.Equal("C", lines[2]);
        }

        [Fact]
        public void ExecuteHubCommand_CommandWithQuotes_ReturnsExpectedString()
        {
            string result = _communicator!.ExecuteHubCommand("echo 'Hello World'");

            Assert.Equal("Hello World", result.Trim());
        }

        [Fact]
        public void ExecuteHubCommand_InvalidCommand_DoesNotThrow()
        {
            Exception? ex = Record.Exception(() =>
            {
                _communicator!.ExecuteHubCommand("command_that_should_not_exist");
            });

            Assert.Null(ex);
        }

        [Fact]
        public void ExecuteHubCommand_LongRunningCommand_Completes()
        {
            string result = _communicator!.ExecuteHubCommand("sleep 1 && echo done");

            Assert.Equal("done", result.Trim());
        }

        #endregion

        #region Node Commands

        [Fact]
        public void ExecuteNodeCommand_Echo_ReturnsExpectedString()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "echo test",
                _node1Host,
                _node1User);

            Assert.Equal("test", result.Trim());
        }

        [Fact]
        public void ExecuteNodeCommand_WhoAmIReturnsConfiguredUser()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "whoami",
                _node1Host,
                _node1User);

            Assert.Equal(_node1User, result.Trim());
        }

        [Fact]
        public void ExecuteNodeCommand_Pwd_ReturnsDirectory()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "pwd",
                _node1Host,
                _node1User);

            Assert.StartsWith("/", result.Trim());
        }

        [Fact]
        public void ExecuteNodeCommand_Hostname_ReturnsNonEmpty()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "hostname",
                _node1Host,
                _node1User);

            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void ExecuteNodeCommand_MultiLineOutput_ReturnsAllLines()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "printf '1\n2\n3\n'",
                _node1Host,
                _node1User);

            string[] lines = result
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(3, lines.Length);
            Assert.Equal("1", lines[0]);
            Assert.Equal("2", lines[1]);
            Assert.Equal("3", lines[2]);
        }

        [Fact]
        public void ExecuteNodeCommand_CommandWithQuotes_ReturnsExpectedString()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "echo 'Node Test'",
                _node1Host,
                _node1User);

            Assert.Equal("Node Test", result.Trim());
        }

        [Fact]
        public void ExecuteNodeCommand_InvalidCommand_DoesNotThrow()
        {
            Exception? ex = Record.Exception(() =>
            {
                _communicator!.ExecuteNodeCommand(
                    "command_that_should_not_exist",
                    _node1Host,
                    _node1User);
            });

            Assert.Null(ex);
        }

        [Fact]
        public void ExecuteNodeCommand_LongRunningCommand_Completes()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "sleep 1 && echo finished",
                _node1Host,
                _node1User);

            Assert.Equal("finished", result.Trim());
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void ExecuteHubCommand_EmptyCommand_DoesNotThrow()
        {
            Exception? ex = Record.Exception(() =>
            {
                _communicator!.ExecuteHubCommand("");
            });

            Assert.Null(ex);
        }

        [Fact]
        public void ExecuteHubCommand_CommandProducesNoOutput_ReturnsEmptyString()
        {
            string result = _communicator!.ExecuteHubCommand("true");

            Assert.True(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void ExecuteNodeCommand_CommandProducesNoOutput_ReturnsEmptyString()
        {
            string result = _communicator!.ExecuteNodeCommand(
                "true",
                _node1Host,
                _node1User);

            Assert.True(string.IsNullOrWhiteSpace(result));
        }

        #endregion
    }
}