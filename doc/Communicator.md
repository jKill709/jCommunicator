# Communicator

## Namespace

jCommunicator

## Purpose

Manages SSH connections to a Cluster Hub and tunnels to individual Nodes for file operations and command execution.

## Constructors

```csharp
public Communicator(string host, string username, string password);
```

Creates a new Communicator instance for the given hub credentials.

## Properties

| Property | Type | Description |
|----------|------|-------------|
| IsConnected | bool | `true` if currently connected to the hub |
| _host | string | (private) The hub hostname |
| _username | string | (private) The hub username |
| _password | string | (private) The hub password |

## Methods

### ConnectAsync()

```csharp
public async Task<bool> ConnectAsync();
```

Establishes an SSH connection to the Cluster Hub and sets up SFTP. Returns `true` on success.

### DisconnectAsync()

```csharp
public async Task DisconnectAsync();
```

Closes all node tunnels, SFTP, and SSH connections.

### DisposeAsync()

```csharp
public async ValueTask DisposeAsync();
```

Async-disposes the communicator, triggering disconnect.

### CheckConnectionAsync()

```csharp
public async Task CheckConnectionAsync();
```

Ensures a connection is established before proceeding.

### checkSSHDeviceAsync()

```csharp
public async Task<SSHCheckResult> checkSSHDeviceAsync(bool verbose);
```

Pings the hub and returns timing details.

## Public API

### Hub File Methods

#### HubFileExists()

```csharp
public async Task<bool> HubFileExists(string hubFilePath, bool verbose = false);
```

Checks if a file exists on the hub.

#### HubFileLastModified()

```csharp
public async Task<DateTime> HubFileLastModified(string hubFilePath, bool verbose = false);
```

Gets the last-modified timestamp of a hub file.

#### GetListOfHubFiles()

```csharp
public async Task<List<LinuxFileInfo>> GetListOfHubFiles(string directory, string fileExtension, bool verbose = false);
```

Lists files matching an extension in a remote directory.

#### DeleteHubFile()

```csharp
public async Task<bool> DeleteHubFile(string hubFilePath, bool verbose = true);
```

Deletes a file on the hub via SFTP.

#### MoveHubFile()

```csharp
public async Task<bool> MoveHubFile(string currentFilePath, string newFilePath, bool verbose = false);
```

Renames a file on the hub.

### Node File Methods

#### NodeFileExists()

```csharp
public async Task<bool> NodeFileExists(string nodeFilePath, string host, bool verbose = false);
```

Checks if a file exists on a specific node (via SSH tunnel).

#### NodeFileLastModified()

```csharp
public async Task<DateTime?> NodeFileLastModified(string nodeFilePath, string host, bool verbose = false);
```

Gets the last-modified timestamp on a node.

#### GetListOfNodeFiles()

```csharp
public async Task<List<LinuxFileInfo>> GetListOfNodeFiles(string directory, string fileExtension, string host, string username, bool verbose = false);
```

Lists files matching an extension on a specific node.

#### DeleteNodeFile()

```csharp
public async Task<bool> DeleteNodeFile(string nodeFilePath, string host, bool verbose = false);
```

Deletes a file on a specific node via the hub SSH tunnel.

#### MoveNodeFile()

```csharp
public async Task<bool> MoveNodeFile(string currentFilePath, string newFilePath, string host, string username, bool verbose = false);
```

Renames a file on a specific node via the hub SSH tunnel.

### Copy Operations

#### CopyHubToNode()

```csharp
public async Task<bool> CopyHubToNode(string hubFilePath, string nodeFilePath, string host, string username, bool verbose = false);
```

Copies a file from the hub to a node using `scp`.

#### CopyNodeToHub()

```csharp
public async Task<bool> CopyNodeToHub(string nodeFilePath, string hubFilePath, string host, string username, bool verbose = false);
```

Copies a file from a node back to the hub using `scp`.

### Asynchronous SFTP File Transfer Methods

#### PCtoHubAsync()

Overloads accept either:
- A single path string with optional directory and command type
- A `ClusterFileIOCommand` directly
- A list of paths or commands for batch operations

Returns a `DownloadResult` indicating success/failure.

## Usage Example

```csharp
var comm = new Communicator("hub.example.com", "user", "pass");

try
{
    await comm.ConnectAsync();

    // Get list of Python files on the hub
    var files = await comm.GetListOfHubFiles("/data", "py");
    foreach (var f in files)
        Console.WriteLine($"{f.Name} ({f.Size} bytes)");

    // Download a single file
    var result = await comm.PCtoHubAsync("/remote/file.txt", "/local/dir", ClusterFileIOCommandType.Download);
    if (result.Success)
        Console.WriteLine($"Downloaded: {result.Command.LocalPath}");

    // Copy from node to hub
    bool copied = await comm.CopyNodeToHub(
        "/node/path/data.csv",
        "/hub/shared/data.csv",
        "node-01",
        "alice"
    );

    await comm.DisposeAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Thread Safety

The communicator uses a `SemaphoreSlim` to ensure only one concurrent connection attempt. File operations are not thread-safe; invoke them from a single thread or protect with your own synchronization.

## Related Types

- [DownloadResult](./DownloadResult.md)
- [ClusterFileIOCommand](./ClusterFileIOCommand.md)
- [LinuxFileInfo](./LinuxFileInfo.md)
- [SSHCheckResult](#sshcheckresult)
