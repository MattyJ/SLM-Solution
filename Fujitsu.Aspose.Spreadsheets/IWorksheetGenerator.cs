using System.Collections.Generic;
using Aspose.Cells;

namespace Fujitsu.Aspose.Spreadsheets
{
    public interface IWorksheetGenerator
    {
        void Generate<T>(Worksheet worksheet, T worksheetTemplate) where T : class;

        /// <summary>
        /// This method uses the following attributes to dynamically generate a worksheet:
        /// 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetAttribute"/> and
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetColumnAttribute"/>.
        /// 
        /// The supplied worksheet data type should be decorated with this attributes.
        /// </summary>
        /// <typeparam name="T">The type holding both the report definition and data.</typeparam>
        /// <param name="worksheet">The worksheet to generate the report against.</param>
        /// <param name="worksheetData">The data used to generate the report.</param>
        void GenerateFromDefinition<T>(Worksheet worksheet, IEnumerable<T> worksheetData) where T : class;
    }
}