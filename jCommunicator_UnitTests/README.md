# jCommunicator Unit Tests

This repository contains the unit and integration test suite for the jCommunicator library, verifying functionality across PC, Hub, and Node components.

## Project Overview

The jCommunicator library enables file and command operations across a cluster of systems (PCs, Hubs, and Nodes). This test suite validates:
- File transfer operations between all system types
- Connection management and status tracking
- Command execution on remote systems
- Error handling for invalid inputs and unreachable hosts
- Logging contract compliance
- Node tunnel setup and teardown

## Features

### Unit Tests
- **ClusterFileIOCommandConstructors**: Validates path parsing, normalization, and all command type initializations
- **DownloadResult_Constructors**: Ensures proper initialization of result objects with correct property states

### Integration Tests
- **End-to-End File Operations**: PC→Hub→Node transfers
- **Connection Management**: Connect/disconnect lifecycle
- **Error Handling**: Null checks, invalid credentials, unreachable hosts, non-existent paths
- **Command Execution**: Hub and Node command running with verbose output
- **File Operations**: Exists checks, timestamps, listing, delete, move
- **Batch Operations**: Multiple file transfers to/from nodes

## Requirements

- .NET 8.0
- xUnit test framework
- SSH connectivity to a Hub and at least one Node for integration tests

## Project Structure

```
jCommunicator_UnitTests/
├── src/
│   ├── CommunicatorTestBase.cs       # Base class for all tests
│   ├── UnitTests/
│   │   ├── ClusterFileIOCommandConstructors.cs
│   │   └── DownloadResult_Constructors.cs
│   └── IntegrationTests/
│       ├── ClusterTest_EndToEnd.cs
│       ├── ConnectionAndStatus.cs
│       ├── DownloadResult_NormalOperation.cs
│       ├── ErrorHandling.cs
│       ├── ExecuteCommandAsync.cs
│       ├── HubFileOperations.cs
│       ├── NodeTunnelSetup.cs
│       └── PCtoHubPCtoNodeAsync.cs
├── doc/
│   ├── IntegrationTests/
│   │   └── README.md                 # Integration test documentation
│   ├── UnitTests/
│   │   ├── README.md                 # Unit test documentation
│   │   ├── ClusterFileIOCommandConstructors.md
│   │   └── DownloadResult_Constructors.md
└── README.md                         # This file
```

## API Reference

### Unit Tests

#### ClusterFileIOCommandConstructors

| Class | Description |
|-------|-------------|
| `ClusterFileIOCommandConstructors_CloneConstructors` | Validates independent cloning of commands preserves all properties |
| `ClusterFileIOCommandConstructors_PathParsingAndNormalization` | Tests path handling and normalization logic |
| `ClusterFileIOCommandConstructor_NormalOperation` | Tests all command types (Exists, Attributes, Download, Upload, Move, Delete) |
| `ClusterFileIOCommandConstructors_BadInputs` | Tests error handling with invalid arguments |

#### DownloadResult_Constructors

| Class | Description |
|-------|-------------|
| `DownloadResult_Constructors` | Tests constructor initialization and property preservation |

### Integration Tests

| Test Category | Description |
|---------------|-------------|
| `ClusterTest_EndToEnd` | End-to-end PC→Hub→Node file transfer workflow |
| `ConnectionAndStatus_ConnectAsync` | ConnectAsync with valid credentials returns true and sets IsConnected |
| `ConnectionAndStatus_IsConnected` | IsConnected state transitions (false→true→false) |
| `ConnectionAndStatus_CheckConnectionAsync` | Auto-reconnect behavior |
| `DownloadResult_NormalOperation` | DownloadResult properties and ToString formatting |
| `ErrorHandling_WithBadConstructorArgs` | Null/empty argument validation |
| `ErrorHandling_WhenHubUnreachable` | SocketException for bad host credentials |
| `ErrorHandling_NonexistantHubPaths` | UnauthorizedAccessException for invalid paths |
| `ErrorHandling_AddNodeTunnelWithBadCredentials` | Invalid credentials return port 0 |
| `ErrorHandling_Communicator_LoggingContract` | All methods use Logger instance |
| `ExecuteCommandAsync_NormalOperation` | Hub and Node command execution returns output string |
| `HubFileOperations_NormalOperations` | Exists, lastModified, list, delete, move on Hub |
| `NodeTunnelSetup_AddNodeTunnelAsync_NormalOperation` | Valid credentials return port > 0 |
| `NodeTunnelSetup_AddNodeTunnelAsync_WithBadCredentials` | Null/empty argument validation |
| `PCtoHubPCtoNodeAsync_NormalOperation` | Single and batch file transfers |

## Running the Tests

### Build and Run All Tests

```bash
cd jCommunicator_UnitTests
dotnet build
dotnet test
```

### Run Specific Test Categories

```bash
# Only unit tests
dotnet test --filter "Category=Unit"

# Only integration tests
dotnet test --filter "Category=Integration"

# Specific test class
dotnet test --filter "FullyQualifiedName~ClusterFileIOCommandConstructors"
```

### Generate Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Summary

| Category | Tests | Description |
|----------|-------|-------------|
| UnitTests | 53 | Constructor and initialization tests |
| IntegrationTests | 67 | End-to-end system behavior tests |
| **Total** | **120** | |

## Contributing

When adding new tests:
- Follow the existing test naming convention
- Use `CommunicatorTestBase` for shared setup/teardown
- Include both positive and negative test cases
- Add integration tests that exercise multiple components together

## License

This project is part of the jCommunicator library. See the main repository for licensing information.
