# DownloadResult Class

The `DownloadResult` class encapsulates the outcome of file transfer operations on the cluster hub or nodes. It provides comprehensive information about the transfer status, file attributes, and any errors encountered.

## Overview

```csharp
public class DownloadResult
{
    public ClusterFileIOCommand Command { get; set; }
    public SftpFileAttributes? Attributes { get; set; }
    public bool FileExists { get; set; }
    public bool MainProcedureSucceeded { get; set; }
    public bool DeleteSucceeded { get; set; }
    public Exception? Exception { get; set; }
    public bool Success => FileExists && MainProcedureSucceeded && DeleteSucceeded && Exception == null;
}
```

## Purpose

The `DownloadResult` class is used throughout the file transfer operations to report detailed information about each transfer attempt. It allows callers to inspect individual transfer results and handle errors appropriately.

## Properties

### Command

The underlying command that triggered this result.

**Type:** `ClusterFileIOCommand`

**Access:** Read-write (though typically set during construction)

### Attributes

File attributes retrieved from the server, including size and modification time.

**Type:** `SftpFileAttributes?`

**Access:** Read-only

Contains information such as:
- Size of the file in bytes
- Last write time
- Permissions
- Other SFTP metadata

### FileExists

Indicates whether the remote file was found to exist before the transfer.

**Type:** `bool`

**Access:** Read-write (set during execution)

### MainProcedureSucceeded

Indicates whether the primary transfer operation (download, upload, or move) completed successfully.

**Type:** `bool`

**Access:** Read-write (set during execution)

### DeleteSucceeded

Indicates whether a delete operation (if requested) completed successfully.

**Type:** `bool`

**Access:** Read-write (set during execution)

### Exception

Any exception that occurred during the transfer operation.

**Type:** `Exception?`

**Access:** Read-write (set during execution)

### Success

Computed property indicating overall success of the operation. Returns true only if:
- The file was found (`FileExists`)
- The main procedure succeeded (`MainProcedureSucceeded`)
- Delete succeeded (if applicable, `DeleteSucceeded`)
- No exception occurred (`Exception == null`)

**Type:** `bool`

## Constructors

### DownloadResult(ClusterFileIOCommand command)

Creates a new result from an existing command.

**Parameters:**
- `command` - The command configuration to use

**Example:**
```csharp
var result = new DownloadResult(command);
```

### DownloadResult(string remotePath, string localPath, ClusterFileIOCommand command)

Creates a new result with explicit remote and local paths.

**Parameters:**
- `remotePath` - The path on the server
- `localPath` - The destination path locally
- `command` - The base command configuration

**Example:**
```csharp
var result = new DownloadResult("/data/file.txt", "/tmp/output/", command);
```

### DownloadResult(string fileName, string remoteDir, string localDir, ClusterFileIOCommand command)

Creates a new result with explicit directory and filename components.

**Parameters:**
- `fileName` - The name of the file
- `remoteDir` - The remote directory path
- `localDir` - The local directory path
- `command` - The base command configuration

**Example:**
```csharp
var result = new DownloadResult("file.txt", "/data", "/tmp/output", command);
```

## ToString() Method

Returns a formatted string representation of the transfer result, suitable for logging or debugging.

**Format:**
```
Remote: {Command.RemotePath}
Local : {Command.LocalPath}
Exists: {FileExists}
Size  : {Attributes?.Size.ToString() ?? "N/A"}
Date  : {Attributes?.LastWriteTime.ToString() ?? "N/A"}
Downloaded: {MainProcedureSucceeded}
Deleted   : {DeleteSucceeded}
Success   : {Success}
{Exception.Message if present}
```

**Example output:**
```
Remote: /data/output.txt
Local : /tmp/hub_data/
Exists: True
Size  : 1024
Date  : 8/5/2026 3:45:12 PM
Downloaded: True
Deleted   : True
Success   : True
```

## Usage Example

```csharp
var result = await communicator.PCtoHubAsync("file.txt", "/local/dir", command);

if (result.Success)
{
    Console.WriteLine($"Successfully downloaded {result.Command.RemotePath}");
}
else
{
    if (result.Exception != null)
    {
        Console.WriteLine($"Error: {result.Exception.Message}");
    }
    else
    {
        Console.WriteLine($"File not found or other issue at {result.Command.RemotePath}");
    }
}
```

## Notes

- The `ToString()` method provides a convenient way to log transfer results
- Individual boolean properties allow fine-grained error handling
- The `Success` property provides a quick check for overall operation success
