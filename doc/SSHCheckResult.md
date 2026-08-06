# SSHCheckResult

## Namespace

jCommunicator

## Purpose

Returned by `checkSSHDeviceAsync()` to provide timing and success information about a connection attempt.

## Constructors

```csharp
public SSHCheckResult(bool success, Exception? exception, long checkTime);
```

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Success | bool | Whether the connection succeeded |
| Exception | Exception? | Any exception that occurred (null if successful) |
| checkTimespan | long | Connection attempt duration in milliseconds |

## Usage Example

```csharp
var result = await communicator.checkSSHDeviceAsync(verbose: true);

if (result.Success)
{
    Console.WriteLine($"Connected in {result.checkTimespan} ms");
}
else
{
    Console.WriteLine($"Failed: {result.Exception?.Message ?? "Unknown error"} ({result.checkTimespan} ms)");
}
```

## Related Types

- [Communicator](./Communicator.md)
