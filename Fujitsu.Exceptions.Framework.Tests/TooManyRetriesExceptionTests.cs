using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.Exceptions.Framework.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TooManyRetriesExceptionTests
    {
        [TestMethod]
        public void TooManyRetriesException_DefaultConstructor_HasStandardMessage()
        {
            #region Arrange

            const string message = "A retryable condition did not complete within the maximum number of retries.";

            #endregion

            #region Act

            var exception = new TooManyRetriesException();

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);

            #endregion
        }

        [TestMethod]
        public void TooManyRetriesException_InnerExceptionConstructor_HasStandardMessage()
        {
            #region Arrange

            const string message = "A retryable condition did not complete within the maximum number of retries.";

            #endregion

            #region Act

            var exception = new TooManyRetriesException(new Exception());

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);

            #endregion
        }

        [TestMethod]
        public void TooManyRetriesException_MessageConstructor_HasCorrectMessage()
        {
            #region Arrange

            const string message = "Hello, World!";

            #endregion

            #region Act

            var exception = new TooManyRetriesException(message);

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);

            #endregion
        }

        [TestMethod]
        public void TooManyRetriesException_MessageInnerExceptionConstructor_HasCorrectValues()
        {
            #region Arrange

            const string message = "Hello, World!";

            var mockException = new Mock<Exception>();

            #endregion

            #region Act

            var exception = new TooManyRetriesException(message, mockException.Object);

            #endregion

            #region Assert

            Assert.AreEqual(message, exception.Message);
            Assert.AreSame(mockException.Object, exception.InnerException);

            #endregion
        }
    }
}
