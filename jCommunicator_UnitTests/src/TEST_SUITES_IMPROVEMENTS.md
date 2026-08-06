# Test Suite Improvement Recommendations

## Current State

The project has **strong unit tests** for `ClusterFileIOCommand` and `DownloadResult`, but **very limited integration testing** despite heavy reliance on external SSH infrastructure. The test suite covers ~85% of public API methods but lacks meaningful assertions in many cases.

---

## Key Gaps & Issues

### 1. Stub Tests Mask Missing Functionality
`CommunicatorIntegrationTests.cs` contains **30+ test methods marked with `// TODO`**—they have no implementation, only type checks. This creates a false sense of coverage. Consider either:
- Removing these stubs to avoid misleading readers
- Implementing them (requires SSH infrastructure)

### 2. No Tests for Core SFTP Operations
Several critical methods are untested:
- `RebuildNodeTunnelsAsync` — private but central to node management
- `checkSSHDeviceAsync` — exposed via public API in integration tests but no dedicated validation
- Batch download operations (`PCtoHubAsync(List<ClusterFileIOCommand>)`) only have type checks

### 3. Edge Cases Missing
- **Race conditions**: `_connectLock` guards async methods; no tests verify behavior under concurrent calls
- **Partial failures**: If `AddNodeTunnelAsync` fails mid-way, `_nodeConnections` may be left in inconsistent state
- **File permission errors**: SFTP operations can fail due to permissions but none are tested
- **Large file transfers**: No stress or timeout testing

### 4. Setup/Teardown Fragility
- `CommunicatorTestBase` hardcodes credentials and assumes SSH services are running
- Tests create temp files but don't verify cleanup (especially in failure paths)
- `_logSink` is set up once per base class; if a test fails mid-run, subsequent tests may see partial logs

### 5. Flakiness Risks
- `PingNodeAsync` uses `echo connected || echo disconnected`; parsing assumes exact output format
- `GetListOfHubFiles` regex depends on `ls -l --full-time` output stability across systems
- Node tunnel port allocation is deterministic (`2200 + count`) but no tests verify port reuse after disconnect

---

## Recommendations

### Immediate (High Impact, Low Effort)
1. **Remove or implement** all 30+ TODO stubs — they're misleading
2. Add tests for `RebuildNodeTunnelsAsync` with mock SSH client
3. Cover SFTP permission errors and partial download failures
4. Verify temp file cleanup in Dispose paths

### Medium Term
5. Introduce mocking for SSH/SFTP clients to test error scenarios without infrastructure
6. Add concurrency stress tests for `_connectLock`
7. Expand `DownloadResult_Success` property tests with real failure scenarios

### Longer Term
8. Consider a test harness that spins up local SSH daemons (e.g., via Docker) for fully isolated integration tests
9. Add performance benchmarks for large file transfers and batch operations

---

**Bottom line:** The unit test foundation is solid, but the integration suite needs substantial work to reflect actual behavior and catch edge cases. Focus first on removing stubs and covering missing SFTP error paths.
