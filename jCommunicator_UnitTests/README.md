# Tests

This repository contains a comprehensive suite of unit and integration tests for the `jCommunicator` library, verifying functionality across hub commands, node registration, file transfer operations, connection management, and multi-node isolation. All tests are implemented using **xUnit** and leverage real hardware (Hub + Nodes) where appropriate to ensure end-to-end reliability.

---

## Test File Index

| File | Description |
|------|-------------|
| [`CommunicatorConstructorTests.cs`](./jCommunicator.Tests/Unit/CommunicatorConstructorTests.cs) | Validates constructor behavior, instance creation, and disposal safety. |
| [`CommunicatorStateTests.cs`](./jCommunicator.Tests/Unit/CommunicatorStateTests.cs) | Ensures correct state transitions (`IsConnected`) and idempotent disconnect/dispose. |
| [`SSHCheckResultTests.cs`](./jCommunicator.Tests/Unit/SSHCheckResultTests.cs) | Tests the `SSHCheckResult` data class for correct property assignment. |
| [`CommandExecutionTests.cs`](./jCommunicator.Tests/Integration/CommandExecutionTests.cs) | Integration tests for remote command execution on Hub and Nodes (echo, whoami, pwd, etc.). |
| [`ConnectionTests.cs`](./jCommunicator.Tests/Integration/ConnectionTests.cs) | Validates connect/disconnect cycles, reconnection, and invalid host handling. |
| [`FileTransferTests.cs`](./jCommunicator.Tests/Integration/FileTransferTests.cs) | Comprehensive file transfer tests: PC→Hub, Hub→Node, Node→PC, round-trips, binary/text, large files. |
| [`HubFileOperationTests.cs`](./jCommunicator.Tests/Integration/HubFileOperationTests.cs) | Tests Hub-side file existence, listing, modification time, move, delete, and edge cases (spaces, empty/large files). |
| [`MultiNodeTests.cs`](./jCommunicator.Tests/Integration/MultiNodeTests.cs) | Validates multi-node scenarios: same file to multiple nodes, independent operations, parallel commands, isolation. |
| [`NodeFileOperationTests.cs`](./jCommunicator.Tests/Integration/NodeFileOperationTests.cs) | Tests Node-side file existence, listing, move, delete, and handling of empty/large files with proper cleanup. |
| [`NodeRegistrationTests.cs`](./jCommunicator.Tests/Integration/NodeRegistrationTests.cs) | Validates SFTP tunnel registration, duplicate handling, null/empty parameter checks, and reconnection rebuild. |

---

## Test Breakdown by File

### `CommunicatorConstructorTests.cs`

| Test | Purpose |
|------|---------|
| `Constructor_CreatesInstance` | Ensures a new `Communicator` is instantiated successfully. |
| `Constructor_IsInitiallyDisconnected` | Confirms the constructor leaves the communicator in a disconnected state. |
| `Constructor_CanCreateMultipleIndependentInstances` | Verifies multiple instances are distinct and both start disconnected. |
| `Constructor_DisposeImmediately_DoesNotThrow` | Checks that disposing an unused communicator is safe. |
| `Constructor_DisposeTwice_DoesNotThrow` | Ensures double disposal does not raise exceptions. |

---

### `CommunicatorStateTests.cs`

| Test | Purpose |
|------|---------|
| `InitialState_IsDisconnected` | Confirms initial `IsConnected` is `false`. |
| `Disconnect_WhenNeverConnected_DoesNotThrow` | Validates disconnect on an already-disconnected instance is safe. |
| `Dispose_WhenNeverConnected_DoesNotThrow` | Checks dispose on a disconnected instance is safe. |
| `Dispose_CanBeCalledMultipleTimes` | Ensures multiple dispose calls are idempotent. |
| `Disconnect_AfterDispose_DoesNotThrow` | Confirms disconnect after dispose does not throw. |
| `IsConnected_RemainsFalse_AfterDispose` | Verifies state remains disconnected post-dispose. |
| `RepeatedDisconnect_DoesNotThrow` | Checks repeated disconnect calls are safe. |
| `Dispose_DoesNotChangeDisconnectedState` | Ensures dispose doesn't flip the state to connected. |

---

### `SSHCheckResultTests.cs`

| Test | Purpose |
|------|---------|
| `Constructor_Success_SetsProperties` | Validates properties when SSH check succeeds. |
| `Constructor_Failure_SetsProperties` | Validates exception and success flags on failure. |
| `Constructor_StoresExactExceptionInstance` | Ensures the original exception object is stored (not cloned). |
| `Constructor_AllowsZeroElapsedTime` | Checks zero elapsed time is accepted. |
| `Constructor_AllowsLongElapsedTime` | Verifies large elapsed times are handled correctly. |
| `FailureResult_CanContainException` | Confirms failure results can hold an exception instance. |

