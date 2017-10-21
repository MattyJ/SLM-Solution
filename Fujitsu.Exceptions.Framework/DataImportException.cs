using Fujitsu.Aspose.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fujitsu.Exceptions.Framework
{
    [Serializable]
    public class DataImportException : Exception
    {

        /// <summary>
        /// Initialises a new instance of the DataImportException class
        /// with a supplied inner exception.
        /// </summary>
        public DataImportException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the DataImportException class
        /// with a supplied inner exception.
        /// </summary>
        public DataImportException(Exception innerException)
            : base("Error importing spreadsheet data", innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of the DataImportException class
        /// with a supplied message and inner exception.
        /// </summary>
        public DataImportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DataImportException(string message, IEnumerable<ImportValidationResult> validationResult)
            : base(message + " : " + ConvertValidationResultToString(validationResult))
        {
        }


        static string ConvertValidationResultToString(IEnumerable<ImportValidationResult> validationResult)
        {
            var errors = new StringBuilder();

            if (validationResult != null)
            {
                foreach (var result in validationResult)
                {
                    if (errors.Length > 0)
                    {
                        errors.AppendLine();
                    }

                    var msg = result.Reference + " : " + result.ValidationResult.ErrorMessage;

                    if ((result.ValidationResult.MemberNames != null) && (result.ValidationResult.MemberNames.Any()))
                    {
                        msg = msg + " - " + string.Join(", ", result.ValidationResult.MemberNames);
                    }

                    errors.Append(msg);
                }
            }

            return errors.ToString();
        }



    }
}
