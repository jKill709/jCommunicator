# jCommunicator

SSH/SFTP client library for connecting to a Cluster Hub and performing file operations on the hub and individual nodes.

## Features

- Connect to a Cluster Hub via SSH
- Tunnel SFTP connections to individual nodes through the hub
- File listing with full metadata (permissions, owner, size, timestamps)
- Download/upload/move/delete files via SFTP
- Copy between hub and nodes using `scp`
- Asynchronous operations throughout

## Requirements

- .NET 8.0 SDK
- SSH server on the cluster hub
- Password authentication configured for hub and nodes

## Installation

Add to your project:

```bash
dotnet add package Renci.SshNet.Async --version 1.4.0
dotnet add package SSH.NET --version 2025.0.0
```

The library also depends on `mLogger`, which must be built separately and referenced manually (see Project Structure).

## Quick Start

```csharp
using jCommunicator;

var comm = new Communicator("hub.example.com", "user", "pass");

try
{
    await comm.ConnectAsync();

    // List Python files on the hub
    var files = await comm.GetListOfHubFiles("/data", "py");
    foreach (var f in files)
        Console.WriteLine($"{f.Name} ({f.Size} bytes)");

    // Download a file
    var result = await comm.PCtoHubAsync(
        "/remote/file.txt",
        "/local/dir",
        ClusterFileIOCommandType.Download
    );

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

## Basic Usage Example

See the full example in `Quick Start` above, or refer to the [API Documentation](#api-documentation).

## Project Structure

```
jCommunicator/
├── jCommunicator.slnx
├── doc/                    # Generated API documentation
│   ├── Communicator.md
│   ├── ClusterFileIOCommand.md
│   ├── DownloadResult.md
│   ├── index.md
│   ├── LinuxFileInfo.md
│   └── SSHCheckResult.md
├── jCommunicator/          # Production library
│   └── src/
│       ├── Communicator.cs
│       ├── DownloadResult.cs
│       └── LinuxFileInfo.cs
├── mLogger/                # Logging dependency (built separately)
└── jCommunicator_UnitTests/
    └── jCommunicator_Tests.csproj
```

## API Reference

- [Communicator](./doc/Communicator.md)
- [DownloadResult](./doc/DownloadResult.md)
- [ClusterFileIOCommand](./doc/ClusterFileIOCommand.md)
- [LinuxFileInfo](./doc/LinuxFileInfo.md)
- [SSHCheckResult](./doc/SSHCheckResult.md)

## Building

From the repository root:

```bash
dotnet build jCommunicator/jCommunicator.slnx
```

The `mLogger` dependency must be built first:

```bash
cd mLogger && dotnet build && cd ..
```

## Testing

Run from the repository root:

```bash
dotnet test jCommunicator_UnitTests/jCommunicator_Tests.csproj
```

## Contributing

Contributions are welcome. Please ensure tests pass and follow the existing code style.

## License

[MIT](LICENSE)