---

### `CommandExecutionTests.cs`

| Test | Purpose |
|------|---------|
| `ExecuteHubCommand_Echo_ReturnsExpectedString` | Basic echo command on Hub. |
| `ExecuteHubCommand_WhoAmI_ReturnsConfiguredUser` | Verifies `whoami` returns the configured hub user. |
| `ExecuteHubCommand_Pwd_ReturnsDirectory` | Ensures `pwd` returns a valid path. |
| `ExecuteHubCommand_Hostname_ReturnsNonEmpty` | Checks `hostname` yields non-empty output. |
| `ExecuteHubCommand_MultiLineOutput_ReturnsAllLines` | Validates multi-line command output handling. |
| `ExecuteHubCommand_CommandWithQuotes_ReturnsExpectedString` | Tests quoting preservation. |
| `ExecuteHubCommand_InvalidCommand_DoesNotThrow` | Confirms non-existent commands don't throw. |
| `ExecuteHubCommand_LongRunningCommand_Completes` | Ensures blocking commands complete correctly. |
| `ExecuteNodeCommand_Echo_ReturnsExpectedString` | Basic echo on Node 1. |
| `ExecuteNodeCommand_WhoAmIReturnsConfiguredUser` | Verifies `whoami` on Node. |
| `ExecuteNodeCommand_Pwd_ReturnsDirectory` | Checks `pwd` on Node. |
| `ExecuteNodeCommand_Hostname_ReturnsNonEmpty` | Validates `hostname` on Node. |
| `ExecuteNodeCommand_MultiLineOutput_ReturnsAllLines` | Tests multi-line output on Node. |
| `ExecuteNodeCommand_CommandWithQuotes_ReturnsExpectedString` | Ensures quoting on Node. |
| `ExecuteNodeCommand_InvalidCommand_DoesNotThrow` | Confirms invalid commands on Node don't throw. |
| `ExecuteNodeCommand_LongRunningCommand_Completes` | Validates long-running commands on Node. |
| `ExecuteHubCommand_EmptyCommand_DoesNotThrow` | Checks empty command handling. |
| `ExecuteHubCommand_CommandProducesNoOutput_ReturnsEmptyString` | Verifies commands with no output return empty string. |
| `ExecuteNodeCommand_CommandProducesNoOutput_ReturnsEmptyString` | Same for Node commands. |

---

### `ConnectionTests.cs`

| Test | Purpose |
|------|---------|
| `Connect_Succeeds` | Basic connect success. |
| `Disconnect_ClearsConnection` | Ensures disconnect clears the connection state. |
| `Connect_Disconnect_Reconnect_Succeeds` | Validates full connect/disconnect/reconnect cycle. |
| `Disconnect_WhenNotConnected_DoesNotThrow` | Checks disconnect on already-disconnected instance is safe. |
| `Connect_WhenAlreadyConnected_DoesNotThrow` | Ensures reconnection on connected instance is safe. |
| `CheckSSHDevice_ValidHost_ReturnsSuccess` | Validates SSH check on a known-good host. |
| `CheckSSHDevice_InvalidHost_ReturnsFailure` | Confirms SSH check fails on invalid host. |
| `Connect_InvalidHost_Throws` | Ensures connection to invalid host throws. |
| `Dispose_WhenConnected_DisconnectsCleanly` | Validates dispose cleans up a connected instance. |
| `Dispose_WhenNeverConnected_DoesNotThrow` | Checks dispose on disconnected instance is safe. |
| `Dispose_CanBeCalledTwice` | Ensures double dispose is safe after connect. |

---

### `FileTransferTests.cs`

| Test | Purpose |
|------|---------|
| `CopyPCToHub_TextFile` | Transfer text file from PC to Hub. |
| `CopyPCToHub_EmptyFile` | Transfer empty file from PC to Hub. |
| `CopyPCToHub_BinaryFile` | Transfer binary file from PC to Hub. |
| `CopyHubToPC_TextFile` | Transfer text file from Hub to PC. |
| `CopyHubToNode_TextFile` | Transfer text file from Hub to Node 1. |
| `CopyNodeToPC_TextFile` | Transfer text file from Node 1 to PC. |
| `CopyPCToNode_TextFile` | Transfer text file from PC to Node 1. |
| `RoundTrip_PC_Hub_PC_PreservesContents` | Verify content preservation via Hub round-trip (binary). |
| `RoundTrip_PC_Node_PC_PreservesContents` | Verify content preservation via Node round-trip (binary). |
| `RoundTrip_PC_Hub_Node_PC_PreservesContents` | Multi-hop round-trip preserving binary data. |
| `CopyLargeFile_OneMegabyte` | Validate large file (1 MB) transfer integrity. |

