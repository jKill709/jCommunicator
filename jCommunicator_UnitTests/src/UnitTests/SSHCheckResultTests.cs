using System;
using Xunit;

namespace jCommunicator.Tests.Unit
{
    public class SSHCheckResultTests
    {
        [Fact]
        public void Constructor_Success_SetsProperties()
        {
            // Arrange
            long elapsed = 123;

            // Act
            SSHCheckResult result = new SSHCheckResult(
                success: true,
                exception: null,
                checkTime: elapsed);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.Equal(elapsed, result.checkTimespan);
        }

        [Fact]
        public void Constructor_Failure_SetsProperties()
        {
            // Arrange
            Exception ex = new InvalidOperationException("Connection failed.");
            long elapsed = 123;

            // Act
            SSHCheckResult result = new SSHCheckResult(
                success: false,
                exception: ex,
                checkTime: elapsed);

            // Assert
            Assert.False(result.Success);
            Assert.Same(ex, result.Exception);
            Assert.Equal(elapsed, result.checkTimespan);
        }

        [Fact]
        public void Constructor_StoresExactExceptionInstance()
        {
            // Arrange
            Exception ex = new Exception("Test Exception");

            // Act
            SSHCheckResult result = new SSHCheckResult(
                false,
                ex,
                0);

            // Assert
            Assert.Same(ex, result.Exception);
        }

        [Fact]
        public void Constructor_AllowsZeroElapsedTime()
        {
            // Act
            SSHCheckResult result = new SSHCheckResult(
                true,
                null,
                0);

            // Assert
            Assert.Equal(0, result.checkTimespan);
        }

        [Fact]
        public void Constructor_AllowsLongElapsedTime()
        {
            // Arrange
            long elapsed = 10;

            // Act
            SSHCheckResult result = new SSHCheckResult(
                true,
                null,
                elapsed);

            // Assert
            Assert.Equal(elapsed, result.checkTimespan);
        }

        [Fact]
        public void FailureResult_CanContainException()
        {
            // Arrange
            Exception ex = new TimeoutException();

            // Act
            SSHCheckResult result = new SSHCheckResult(
                false,
                ex,
                5);

            // Assert
            Assert.False(result.Success);
            Assert.IsType<TimeoutException>(result.Exception);
        }
    }
}