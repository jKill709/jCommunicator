# Communicator Class

The `Communicator` class is the primary entry point for SSH-based cluster communication. It manages connections to a Cluster Hub and provides tunneling to individual Nodes for file operations and command execution.

## Overview

```csharp
public class Communicator : IAsyncDisposable
{
    public bool IsConnected { get; }
}
```

### Purpose

The `Communicator` establishes an SSH connection to a cluster hub server, manages SFTP sessions for file transfers, and creates forwarded port tunnels to individual nodes within the cluster. It uses the Renci.SshNet library for SSH functionality.

## Constructor

### Communicator(string host, string username, string password)

Creates a new instance of the `Communicator` class.

**Parameters:**
- `host` - The hostname or IP address of the cluster hub server
- `username` - The username for authentication
- `password` - The password for authentication

**Example:**
```csharp
var communicator = new Communicator("cluster.example.com", "admin", "secret");
```

## Properties

### IsConnected

Read-only boolean indicating whether the SSH connection is currently active.

**Type:** `bool`

## Public Methods

### ConnectAsync()

Establishes a connection to the cluster hub and all configured nodes.

**Returns:** `Task<bool>` - True if connection succeeded, false otherwise

**Example:**
```csharp
await communicator.ConnectAsync();
if (communicator.IsConnected)
{
    // Connection successful
}
```

### DisconnectAsync()

Closes the SSH connection and all node tunnels.

**Returns:** `Task`

**Example:**
```csharp
await communicator.DisconnectAsync();
```

### CheckConnectionAsync()

Ensures the hub is connected, establishing a new connection if necessary.

**Returns:** `Task`

**Example:**
```csharp
await communicator.CheckConnectionAsync();
```

### AddNodeTunnelAsync(string nodeHost, string nodeUsername, string nodePassword, bool verbose = false)

Creates a port-forwarded tunnel to an individual node within the cluster.

**Parameters:**
- `nodeHost` - The hostname of the node
- `nodeUsername` - The username for the node
- `nodePassword` - The password for the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<int>` - The local port number assigned to the tunnel, or 0 on failure

**Example:**
```csharp
int port = await communicator.AddNodeTunnelAsync("node1", "user1", "pass1");
```

### PingNodeAsync(string host, bool verbose = false)

Pings a node to verify connectivity through the tunnel.

**Parameters:**
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if the node is reachable

**Example:**
```csharp
bool connected = await communicator.PingNodeAsync("node1");
```

## Hub File Operations

### HubFileExists(string hubFilePath, bool verbose = false)

Checks if a file exists on the cluster hub.

**Parameters:**
- `hubFilePath` - The path to the file on the hub (e.g., "/path/to/file.txt")
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if the file exists

**Example:**
```csharp
bool exists = await communicator.HubFileExists("/data/output.txt");
```

### HubFileLastModified(string hubFilePath)

Gets the last modification time of a file on the cluster hub.

**Parameters:**
- `hubFilePath` - The path to the file on the hub

**Returns:** `Task<DateTime>` - The last modified time, or DateTime.MinValue if file doesn't exist

**Example:**
```csharp
DateTime lastModified = await communicator.HubFileLastModified("/data/output.txt");
```

### GetListOfHubFiles(string directory, string fileExtension, bool verbose = false)

Lists files in a directory on the cluster hub with matching extension.

**Parameters:**
- `directory` - The directory path
- `fileExtension` - The file extension to filter (e.g., "txt")
- `verbose` - Whether to log detailed information

**Returns:** `Task<List<LinuxFileInfo>>` - List of files matching the criteria

**Example:**
```csharp
var files = await communicator.GetListOfHubFiles("/data", "txt");
```

### DeleteHubFile(string hubFilePath, bool verbose = true)

Deletes a file from the cluster hub.

**Parameters:**
- `hubFilePath` - The path to the file on the hub
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if deletion succeeded

**Example:**
```csharp
await communicator.DeleteHubFile("/data/temp.txt");
```

### MoveHubFile(string currentFilePath, string newFilePath, bool verbose = false)

Moves (renames) a file on the cluster hub.

**Parameters:**
- `currentFilePath` - The current path of the file
- `newFilePath` - The new path for the file
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if move succeeded

**Example:**
```csharp
await communicator.MoveHubFile("/data/old.txt", "/data/new.txt");
```

## Node File Operations

### NodeFileExists(string nodeFilePath, string host, bool verbose = false)

Checks if a file exists on a specific node.

**Parameters:**
- `nodeFilePath` - The path to the file on the node
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if the file exists

**Example:**
```csharp
bool exists = await communicator.NodeFileExists("/data/output.txt", "node1");
```

### NodeFileLastModified(string nodeFilePath, string host, bool verbose = false)

Gets the last modification time of a file on a specific node.

**Parameters:**
- `nodeFilePath` - The path to the file on the node
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<DateTime?>` - The last modified time, or null if file doesn't exist

