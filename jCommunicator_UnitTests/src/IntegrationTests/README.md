# Integration Tests

This directory contains integration tests that verify the complete system behavior across components.

## Test Categories

### ClusterTest_EndToEnd

End-to-end tests for PC-to-Hub-to-Node file operations:
- Upload from PC to Hub
- Copy from Hub to Node
- Download from Hub to PC
- Direct PC-to-Node transfers

### ConnectionAndStatus

Tests for ConnectAsync and related status methods:
- Initial connection state
- Connection persistence
- Auto-reconnect behavior

### DownloadResult_NormalOperation

Tests for DownloadResult class operations:
- Properties initialization
- Success determination logic
- ToString formatting

### ErrorHandling

Error handling tests:
- Bad constructor arguments
- Hub unreachable scenarios
- Non-existent paths
- Invalid credentials

### ExecuteCommandAsync

Command execution tests:
- Hub command execution
- Node command execution
- Verbose output handling

### HubFileOperations

Hub file operations:
- File existence checks
- Last modified timestamps
- List directory contents
- Delete and move files

### PCtoHubPCtoNodeAsync

File transfer operations:
- Single file transfers (PCtoHub, PCtoNode)
- Multiple file batch transfers

### NodeTunnelSetup

Node tunnel setup tests:
- AddNodeTunnelAsync with valid/invalid credentials
- Ping node functionality
- Node file operations after tunnel setup

### Communicator_LoggingContract

Tests for logging contract:
- Verify all methods use the logger instance
- Check log output contains expected content

