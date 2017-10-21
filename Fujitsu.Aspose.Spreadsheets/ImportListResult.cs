using System.Collections.Generic;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class ImportListResult<T> 
    {
        public ImportListResult()
        {
            Results = new List<T>();
            ValidationResults = new List<ImportValidationResult>();
        }

        public List<T> Results { get; set; }

        public List<ImportValidationResult> ValidationResults { get; set; }
    }
}