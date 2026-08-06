# LinuxFileInfo

## Namespace

jCommunicator

## Purpose

Represents file metadata extracted from `ls -l --full-time` output on Linux systems.

## Constructors

None. Use the `init` properties to construct instances.

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Permissions | string | File permission string (e.g., "drwxr-xr-x") |
| HardLinks | int | Number of hard links to the file |
| Owner | string | Username owning the file |
| Group | string | Group name for the file |
| Size | long | File size in bytes |
| LastWriteTime | DateTimeOffset | File modification timestamp |
| Name | string | Filename |

## Methods

### IsDirectory

```csharp
public bool IsDirectory => Permissions.StartsWith("d");
```

Returns `true` if the file is a directory, based on the permission string.

### ToString()

```csharp
public override string ToString()
{
    return $"{Name} ({Size} bytes, {LastWriteTime})";
}
```

Returns a formatted string representation of the file info.

## Usage Example

```csharp
var files = await communicator.GetListOfHubFiles("/path/to/dir", "py");

foreach (var f in files)
{
    if (f.IsDirectory)
        Console.WriteLine($"Directory: {f.Name}");
    else
        Console.WriteLine($"{f.Name} ({f.Size} bytes)");
}
```

## Related Types

- [Communicator](./Communicator.md)
