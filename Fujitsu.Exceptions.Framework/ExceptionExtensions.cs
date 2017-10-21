using System;
using Fujitsu.Exceptions.Framework.Interfaces;
using Fujitsu.SLM.Core.Injection;

namespace Fujitsu.Exceptions.Framework
{
    public static class ExceptionExtensions
    {
        public static string Flatten(this Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            string message;
            try
            {
                var objectBuilder = new ObjectBuilder();
                var type = exception.GetType();
                var exceptionFormatter = objectBuilder.Resolve<IExceptionFormatter>(type.Name, true);
                var formattedException = exceptionFormatter.ToString(exception);

                message = string.Concat(type.Namespace, ".", type.Name, ":", formattedException, Environment.NewLine);
            }
            catch (Exception ex)
            {
                message = string.Format("Failed to format exception {0} with exception {1}", exception, ex);
            }
            return message;
        }
    }
}
