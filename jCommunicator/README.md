# jCommunicator

A .NET library for SSH-based cluster communication, providing secure file transfer operations between a local PC, a cluster hub server, and individual nodes within the cluster.

## Features

- **SSH Tunneling**: Connect to a central cluster hub and create port-forwarded tunnels to individual nodes
- **SFTP File Operations**: Download, upload, move, and delete files with comprehensive result tracking
- **Node Management**: Add and maintain tunnels to multiple cluster nodes
- **File Listing**: Retrieve detailed file metadata from both hub and nodes (permissions, owner, group, size, timestamps)
- **Command Execution**: Run shell commands on the hub and nodes
- **Cross-Transfer**: Copy files between hub and nodes using SCP
- **Result Tracking**: Detailed transfer results with existence checks, attribute retrieval, and delete operations
- **Logging**: Integrated logging via mLogger for debugging and monitoring

## Requirements

- **.NET 8.0** runtime or higher
- SSH access to a cluster hub server
- SFTP access credentials for the hub and nodes
- `Renci.SshNet.Async` package (version 1.4.0)

## Installation

```bash
dotnet add package Renci.SshNet.Async --version 1.4.0
```

Or via NuGet:
```
Install-Package Renci.SshNet.Async -Version 1.4.0
```

## Quick Start

```csharp
using jCommunicator;
using System.Threading.Tasks;

// Initialize the communicator
var communicator = new Communicator("cluster.example.com", "admin", "secret");

try
{
    // Connect to the cluster hub
    await communicator.ConnectAsync();

    if (communicator.IsConnected)
    {
        Console.WriteLine("Connected!");

        // Add a tunnel to a node
        int port = await communicator.AddNodeTunnelAsync(
            "node1.example.com", 
            "user1", 
            "pass1");

        // List files on the hub
        var hubFiles = await communicator.GetListOfHubFiles("/data", "txt");
        foreach (var file in hubFiles)
        {
            Console.WriteLine(file);
        }

        // List files on a node
        var nodeFiles = await communicator.GetListOfNodeFiles(
            "/data", 
            "txt", 
            "node1", 
            "user1");
        foreach (var file in nodeFiles)
        {
            Console.WriteLine(file);
        }

        // Upload a file to the hub
        var uploadCommand = ClusterFileIOCommand.Create(
            "input.txt", 
            "/data/upload/", 
            "/local/input.txt", 
            ClusterFileIOCommandType.Upload,
            getAttributes: true);
        
        var result = await communicator.PCtoHubAsync(uploadCommand);
        Console.WriteLine(result);

        // Download a file from the hub
        var downloadCommand = ClusterFileIOCommand.Create(
            "/data/output.txt", 
            "/local/download/", 
            ClusterFileIOCommandType.Download,
            checkExists: true,
            getAttributes: true,
            deleteAfter: true);
        
        result = await communicator.PCtoHubAsync(downloadCommand);
        Console.WriteLine(result);

        // Copy a file from hub to node
        bool copied = await communicator.CopyHubToNode(
            "/data/input.txt", 
            "/data/output.txt", 
            "node1", 
            "user1");

        // Run a command on the hub
        string output = await communicator.ExecuteHubCommandAsync("ls /data");
        Console.WriteLine(output);

        // Run a command on a node
        string nodeOutput = await communicator.ExecuteNodeCommandAsync(
            "cat /data/output.txt", 
            "node1", 
            "user1");
        Console.WriteLine(nodeOutput);
    }
}
finally
{
    // Clean up resources
    await communicator.DisposeAsync();
}
```

## Usage Examples

### Hub File Operations

#### Check if a file exists on the hub
```csharp
bool exists = await communicator.HubFileExists("/data/report.txt");
```

#### Get file metadata
```csharp
DateTime lastModified = await communicator.HubFileLastModified("/data/report.txt");
Console.WriteLine($"Last modified: {lastModified}");
```

