using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Diagnostics;
using System.Threading;

namespace Fujitsu.Exceptions.Framework
{
    public static class RetryableOperation
    {
        public static void Invoke(string exceptionPolicy, Action action)
        {
            var retries = 0;
            var succeeded = false;

            while (!succeeded)
            {
                try
                {
                    try
                    {
                        action();
                        succeeded = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Fujitsu.SLM RetryableOperation.cs->Exception: {ex.Flatten()}");
                        if (ExceptionPolicy.HandleException(ex, exceptionPolicy))
                        {
                            throw;
                        }

                        succeeded = true;
                    }
                }
                catch (RetryableErrorException rex)
                {
                    // The exception class itself can decide if the exception really should be retried or if it should not.
                    if (!rex.ShouldRetry || ++retries > rex.MaxRetries)
                    {
                        throw new TooManyRetriesException(rex.Message);
                    }

                    // Consider carefully the effect on the system if threads are sleeping.
                    Thread.Sleep(rex.CalculateRetryInterval(retries));
                }
            }
        }

    }
}