---

### `HubFileOperationTests.cs`

| Test | Purpose |
|------|---------|
| `HubFileExists_NewFile_ReturnsTrue` | Confirms newly created Hub files are detected. |
| `HubFileExists_MissingFile_ReturnsFalse` | Ensures missing files return false. |
| `HubFileLastModified_NewFile_IsRecent` | Validates modification time is recent. |
| `GetListOfHubFiles_ReturnsCreatedFile` | Tests listing files in a directory. |
| `DeleteHubFile_RemovesFile` | Confirms deletion removes the file. |
| `DeleteHubFile_MissingFile_DoesNotThrow` | Checks delete on missing file is safe. |
| `MoveHubFile_RenamesFile` | Validates move operation renames correctly. |
| `MoveHubFile_InvalidSource_ReturnsFalse` | Ensures invalid source returns false. |
| `HubFileNames_WithSpaces_Work` | Tests filenames containing spaces. |
| `HubEmptyFile_Exists` | Confirms empty file detection works. |
| `HubLargeFile_Exists` | Validates large file (10 MB) detection. |

---

### `MultiNodeTests.cs`

| Test | Purpose |
|------|---------|
| `SameFile_ToMultipleNodes` | Copies same file to Node 1 and Node 2, verifies both succeed. |
| `IndependentNodeOperations` | Ensures operations on one node don't affect the other. |
| `ParallelCommands` | Validates simultaneous command execution on both nodes. |
| `Node1Operations_DoNotAffectNode2` | Confirms isolate: moving a file on Node 1 doesn't impact Node 2. |
| `SequentialOperationsAcrossNodes` | Tests sequential commands across nodes without interference. |
| `BothNodesRemainRegisteredAfterMultipleOperations` | Ensures nodes stay registered after many operations. |

---

### `NodeFileOperationTests.cs`

| Test | Purpose |
|------|---------|
| `NodeFileExists_FileExists_ReturnsTrue` | Confirms existence detection on Node. |
| `NodeFileExists_FileMissing_ReturnsFalse` | Ensures missing files return false. |
| `NodeFileLastModified_ReturnsRecentTime` | Validates modification time recency. |
| `GetListOfNodeFiles_ReturnsCreatedFile` | Tests listing Node files. |
| `DeleteNodeFile_RemovesFile` | Confirms deletion removes the file. |
| `DeleteNodeFile_MissingFile_DoesNotThrow` | Checks delete on missing file is safe. |
| `MoveNodeFile_RenamesFile` | Validates move operation on Node. |
| `MoveNodeFile_InvalidSource_Throws` | Ensures invalid source throws. |
| `NodeFileOperations_HandleEmptyFile` | Confirms empty files are handled correctly. |
| `NodeFileOperations_HandleLargeFile` | Validates large file (10 MB) handling. |
| `NodeFileOperations_WithInvalidNode_Throws` | Ensures invalid node parameter throws. |

---

### `NodeRegistrationTests.cs`

| Test | Purpose |
|------|---------|
| `AddFirstNode_Returns2200` | Validates first node gets port 2200. |
| `AddSecondNode_Returns2201` | Ensures second node gets sequential port 2201. |
| `AddedNode_CanAccessFiles` | Confirms registered node can access files. |
| `MultipleNodes_CanBothAccessFiles` | Validates both nodes accessible simultaneously. |
| `AddDuplicateNode_DoesNotThrow` | Checks duplicate registration is safe. |
| `AddDuplicateNode_ReturnsSamePort` | Ensures duplicate returns same port. |
| `AddNode_WithEmptyHostname_Throws` | Validates empty hostname throws. |
| `AddNode_WithNullHostname_Throws` | Confirms null hostname throws `ArgumentNullException`. |
| `AddNode_WithEmptyUsername_Throws` | Checks empty username throws. |
| `Reconnect_RebuildsNodeConnections` | Ensures nodes are rebuilt after reconnect. |
| `RegisterTwoNodes_InSequence_BothRemainAccessible` | Validates sequential registration preserves both nodes. |

---

## 📊 Test Statistics

- **Total Tests:** 93
- **Unit Tests:** 18 (Constructor, State, SSHCheckResult)
- **Integration Tests:** 75 (Command execution, connections, file transfers, multi-node, node operations, registration)
- **Coverage Areas:** Connection lifecycle, SFTP tunneling, Hub/Node command execution, file CRUD operations, large/binary data integrity, edge cases (spaces, empty files, invalid inputs), and multi-node isolation.

Integration test require real hardware and many will fail if run in parallell, due to RenciSSH not  being threadsafe. No mocking is used for core functionality to guarantee production-like behavior.