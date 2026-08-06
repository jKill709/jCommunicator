# DownloadResult

## Namespace

jCommunicator

## Purpose

Encapsulates the result of a file operation (download, upload, move, delete) performed via SFTP.

## Constructors

```csharp
public DownloadResult(ClusterFileIOCommand command);
public DownloadResult(string remotePath, string localPath, ClusterFileIOCommand command);
public DownloadResult(string fileName, string remoteDir, string localDir, ClusterFileIOCommand command);
```

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Command | ClusterFileIOCommand | The original file I/O command |
| Attributes | SftpFileAttributes? | Remote file attributes (size, timestamp) |
| FileExists | bool | Whether the remote file exists |
| MainProcedureSucceeded | bool | Whether the primary operation succeeded |
| DeleteSucceeded | bool | Whether deletion (if requested) succeeded |
| Exception | Exception? | Any exception that occurred |

## Methods

### Success

```csharp
public bool Success => FileExists && MainProcedureSucceeded && DeleteSucceeded && Exception == null;
```

Returns `true` only if all operations completed successfully.

### ToString()

```csharp
public override string ToString()
{
    return $"Remote: {Command.RemotePath}\nLocal : {Command.LocalPath}\nExists: {FileExists}\nSize  : {Attributes?.Size.ToString() ?? "N/A"}\nDate  : {Attributes?.LastWriteTime.ToString() ?? "N/A"}\nDownloaded: {MainProcedureSucceeded}\nDeleted   : {DeleteSucceeded}\nSuccess   : {Success}\n{(Exception != null ? Exception.Message : "")}";
}
```

Returns a detailed string representation of the result.

## Usage Example

```csharp
var result = await communicator.PCtoHubAsync("myfile.txt", "/local/dir", ClusterFileIOCommandType.Download);

if (result.Success)
{
    Console.WriteLine($"Downloaded {result.Command.LocalPath} ({result.Attributes.Size} bytes)");
}
else if (result.Exception != null)
{
    Console.WriteLine($"Error: {result.Exception.Message}");
}
```

## Related Types

- [ClusterFileIOCommand](#clusterfileiocommand)
- [Communicator](./Communicator.md)
