# ClusterFileIOCommandType Enum

The `ClusterFileIOCommandType` enum specifies the type of file operation to perform on the cluster hub or nodes.

## Values

| Member | Description |
|--------|-------------|
| **Exists** | Check if a file exists at the specified path |
| **Attributes** | Retrieve file metadata (size, timestamp, permissions) |
| **Download** | Download a file from remote to local location |
| **Upload** | Upload a file from local to remote location |
| **Move** | Move/rename a file on the server |
| **Delete** | Delete a file from the server |

## Usage

These types are used with `ClusterFileIOCommand` to configure operations:

```csharp
// Check if file exists
var command = ClusterFileIOCommand.Create(
    "/data/report.txt", 
    "", 
    ClusterFileIOCommandType.Exists,
    checkExists: true
);

// Download with metadata
var downloadCommand = ClusterFileIOCommand.Create(
    "/data/output.txt", 
    "/local/output/", 
    ClusterFileIOCommandType.Download,
    checkExists: true,
    getAttributes: true,
    deleteAfter: true
);

// Upload file
var uploadCommand = ClusterFileIOCommand.Create(
    "input.csv", 
    "/data/input/", 
    "/local/input.csv", 
    ClusterFileIOCommandType.Upload,
    getAttributes: true
);

// Move/rename file
var moveCommand = ClusterFileIOCommand.Create(
    "/data/archive/old.txt", 
    "/data/archive/new.txt", 
    ClusterFileIOCommandType.Move
);

// Delete file
var deleteCommand = ClusterFileIOCommand.Create(
    "/data/temp.txt", 
    "", 
    ClusterFileIOCommandType.Delete,
    deleteAfter: true
);
```

## Notes

- The enum is used internally by the `DownloadResult` class to determine which operation to perform
- Each command type may have different optional flags (e.g., `checkExists` for Exists, `getAttributes` for Attributes)
- Delete operations typically set `deleteAfter: true` since the delete happens after the main procedure
