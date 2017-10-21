using System;
using System.Data.Entity.Validation;
using System.Linq;
using Fujitsu.Exceptions.Framework.Interfaces;

namespace Fujitsu.Exceptions.Framework.ExceptionFormatters
{
    public class DbEntityValidationExceptionFormatter : IExceptionFormatter
    {
        public string ToString(Exception e)
        {
            var dbEntityValidationException = (DbEntityValidationException)e;

            var message = string.Concat(Environment.NewLine, e.Message, Environment.NewLine, e.StackTrace, Environment.NewLine, "EntityValidationErrors are:");

            message = dbEntityValidationException.EntityValidationErrors.Aggregate(
                message,
                (current1, validationErrors) =>
                validationErrors.ValidationErrors.Aggregate(
                    current1,
                    (current, validationError) =>
                    string.Concat(current, string.Format("\n  Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage))));

            message = string.Concat(message, e.InnerException.Flatten());

            return message;
        }
    }
}