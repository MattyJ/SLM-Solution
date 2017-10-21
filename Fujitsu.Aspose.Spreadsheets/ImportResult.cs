using System.Collections.Generic;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class ImportResult<T>
    {
        public ImportResult()
        {

            ValidationResults = new List<ImportValidationResult>();
        }

        public T Result { get; set; }

        public List<ImportValidationResult> ValidationResults { get; set; }
    }
}