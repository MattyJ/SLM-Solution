using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Fujitsu.Exceptions.Framework.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RetryableErrorExceptionTests
    {
        [TestMethod]
        public void RetryableErrorException_DefaultConstructor_HasStandardMessage()
        {
            #region Arrange

            const string message = "A retryable error occurred";

            #endregion

            #region Act

            var exception = new RetryableErrorException();

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);

            #endregion
        }

        [TestMethod]
        public void RetryableErrorException_MessageConstructor_HasCorrectMessage()
        {
            #region Arrange

            const string message = "Hello, World!";

            #endregion

            #region Act

            var exception = new RetryableErrorException(message);

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);

            #endregion
        }

        [TestMethod]
        public void RetryableErrorException_MessageInnerExceptionConstructor_HasCorrectValues()
        {
            #region Arrange

            const string message = "Hello, World!";

            var mockException = new Mock<Exception>();

            #endregion

            #region Act

            var exception = new RetryableErrorException(message, mockException.Object);

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);
            Assert.AreSame(mockException.Object, exception.InnerException);

            #endregion
        }

        [TestMethod]
        public void RetryableErrorException_ShouldRetry()
        {
            #region Arrange

            #endregion

            #region Act

            var exception = new RetryableErrorException();

            #endregion

            #region Assert

            Assert.IsTrue(exception.ShouldRetry);

            #endregion
        }

        [TestMethod]
        public void RetryableErrorException_RetryInterval_AlwaysDefault()
        {
            #region Arrange

            var retryInterval = RetryableErrorException.DefaultRetryInterval;

            #endregion

            #region Act

            // Standard RetryableErrorException should not use any kind of 'fallback', it should always return the default interval

            var exception = new RetryableErrorException();
            var interval1 = exception.CalculateRetryInterval(1);
            var interval2 = exception.CalculateRetryInterval(2);
            var interval3 = exception.CalculateRetryInterval(3);

            #endregion

            #region Assert

            Assert.AreEqual(retryInterval, interval1);
            Assert.AreEqual(retryInterval, interval2);
            Assert.AreEqual(retryInterval, interval3);

            #endregion
        }

        [TestMethod]
        public void RetryableErrorException_MaxRetries_IsDefault()
        {
            #region Arrange

            var attempts = RetryableErrorException.DefaultRetryAttempts;

            #endregion

            #region Act

            // Standard RetryableErrorException should not use any kind of 'fallback', it should always return the default interval

            var exception = new RetryableErrorException();

            #endregion

            #region Assert

            Assert.AreEqual(attempts, exception.MaxRetries);

            #endregion
        }
    }
}