#### List all .txt files in a directory
```csharp
var files = await communicator.GetListOfHubFiles("/data", "txt");
foreach (var file in files)
{
    Console.WriteLine($"{file.Name}: {file.Size} bytes, owner: {file.Owner}");
}
```

#### Delete a file from the hub
```csharp
await communicator.DeleteHubFile("/data/temp.txt");
```

### Node Operations

#### Check if a file exists on a specific node
```csharp
bool exists = await communicator.NodeFileExists("/data/output.txt", "node1");
```

#### List files on a node
```csharp
var files = await communicator.GetListOfNodeFiles(
    "/data", 
    "txt", 
    "node1", 
    "admin");
```

### Cross-Transfer

#### Copy from hub to node
```csharp
bool success = await communicator.CopyHubToNode(
    "/data/input.txt", 
    "/data/output.txt", 
    "node1", 
    "admin");
```

#### Copy from node to hub
```csharp
bool success = await communicator.CopyNodeToHub(
    "/data/output.txt", 
    "/data/input.txt", 
    "node1", 
    "admin");
```

### Command Execution

#### Execute a command on the hub
```csharp
string result = await communicator.ExecuteHubCommandAsync("ls -la /data");
```

#### Execute a command on a node
```csharp
string result = await communicator.ExecuteNodeCommandAsync(
    "cat /data/output.txt", 
    "node1", 
    "admin");
```

## Project Structure

```
jCommunicator/
├── jCommunicator/                    # Main library project
│   ├── src/                          # Source files
│   │   ├── Communicator.cs           # Main class with all operations
│   │   ├── DownloadResult.cs         # Transfer result tracking
│   │   └── LinuxFileInfo.cs          # File metadata struct
│   └── jCommunicator.csproj          # Project file
├── jCommunicator_UnitTests/          # Unit test project
│   └── src/
│       ├── CommunicatorTestBase.cs
│       └── UnitTests/
│           └── ClusterFileIOCommandConstructors_Group*.cs
├── mLogger/                          # Logging library dependency
├── jCommunicator.slnx               # Solution file
└── README.md                        # This file
```

## API Reference

### Core Types

| Type | Description |
|------|-------------|
| [Communicator](doc/Communicator.md) | Main class for cluster communication |
| DownloadResult | Transfer operation result container |
| ClusterFileIOCommand | File I/O command configuration |
| ClusterFileIOCommandType | Enum of supported operation types |
| LinuxFileInfo | Detailed file metadata from hub/nodes |

### Communicator Methods by Category

**Connection:**
- `ConnectAsync()` - Establish connection to cluster hub
- `DisconnectAsync()` - Close all connections
- `CheckConnectionAsync()` - Ensure hub is connected
- `AddNodeTunnelAsync()` - Create tunnel to a node
- `PingNodeAsync()` - Verify node connectivity

**Hub File Operations:**
- `HubFileExists()` / `DeleteHubFile()` / `MoveHubFile()`
- `HubFileLastModified()`
- `GetListOfHubFiles()`

**Node File Operations:**
- `NodeFileExists()` / `DeleteNodeFile()` / `MoveNodeFile()`
- `NodeFileLastModified()`
- `GetListOfNodeFiles()`

**Cross-Transfer:**
- `CopyHubToNode()` / `CopyNodeToHub()`

**Command Execution:**
- `ExecuteHubCommandAsync()` / `ExecuteNodeCommandAsync()`

**SFTP Transfers:**
- `PCtoHubAsync()` (multiple overloads)
- `PCtoNodeAsync()` (multiple overloads)

## Building

```bash
cd jCommunicator/jCommunicator
dotnet build
```

## Testing

Run the unit tests:

```bash
cd jCommunicator_UnitTests
dotnet test
```

## Contributing

Contributions are welcome! Please ensure:

1. All public types have comprehensive documentation
2. Code follows .NET C# best practices
3. Unit tests cover new functionality
4. Documentation is updated accordingly

## License

This project is part of the jCommunicator repository. See LICENSE for details.
