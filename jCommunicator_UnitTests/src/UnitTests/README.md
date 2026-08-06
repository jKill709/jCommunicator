# Unit Tests

This directory contains unit tests that verify the correctness of individual classes and methods in isolation.

## Test Categories

### ClusterFileIOCommandConstructors

Tests for the `ClusterFileIOCommand` constructor:
- Clone constructors - verify independent cloning behavior
- Path parsing and normalization - test path handling
- Normal operation - test all command types (Exists, Attributes, Download, Upload, Move, Delete)
- Bad inputs - test error handling with invalid arguments

### DownloadResult_Constructors

Tests for the `DownloadResult` class:
- Constructor initialization - verify all properties are set correctly
- Command reference preservation - ensure command object is maintained
- Flag preservation - verify checkExists, getAttributes, deleteAfter, checkSize flags

