# LinuxFileInfo Struct

The `LinuxFileInfo` struct represents file information as returned by `ls -l --full-time` on Linux systems, typically used for displaying detailed file listings from the cluster hub or nodes.

## Overview

```csharp
public struct LinuxFileInfo
{
    public string Permissions { get; init; }
    public int HardLinks { get; init; }
    public string Owner { get; init; }
    public string Group { get; init; }
    public long Size { get; init; }
    public DateTimeOffset LastWriteTime { get; init; }
    public string Name { get; init; }

    public bool IsDirectory => Permissions.StartsWith("d");

    public override string ToString()
    {
        return $"{Name} ({Size} bytes, {LastWriteTime})";
    }
}
```

## Purpose

The `LinuxFileInfo` struct provides a normalized representation of file metadata as returned by Linux `ls -l --full-time`. It allows consistent handling of file information across both hub and node file listings.

## Properties

### Permissions

The permission string from `ls`, typically 10 characters starting with the file type indicator (`-` for regular files, `d` for directories, etc.).

**Type:** `string`

**Access:** Read-only (set during construction)

### HardLinks

The number of hard links to the file.

**Type:** `int`

**Access:** Read-only (set during construction)

### Owner

The username of the file owner.

**Type:** `string`

**Access:** Read-only (set during construction)

### Group

The group name associated with the file.

**Type:** `string`

**Access:** Read-only (set during construction)

### Size

The size of the file in bytes. For directories, this is typically 4096.

**Type:** `long`

**Access:** Read-only (set during construction)

### LastWriteTime

The last modification time of the file, parsed as a `DateTimeOffset` to preserve timezone information.

**Type:** `DateTimeOffset`

**Access:** Read-only (set during construction)

### Name

The filename or directory name.

**Type:** `string`

**Access:** Read-only (set during construction)

### IsDirectory

Computed property indicating whether the entry is a directory (permissions starts with 'd').

**Type:** `bool`

**Access:** Read-only

## ToString() Method

Returns a formatted string with the name, size, and last write time.

**Format:** `{Name} ({Size} bytes, {LastWriteTime})`

**Example output:**
```
/data/report.txt (10240 bytes, 8/5/2026 3:45:12 PM +00:00)
```

## Usage Example

```csharp
// List files on the hub
var hubFiles = await communicator.GetListOfHubFiles("/data", "txt");

foreach (var file in hubFiles)
{
    Console.WriteLine(file.Name);
    Console.WriteLine($"  Size: {file.Size} bytes");
    Console.WriteLine($"  Owner: {file.Owner}");
    Console.WriteLine($"  Last modified: {file.LastWriteTime}");
    Console.WriteLine($"  Is directory: {file.IsDirectory}");
    Console.WriteLine();
}

// List files on a node
var nodeFiles = await communicator.GetListOfNodeFiles("/data", "txt", "node1", "admin");

foreach (var file in nodeFiles)
{
    Console.WriteLine(file.ToString());
}
```

## Notes

- The struct is immutable after construction (all properties are `init`-only)
- Permission strings follow standard Unix format: `-rwxr-xr--` for regular files
- Timezone offset is preserved in the `DateTimeOffset` to ensure accurate timestamps across regions
