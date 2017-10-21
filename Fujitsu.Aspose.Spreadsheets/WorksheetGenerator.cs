using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Dependency;
using Fujitsu.Aspose.Spreadsheets.Extensions;
using Fujitsu.Aspose.Spreadsheets.Helpers;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetGenerator : IWorksheetGenerator
    {
        public void Generate<T>(Worksheet worksheet, T worksheetTemplate) where T : class
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            // get the item attribute
            var itemAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(WorksheetItemAttribute)) as WorksheetItemAttribute;

            // if this Item does not support the Attribute then throw exception
            if (itemAttribute == null)
            {
                throw new MissingAttributeException<T, WorksheetItemAttribute>();
            }

            // get cell references for the type
            var cellReferenceDictionary = Activator.CreateInstance<T>().ItemCellReferenceDictionary();
            var builtInProperties = Activator.CreateInstance<T>().WorkbookBuiltInPropertyDictionary();
            var customProperties = Activator.CreateInstance<T>().WorkbookCustomPropertyDictionary();
            var namedRanges = Activator.CreateInstance<T>().WorkbookNamedRangeDictionary();

            // if we don't have any then throw and exception
            if (cellReferenceDictionary.Count == 0 && builtInProperties.Count == 0 && customProperties.Count == 0 && namedRanges.Count == 0)
            {
                throw new MissingValueAttributeException<T, WorksheetItemValueAttribute>();
            }

            // we are here and ready to start writing the values
            foreach (var key in cellReferenceDictionary.Keys)
            {
                var propInfo = cellReferenceDictionary[key];

                if (!propInfo.IsFormattedValue())
                {
                    var value = propInfo.GetValue(worksheetTemplate);
                    worksheet.Cells[key].Value = value;
                }
                else
                {
                    var value =propInfo.FormatPropertyValue(worksheetTemplate);
                     worksheet.Cells[key].Formula = value;

                    worksheet.Cells[key].SetHyperLinkText();
                }
            }

            // get the document pro
            foreach (var key in builtInProperties.Keys)
            {
                var propInfo = builtInProperties[key];
                var value = propInfo.GetValue(worksheetTemplate);

                if (value != null)
                {
                    SetBuiltInProperty(worksheet.Workbook, key, value.ToString());
                }
            }

            // get the document pro
            foreach (var key in customProperties.Keys)
            {
                var propInfo = customProperties[key];
                var value = propInfo.GetValue(worksheetTemplate);

                SetCustomProperty(worksheet.Workbook, key, value);
            }

            // get the document pro
            foreach (var key in namedRanges.Keys)
            {
                var propInfo = namedRanges[key];
                var value = propInfo.GetValue(worksheetTemplate);

                SetNamedRange(worksheet.Workbook, key, value);
            }

        }

        /// <summary>
        /// This method uses the following attributes to dynamically generate a worksheet. The following
        /// attributes are class level:
        /// 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetAttribute"/>, 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetStyleAttribute"/>, 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetRowAttribute"/>, 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetFilterAttribute"/> and 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetFreezeAttribute"/>.
        /// 
        /// The following attributes are property level:
        /// 
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetColumnAttribute"/>,
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetColumnStyleAttribute"/> and
        ///     <see cref="Fujitsu.Aspose.Spreadsheets.Attributes.WorksheetColumnHeaderStyleAttribute"/>.
        /// 
        /// The supplied worksheet data type should be decorated with these attributes.
        /// </summary>
        /// <typeparam name="T">The type holding both the report definition and data.</typeparam>
        /// <param name="worksheet">The worksheet to generate the report against.</param>
        /// <param name="worksheetData">The data used to generate the report.</param>
        public void GenerateFromDefinition<T>(Worksheet worksheet, IEnumerable<T> worksheetData) where T : class
        {
            var dependencyResolver = new DependencyResolver();
            var reportDefinition = ReportDefinitionHelper.GetReportDefinition<T>();

            // Set the worksheet level settings.
            worksheet.Name = reportDefinition.Name;
            if (!string.IsNullOrEmpty(reportDefinition.TabColor))
            {
                worksheet.TabColor = ColorTranslator.FromHtml(reportDefinition.TabColor);
            }

            // Set the standard width.
            if (reportDefinition.StandardWidth.HasValue)
            {
                worksheet.Cells.StandardWidth = reportDefinition.StandardWidth.Value;
            }

            // Generate column headings.
            worksheet.Cells.ImportArray(reportDefinition.Columns.Select(s => s.Name).ToArray(), 0, 0, false);

            // Generate the data rows.
            var i = 1;
            foreach (var rowData in worksheetData)
            {
                worksheet.Cells.ImportObjectArray(
                    reportDefinition.Columns
                        .Select(s => s.Value(rowData))
                        .ToArray(),
                    i++,
                    0,
                    false);
            }

            // Set freeze panes.
            if (reportDefinition.Freeze != null)
            {
                worksheet.FreezePanes(reportDefinition.Freeze.Row,
                    reportDefinition.Freeze.Column,
                    reportDefinition.Freeze.FreezedRows.HasValue ? reportDefinition.Freeze.FreezedRows.Value : worksheet.Cells.MaxRow,
                    reportDefinition.Freeze.FreezedColumns.HasValue ? reportDefinition.Freeze.FreezedColumns.Value : worksheet.Cells.MaxColumn);
            }

            // Set filter.
            if (reportDefinition.Filter != null)
            {
                worksheet.AutoFilter.SetRange(reportDefinition.Filter.Row,
                    reportDefinition.Filter.StartColumn.HasValue ? reportDefinition.Filter.StartColumn.Value : 0,
                    reportDefinition.Filter.EndColumn.HasValue ? reportDefinition.Filter.EndColumn.Value : worksheet.Cells.MaxColumn);
            }

            // Set column styles.
            reportDefinition.Columns.ForEach(f =>
            {
                if (f.Width.HasValue)
                {
                    worksheet.Cells.SetColumnWidth(f.ColumnIndex, f.Width.Value);
                }
                if (f.Style != null)
                {
                    var range = worksheet.Cells.CreateRange(0, f.ColumnIndex, worksheet.Cells.MaxRow + 1, 1);
                    range.ApplyStyle(f.Style.Style, f.Style.StyleFlag);
                }
                if (f.HeaderStyle != null)
                {
                    var range = worksheet.Cells.CreateRange(0, f.ColumnIndex, 1, 1);
                    range.ApplyStyle(f.HeaderStyle.Style, f.HeaderStyle.StyleFlag);
                }
            });

            // Set all the styles.
            reportDefinition.Styles.ForEach(f =>
            {
                var endColumn = f.EndColumn.HasValue ? f.EndColumn.Value : worksheet.Cells.MaxColumn;
                var endRow = f.EndRow.HasValue ? f.EndRow.Value : worksheet.Cells.MaxRow;
                var range = worksheet.Cells.CreateRange(f.StartRow,
                    f.StartColumn,
                    endRow - f.StartRow + 1,
                    endColumn - f.StartColumn + 1);
                range.ApplyStyle(f.Style, f.StyleFlag);
                if (f.Height.HasValue)
                {
                    for (var rowIdx = f.StartRow; rowIdx <= endRow; rowIdx++)
                    {
                        worksheet.Cells.SetRowHeight(rowIdx, f.Height.Value);
                    }
                }
            });

            // Add additional rows.
            reportDefinition.Rows.ForEach(f =>
            {
                var rowIndex = f.Row == int.MinValue ? worksheet.Cells.MaxRow + 1 : f.Row;
                worksheet.Cells.InsertRow(rowIndex);
                // Process each of the row contents.
                f.RowContents.ForEach(r =>
                {
                    var endColumn = r.EndColumn.HasValue ? r.EndColumn.Value : worksheet.Cells.MaxColumn;
                    var range = worksheet.Cells.CreateRange(rowIndex,
                        r.StartColumn,
                        1,
                        endColumn - r.StartColumn + 1);
                    range.ApplyStyle(r.Style, r.StyleFlag);
                    range.Merge();
                    if (r.Value != null)
                    {
                        range.Value = r.Value;
                    }
                    else if (!string.IsNullOrEmpty(r.ValueFormula))
                    {
                        worksheet.Cells[rowIndex, r.StartColumn].Formula = r.ValueFormula;
                    }
                    else if (!string.IsNullOrEmpty(r.ValueExternal))
                    {
                        var helper = dependencyResolver.Resolve<ICellValue>(r.ValueExternal);
                        range.Value = helper.Get();
                    }
                });
            });

            worksheet.Workbook.CalculateFormula();
        }

        private void SetBuiltInProperty(Workbook workbook, string propertyName, string propertyValue)
        {
            // get the property
            var property = workbook.BuiltInDocumentProperties[propertyName];

            // if we have a property, then set the value.
            if (property != null)
            {
                property.Value = propertyValue;

            }
        }

        private void SetCustomProperty(Workbook workbook, string propertyName, object propertyValue)
        {
            // get the property
            var property = workbook.Worksheets.CustomDocumentProperties[propertyName];

            // if we have a property, then set the value.
            if (property != null && propertyValue != null)
            {
                property.Value = propertyValue;
            }

        }

        private void SetNamedRange(Workbook workbook, string rangeName, object rangeValue)
        {
            // get the property
            var range = workbook.Worksheets.GetRangeByName(rangeName);

            // if we have a property, then set the value.
            if (range != null)
            {
                range.Value = rangeValue;

            }

        }
    }
}