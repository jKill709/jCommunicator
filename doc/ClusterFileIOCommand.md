# ClusterFileIOCommand

## Namespace

jCommunicator

## Purpose

Represents a file I/O operation to be performed on the cluster via SFTP.

## Constructors

```csharp
public ClusterFileIOCommand(string remotePath, string localPath, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool deleteAfter = false, bool checkSize = false);
public ClusterFileIOCommand(string remoteDir, string localDir, string fileName, ClusterFileIOCommandType type, bool checkExists = false, bool getAttributes = false, bool deleteAfter = false, bool checkSize = false);
public ClusterFileIOCommand(string remotePath, string localPath, ClusterFileIOCommand other);
public ClusterFileIOCommand(string remoteDir, string localDir, string fileName, ClusterFileIOCommand other);
```

## Properties

| Property | Type | Description |
|----------|------|-------------|
| RemoteDir | string | Remote directory path (empty by default) |
| RemoteFileName | string | Remote filename |
| RemotePath | string | Full remote path (`RemoteDir + '/' + RemoteFileName`) |
| LocalDir | string | Local directory path (empty by default) |
| LocalFileName | string | Local filename |
| LocalPath | string | Full local path |
| Type | ClusterFileIOCommandType | The operation type |
| checkExists | bool | Whether to check if the remote file exists |
| getAttributes | bool | Whether to retrieve file attributes |
| deleteAfter | bool | Whether to delete the remote file after operation |
| checkSize | bool | Whether to validate file size |

## Usage Example

```csharp
var cmd = new ClusterFileIOCommand(
    remotePath: "/remote/path/file.txt",
    localPath: "/local/output/file.txt",
    type: ClusterFileIOCommandType.Download,
    getAttributes: true
);

var result = await communicator.PCtoHubAsync(cmd);
```

## Related Types

- [DownloadResult](./DownloadResult.md)
- [Communicator](./Communicator.md)