**Example:**
```csharp
DateTime? lastModified = await communicator.NodeFileLastModified("/data/output.txt", "node1");
```

### GetListOfNodeFiles(string directory, string fileExtension, string host, string username, bool verbose = false)

Lists files in a directory on a specific node with matching extension.

**Parameters:**
- `directory` - The directory path
- `fileExtension` - The file extension to filter
- `host` - The hostname of the node
- `username` - The username for the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<List<LinuxFileInfo>>` - List of files matching the criteria

**Example:**
```csharp
var files = await communicator.GetListOfNodeFiles("/data", "txt", "node1", "admin");
```

### DeleteNodeFile(string nodeFilePath, string host, bool verbose = false)

Deletes a file from a specific node.

**Parameters:**
- `nodeFilePath` - The path to the file on the node
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if deletion succeeded

**Example:**
```csharp
await communicator.DeleteNodeFile("/data/temp.txt", "node1");
```

### MoveNodeFile(string currentFilePath, string newFilePath, string host, string username, bool verbose = false)

Moves (renames) a file on a specific node.

**Parameters:**
- `currentFilePath` - The current path of the file
- `newFilePath` - The new path for the file
- `host` - The hostname of the node
- `username` - The username for the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if move succeeded

**Example:**
```csharp
await communicator.MoveNodeFile("/data/old.txt", "/data/new.txt", "node1", "admin");
```

## Cross-Transfer Operations

### CopyHubToNode(string hubFilePath, string nodeFilePath, string host, string username, bool verbose = false)

Copies a file from the cluster hub to a specific node.

**Parameters:**
- `hubFilePath` - The path on the hub
- `nodeFilePath` - The destination path on the node
- `host` - The hostname of the node
- `username` - The username for the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if copy succeeded

**Example:**
```csharp
bool success = await communicator.CopyHubToNode("/data/input.txt", "/data/output.txt", "node1", "admin");
```

### CopyNodeToHub(string nodeFilePath, string hubFilePath, string host, string username, bool verbose = false)

Copies a file from a specific node to the cluster hub.

**Parameters:**
- `nodeFilePath` - The path on the node
- `hubFilePath` - The destination path on the hub
- `host` - The hostname of the node
- `username` - The username for the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<bool>` - True if copy succeeded

**Example:**
```csharp
bool success = await communicator.CopyNodeToHub("/data/output.txt", "/data/input.txt", "node1", "admin");
```

## Asynchronous SFTP Transfer Methods

### PCtoHubAsync(string hubFilePath, string localDirectory, ClusterFileIOCommand command, bool verbose = false)

Downloads a file from the cluster hub to a local directory.

**Parameters:**
- `hubFilePath` - The path on the hub
- `localDirectory` - The local destination directory
- `command` - The transfer command configuration
- `verbose` - Whether to log detailed information

**Returns:** `Task<DownloadResult>` - Result of the transfer operation

### PCtoHubAsync(ClusterFileIOCommand command, bool verbose = false)

