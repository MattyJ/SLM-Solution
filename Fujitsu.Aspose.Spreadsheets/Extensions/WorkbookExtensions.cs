using System.IO;
using Aspose.Cells;

namespace Fujitsu.Aspose.Spreadsheets
{
    public static class WorkbookExtensions
    {
        public static string ExportToHtml(this Workbook workbook, int worksheetIndex)
        {
            // Select the required worksheet
            workbook.Worksheets.ActiveSheetIndex = worksheetIndex;

            // Create the memory stream for the Html
            var memoryStream = new MemoryStream();

            // Save the workbook to the memory stream
            workbook.Save(memoryStream, SaveFormat.Html);

            // Position at start ready for read
            memoryStream.Position = 0;

            // Create stream reader and return
            var streamReader = new StreamReader(memoryStream);

            return streamReader.ReadToEnd();
        }

        public static string ExportToHtml(this Workbook workbook, string worksheetName)
        {
            // Select the required worksheet
            var worksheet = workbook.Worksheets[worksheetName];
            if (worksheet == null)
            {
                return null;
            }
            
            return workbook.ExportToHtml(worksheet.Index);
        }

        
    }
}
