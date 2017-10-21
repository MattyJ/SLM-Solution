using System;
using Fujitsu.Exceptions.Framework.Interfaces;

namespace Fujitsu.Exceptions.Framework.ExceptionFormatters
{
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        public string ToString(Exception e)
        {
            var message = string.Concat(Environment.NewLine, e.Message, Environment.NewLine, e.StackTrace);
            message = string.Concat(message, e.InnerException.Flatten());
            return message;
        }
    }
}