Downloads a file from the cluster hub using a pre-configured command.

**Parameters:**
- `command` - The transfer command configuration
- `verbose` - Whether to log detailed information

**Returns:** `Task<DownloadResult>` - Result of the transfer operation

### PCtoHubAsync(List<string> hubFilePaths, string localDirectory, ClusterFileIOCommand command, bool verbose = false)

Downloads multiple files from the cluster hub.

**Parameters:**
- `hubFilePaths` - List of paths on the hub
- `localDirectory` - The local destination directory
- `command` - The transfer command configuration
- `verbose` - Whether to log detailed information

**Returns:** `Task<List<DownloadResult>>` - List of transfer results

### PCtoHubAsync(List<ClusterFileIOCommand> commands, bool verbose = false)

Downloads multiple files using pre-configured commands.

**Parameters:**
- `commands` - List of transfer command configurations
- `verbose` - Whether to log detailed information

**Returns:** `Task<List<DownloadResult>>` - List of transfer results

### PCtoNodeAsync(string hubFilePath, string localDirectory, ClusterFileIOCommand command, string host, bool verbose = false)

Downloads a file from the cluster hub to a specific node.

**Parameters:**
- `hubFilePath` - The path on the hub
- `localDirectory` - The local destination directory
- `command` - The transfer command configuration
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<DownloadResult>` - Result of the transfer operation

### PCtoNodeAsync(ClusterFileIOCommand command, string host, bool verbose = false)

Downloads a file from the cluster hub using a pre-configured command.

**Parameters:**
- `command` - The transfer command configuration
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<DownloadResult>` - Result of the transfer operation

### PCtoNodeAsync(List<string> nodeFilePaths, string localDirectory, ClusterFileIOCommand command, string host, bool verbose = false)

Downloads multiple files from the cluster hub to a specific node.

**Parameters:**
- `nodeFilePaths` - List of paths on the hub
- `localDirectory` - The local destination directory
- `command` - The transfer command configuration
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<List<DownloadResult>>` - List of transfer results

### PCtoNodeAsync(List<ClusterFileIOCommand> commands, string host, bool verbose = false)

Downloads multiple files from the cluster hub using pre-configured commands.

**Parameters:**
- `commands` - List of transfer command configurations
- `host` - The hostname of the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<List<DownloadResult>>` - List of transfer results

## Command Execution

### ExecuteHubCommandAsync(string command, bool verbose = false)

Executes a command on the cluster hub.

**Parameters:**
- `command` - The command to execute
- `verbose` - Whether to log detailed information

**Returns:** `Task<string>` - The output of the command

**Example:**
```csharp
string result = await communicator.ExecuteHubCommandAsync("ls /data");
```

### ExecuteNodeCommandAsync(string cmd, string host, string username, bool verbose = false)

Executes a command on a specific node.

**Parameters:**
- `cmd` - The command to execute
- `host` - The hostname of the node
- `username` - The username for the node
- `verbose` - Whether to log detailed information

**Returns:** `Task<string>` - The output of the command

**Example:**
```csharp
string result = await communicator.ExecuteNodeCommandAsync("ls /data", "node1", "admin");
```

## Lifecycle Management

### Dispose()

Disposes of the communicator, disconnecting and cleaning up resources.

**Example:**
```csharp
using (var communicator = new Communicator(host, username, password))
{
    await communicator.ConnectAsync();
    // Use communicator
}
// Resources disposed here
```

### IAsyncDisposable.DisposeAsync()

Async version of Dispose - disconnects and cleans up resources.

**Example:**
```csharp
await communicator.DisposeAsync();
```

## Notes

- All file paths are normalized to use forward slashes (`/`) for SFTP compatibility
- The library uses a single SSH connection to the hub with port forwarding to nodes
- SFTP operations are performed through the `DownloadResult` class for consistent error handling
- Node tunnels are created on-demand and stored in an internal dictionary keyed by node hostname
