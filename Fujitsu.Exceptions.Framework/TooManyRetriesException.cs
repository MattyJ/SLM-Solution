using System;

namespace Fujitsu.Exceptions.Framework
{
    [Serializable]
    public class TooManyRetriesException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the TooManyRetriesException class
        /// with a supplied inner exception.
        /// </summary>
        public TooManyRetriesException(Exception innerException)
            : base("A retryable condition did not complete within the maximum number of retries.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TooManyRetriesException class.
        /// </summary>
        public TooManyRetriesException()
            : base("A retryable condition did not complete within the maximum number of retries.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the TooManyRetriesException class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TooManyRetriesException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///    Initializes a new instance of the TooManyRetriesException class with a specified
        ///     error message and a reference to the inner exception that is the cause of
        ///     this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public TooManyRetriesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}