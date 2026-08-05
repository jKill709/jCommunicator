# ClusterFileIOCommand Struct

The `ClusterFileIOCommand` struct defines the parameters for file I/O operations on the cluster hub or individual nodes. It encapsulates both path information and operation type, providing a flexible way to configure transfer commands.

## Overview

```csharp
public struct ClusterFileIOCommand
{
    public string RemoteDir { get; set; } = "";
    public string RemoteFileName { get; set; } = "";
    public string RemotePath => RemoteDir + '/' + RemoteFileName;

    public string LocalDir { get; set; } = "";
    public string LocalFileName { get; set; } = "";
    public string LocalPath => Path.Combine(LocalDir, LocalFileName);

    public ClusterFileIOCommandType Type { get; set; }

    public bool checkExists { get; set; }
    public bool getAttributes { get; set; }
    public bool deleteAfter { get; set; }
    public bool checkSize { get; set; }

    public static ClusterFileIOCommand Create(string remotePath, string localPath, 
        ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, 
        bool deleteAfter = false, bool checkSize = false)
    {
        // Factory method for creating commands from full paths
    }

    public static ClusterFileIOCommand Create(string fileName, string remoteDir, string localDir, 
        ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, 
        bool deleteAfter = false, bool checkSize = false)
    {
        // Factory method for creating commands from components
    }

    public static ClusterFileIOCommand Create(string remotePath, string localPath, 
        ClusterFileIOCommand other)
    {
        // Copy constructor
    }
}
```

## Purpose

The `ClusterFileIOCommand` struct serves as the primary configuration object for file operations. It separates concerns between path handling (which requires careful directory/filename splitting) and operation type, making it easier to reuse command configurations while modifying paths.

## Properties

### RemoteDir

The directory component of the remote path, normalized to use forward slashes.

**Type:** `string`

**Default:** Empty string

**Access:** Read-write

### RemoteFileName

The filename component of the remote path.

**Type:** `string`

**Default:** Empty string

**Access:** Read-write

### RemotePath

Computed property returning the full remote path as `RemoteDir + '/' + RemoteFileName`.

**Type:** `string`

**Access:** Read-only

### LocalDir

The directory component of the local path.

**Type:** `string`

**Default:** Empty string

**Access:** Read-write

### LocalFileName

The filename component of the local path.

**Type:** `string`

**Default:** Empty string

**Access:** Read-write

### LocalPath

Computed property returning the full local path using `Path.Combine(LocalDir, LocalFileName)`.

**Type:** `string`

**Access:** Read-only

### Type

The operation type: Exists, Attributes, Download, Upload, Move, or Delete.

**Type:** `ClusterFileIOCommandType`

**Access:** Read-write

### checkExists

Indicates whether to verify the remote file exists before proceeding with the operation.

**Type:** `bool`

**Default:** false

**Access:** Read-write

### getAttributes

Indicates whether to retrieve and store file attributes (size, timestamp).

**Type:** `bool`

**Default:** false

**Access:** Read-write

### deleteAfter

Indicates whether to delete the remote file after a successful download.

**Type:** `bool`

**Default:** false

**Access:** Read-write

### checkSize

Reserved for future use; not currently implemented.

**Type:** `bool`

**Default:** false

**Access:** Read-write

## Constructors

### ClusterFileIOCommand(string remotePath, string localPath, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool deleteAfter = false, bool checkSize = false)

Creates a command from full remote and local paths. The path components are automatically extracted using `Path.GetFileName` and `Path.GetDirectoryName`.

**Parameters:**
- `remotePath` - Full path on the server (required, cannot be null or empty)
- `localPath` - Local destination path (required, cannot be null or empty)
- `type` - The operation type
- `checkExists` - Whether to verify remote file exists
- `getAttributes` - Whether to retrieve file attributes
- `deleteAfter` - Whether to delete after download

**Example:**
```csharp
var command = ClusterFileIOCommand.Create(
    "/data/output.txt", 
    "/tmp/hub_data/file.txt", 
    ClusterFileIOCommandType.Download,
    checkExists: true
);
```

### ClusterFileIOCommand(string fileName, string remoteDir, string localDir, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool deleteAfter = false, bool checkSize = false)

Creates a command from filename and directory components. This is useful for constructing paths programmatically.

**Parameters:**
- `fileName` - The filename (required, cannot be null or empty)
- `remoteDir` - Remote directory path (required, cannot be null or empty)
- `localDir` - Local directory path
- `type` - The operation type
- `checkExists` - Whether to verify remote file exists
- `getAttributes` - Whether to retrieve file attributes
- `deleteAfter` - Whether to delete after download

**Example:**
```csharp
var command = ClusterFileIOCommand.Create(
    "output.txt", 
    "/data", 
    "/tmp/hub_data", 
    ClusterFileIOCommandType.Upload,
    getAttributes: true
);
```

### ClusterFileIOCommand(string remotePath, string localPath, ClusterFileIOCommand other)

Copy constructor that creates a new command with the same configuration as an existing one, but with different paths.

**Parameters:**
- `remotePath` - New remote path (required, cannot be null or empty)
- `localPath` - New local path (required, cannot be null or empty)
- `other` - The source command to copy from

**Example:**
```csharp
var original = ClusterFileIOCommand.Create("/data/file.txt", "/tmp/out.txt", ClusterFileIOCommandType.Download);
var modified = new ClusterFileIOCommand("/data/backup/file.txt", "/tmp/backup.txt", original);
```

## Usage Examples

### Simple Download with Validation
```csharp
var command = ClusterFileIOCommand.Create(
    "/data/report.pdf", 
    "/local/reports/", 
    ClusterFileIOCommandType.Download,
    checkExists: true,
    getAttributes: true
);

var result = await communicator.PCtoHubAsync(command);
```

### Upload with Metadata
```csharp
var command = ClusterFileIOCommand.Create(
    "data.csv", 
    "/data/input/", 
    "/local/input/data.csv", 
    ClusterFileIOCommandType.Upload,
    getAttributes: true
);

var result = await communicator.PCtoHubAsync(command);
```

### Move Operation
```csharp
var command = ClusterFileIOCommand.Create(
    "/data/archive/old.txt", 
    "/data/archive/new.txt", 
    ClusterFileIOCommandType.Move
);

var result = await communicator.PCtoHubAsync(command);
```

## Notes

- Remote paths are always normalized to use forward slashes (`/`) for SFTP compatibility
- Local paths preserve the platform-specific separator (backslash on Windows, forward slash on Unix)
- The struct is immutable after construction except for properties explicitly set
- Empty strings for `RemoteDir` or `LocalDir` result in root directory (`"/"`) when combined with a filename
