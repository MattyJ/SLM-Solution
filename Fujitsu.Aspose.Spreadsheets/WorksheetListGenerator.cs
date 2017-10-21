using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Extensions;
using System;
using System.Collections.Generic;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetListGenerator
    {
        public void Generate<T>(Worksheet worksheet, List<T> worksheetListItems, int? overrideTitleRow = null, int? overrideStartRow = null) where T : class
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            // get the item attribute
            var rowAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(WorksheetRowItemAttribute)) as WorksheetRowItemAttribute;

            // if this Item does not support the Attribute then throw exception
            if (rowAttribute == null)
            {
                throw new MissingAttributeException<T, WorksheetRowItemAttribute>();
            }

            // get cell references for the type
            var rowItemValueAttributes = Activator.CreateInstance<T>().RowItemValueDictionary();

            // if we don't have any then throw and exception
            if (rowItemValueAttributes.Count == 0)
            {
                throw new MissingValueAttributeException<T, WorksheetRowItemValueAttribute>();
            }

            var startRow = overrideStartRow ?? rowAttribute.StartRow;
            var titleRow = overrideTitleRow ?? rowAttribute.TitleRow;
            var duplicateRow = rowAttribute.DuplicateFirstRowOnGenerate;

            var propertyInfoDictionary = worksheet.PropertyInfoDictionaryForRowItem(titleRow, rowItemValueAttributes);

            if (worksheetListItems != null && worksheetListItems.Count > 0)
            {
                if (duplicateRow)
                {
                    // insert the correct number of rows for the data
                    worksheet.Cells.InsertRows(startRow + 1, worksheetListItems.Count - 1);

                    var cells = worksheet.Cells;

                    for (int index = 1; index < worksheetListItems.Count; index++)
                    {
                        worksheet.Cells.CopyRow(cells, startRow, startRow + index);
                    }

                }

                // now populate
                foreach (var worksheetItem in worksheetListItems)
                {
                    foreach (var columnIndex in propertyInfoDictionary.Keys)
                    {
                        var propInfo = propertyInfoDictionary[columnIndex];

                        if (!propInfo.PropertyInfo.IsFormattedValue())
                        {
                            var value = propInfo.PropertyInfo.GetValue(worksheetItem);
                            worksheet.Cells[startRow, columnIndex].Value = value;
                        }
                        else
                        {
                            var value = propInfo.PropertyInfo.FormatPropertyValue(worksheetItem);
                            worksheet.Cells[startRow, columnIndex].Formula = value;

                            worksheet.Cells[startRow, columnIndex].SetHyperLinkText();
                        }
                    }

                    startRow++;

                }
            }
        }
    }
}