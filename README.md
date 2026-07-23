# jCommunicator

A lightweight .NET library for communicating with Raspberry Pi clusters over SSH.

`jCommunicator` provides a simple API for executing remote commands and transferring files between a Windows desktop, a Raspberry Pi Hub, and one or more Raspberry Pi Nodes. It was originally developed for managing a distributed computer vision cluster but is designed to be reusable for any Hub/Node deployment.

---

## Features

* SSH connection management
* Execute shell commands on the Hub
* Execute commands on Nodes through the Hub
* SCP file transfers between PC and Hub
* SFTP file transfers between PC and Nodes
* Copy files between Hub and Nodes
* Remote file management

  * Check existence
  * Retrieve last modified times
  * Move/Rename files
  * Delete files
* Automatic SSH tunnel management for multiple Nodes
* Integrated logging through **mLogger**

---

## Intended Architecture

```text
Desktop PC
    │
    │ SSH / SCP
    ▼
Raspberry Pi Hub
    │
    │ Private Access Point
    ▼
Raspberry Pi Node(s)
```

The desktop application communicates directly with the Hub over SSH. The Hub acts as a gateway to the Nodes, allowing secure command execution and file transfers without exposing the Nodes to the external network.

---

## Requirements

* .NET 8
* SSH enabled on the Hub
* SSH enabled on each Node
* Shared SSH credentials for the cluster

### Dependencies

* `Renci.SshNet`
* `mLogger`

---

## Installation

Install the required NuGet packages:

```bash
dotnet add package SSH.NET
```

and reference **mLogger** in your project.

---

## Quick Start

```csharp
using var communicator = new Communicator(
    "192.168.1.50",
    "pi",
    "password");

communicator.Connect();

// Register a Node
communicator.AddNodeSFTP(
    "node01",
    "pi");

// Execute a command on the Hub
string uptime =
    communicator.ExecuteHubCommand("uptime");

// Upload a file to the Hub
communicator.CopyPCtoHub(
    @"C:\Data\config.json",
    "/home/pi/config.json");

// Copy the file from the Hub to the Node
communicator.CopyHubToNode(
    "/home/pi/config.json",
    "/home/pi/config.json",
    "node01",
    "pi");

// Execute a command on the Node
communicator.ExecuteNodeCommand(
    "sudo systemctl restart myservice",
    "node01",
    "pi");
```

---

## API Overview

### Connection

```csharp
Connect()
Disconnect()
AddNodeSFTP()
PingNode()
```

Creates and manages SSH connections, SFTP sessions, and SSH tunnels to each registered Node.

---

### Command Execution

```csharp
ExecuteHubCommand()
ExecuteNodeCommand()
```

Execute arbitrary shell commands on either the Hub or a registered Node.

---

### Hub File Operations

```csharp
HubFileExists()
HubFileLastModified()
MoveHubFile()
DeleteHubFile()
```

Perform common file operations directly on the Hub.

---

### Node File Operations

```csharp
NodeFileExists()
NodeFileLastModified()
MoveNodeFile()
DeleteNodeFile()
```

Perform the same operations on any registered Node through its SFTP tunnel.

---

### File Transfers

```csharp
CopyPCtoHub()
CopyHubToPC()
CopyHubToNode()
CopyPCtoNode()
CopyNodeToPC()
```

Provides high-level methods for moving files throughout the cluster without manually managing SSH sessions.

---

## Logging

`jCommunicator` uses **mLogger** for all logging output.

Logging includes:

* Connection events
* Command execution
* File transfers
* Errors
* Diagnostic information

Applications using `jCommunicator` can configure `mLogger` with any supported sink.

---

## Current Status

This repository represents the initial standalone extraction of the communication layer from a larger Raspberry Pi cluster management application.

Future work includes:

* Improved asynchronous APIs
* Additional cancellation support
* Separation of cluster-specific functionality into higher-level libraries
* Continued API refinement

---

## License

This project is licensed under the MIT License.
