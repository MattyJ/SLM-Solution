using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets;
using Fujitsu.SLM.DataImporters.Entities;
using Fujitsu.SLM.DataImporters.Interfaces;
using System;
using System.IO;

namespace Fujitsu.SLM.DataImporters
{
    public class ServiceDecompositionImporter : IServiceDecompositionImporter
    {
        public ImportListResult<ServiceDecomposition> ImportSpreadsheet(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            // Set the license for Aspose
            var license = new License();
            license.SetLicense("Aspose.Total.lic");

            var workbook = new Workbook(filename);

            var worksheet = workbook.Worksheets.GetSheetByCodeName("Decomposition");

            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            var importer = new WorksheetListImporter();

            var sheet = importer.Import<ServiceDecomposition>(worksheet);

            var result = new ImportListResult<ServiceDecomposition> { ValidationResults = sheet.ValidationResults };

            var lineNumber = 2;
            foreach (var line in sheet.Results)
            {
                if (!string.IsNullOrWhiteSpace(line.ServiceDomain))
                {
                    line.WorksheetName = worksheet.Name;
                    line.LineNumber = lineNumber;

                    result.Results.Add(line);
                }

                lineNumber++;
            }

            return result;
        }
    }
}
