using System;

namespace Fujitsu.Exceptions.Framework
{
    [Serializable]
    public class RetryableErrorException : Exception
    {
        public static int DefaultRetryInterval = 5000;
        public static int DefaultRetryAttempts = 1;

        /// <summary>
        /// Initializes a new instance of the RetryableErrorException class.
        /// </summary>
        public RetryableErrorException()
            : base("A retryable error occurred")
        {
        }

        /// <summary>
        /// Initializes a new instance of the RetryableErrorException class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RetryableErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///    Initializes a new instance of the RetryableErrorException class with a specified
        ///     error message and a reference to the inner exception that is the cause of
        ///     this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RetryableErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Indicates if the operation causing the exception should be retried. By default,
        /// the operation should always be retried. However, derived exception classes can
        /// override this behaviour to provide more detailed analysis of the error.
        /// </summary>
        public virtual bool ShouldRetry
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the amount of time that the exception handler should wait (in milliseconds)
        /// before retrying the failed operation. By default, this is a fixed value but a derived
        /// exception class can implement a different algorithm, potentially based on the number
        /// of failed attempts.
        /// </summary>
        /// <param name="attempt">The number of rety attempts so far (including the current attempt, so starts at 1).</param>
        /// <returns>The number of milliseconds that the logic should wait before retrying the operation.</returns>
        public virtual int CalculateRetryInterval(int attempt)
        {
            return DefaultRetryInterval;
        }

        /// <summary>
        /// Returns the maximum number of attempts that should be made of the failed operation
        /// before aborting.
        /// </summary>
        public virtual int MaxRetries
        {
            get
            {
                return DefaultRetryAttempts;
            }
        }
    }
}
