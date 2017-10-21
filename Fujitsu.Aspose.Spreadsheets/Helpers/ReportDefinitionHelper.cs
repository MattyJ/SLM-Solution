using System;
using System.Drawing;
using System.Linq;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Attributes;
using Fujitsu.Aspose.Spreadsheets.Extensions;
using Fujitsu.Aspose.Spreadsheets.Types.Definition;

namespace Fujitsu.Aspose.Spreadsheets.Helpers
{
    internal static class ReportDefinitionHelper
    {
        internal static ReportDefinition<T> GetReportDefinition<T>() where T : class
        {
            var reportDefinition = new ReportDefinition<T>();

            // See if there is a Worksheet Attribute.
            var worksheetAttribute = typeof(T)
                .GetCustomAttributes(typeof(WorksheetAttribute), true)
                .FirstOrDefault() as WorksheetAttribute;

            if (worksheetAttribute != null)
            {
                reportDefinition.Name = worksheetAttribute.Name;
                reportDefinition.TabColor = worksheetAttribute.TabColor;
                if (worksheetAttribute.StandardWidth != int.MinValue)
                {
                    reportDefinition.StandardWidth = worksheetAttribute.StandardWidth;
                }
            }

            // See if there is a Freeze Attribute.
            var worksheetFreezeAttribute = typeof(T)
                .GetCustomAttributes(typeof(WorksheetFreezeAttribute), true)
                .FirstOrDefault() as WorksheetFreezeAttribute;

            if (worksheetFreezeAttribute != null)
            {
                reportDefinition.Freeze = new FreezeDefinition()
                {
                    Column = worksheetFreezeAttribute.Column,
                    Row = worksheetFreezeAttribute.Row
                };
                if (worksheetFreezeAttribute.FreezedColumns != int.MinValue)
                {
                    reportDefinition.Freeze.FreezedColumns = worksheetFreezeAttribute.FreezedColumns;
                }
                if (worksheetFreezeAttribute.FreezedRows != int.MinValue)
                {
                    reportDefinition.Freeze.FreezedRows = worksheetFreezeAttribute.FreezedRows;
                }
            }

            // See if there is a filter attribute.
            var worksheetFilterAttribute = typeof(T)
                .GetCustomAttributes(typeof(WorksheetFilterAttribute), true)
                .FirstOrDefault() as WorksheetFilterAttribute;

            if (worksheetFilterAttribute != null)
            {
                reportDefinition.Filter = new FilterDefinition()
                {
                    Row = worksheetFilterAttribute.Row
                };
                if (worksheetFilterAttribute.StartColumn != int.MinValue)
                {
                    reportDefinition.Filter.StartColumn = worksheetFilterAttribute.StartColumn;
                }
                if (worksheetFilterAttribute.EndColumn != int.MinValue)
                {
                    reportDefinition.Filter.EndColumn = worksheetFilterAttribute.EndColumn;
                }
            }

            // See if there are worksheet rows.
            var worksheetRowAttributes = typeof (T)
                .GetCustomAttributes(typeof (WorksheetRowAttribute), true)
                .Select(s => s as WorksheetRowAttribute)
                .OrderBy(o => o.Row)
                .ToList();
            worksheetRowAttributes.ForEach(f =>
            {
                // See if a row already exists.
                var row = reportDefinition
                    .Rows
                    .SingleOrDefault(s => s.Row == f.Row);
                if (row == null)
                {
                    row = new RowDefinition()
                    {
                        Row = f.Row
                    };
                    reportDefinition.Rows.Add(row);
                }

                var rowContent = new RowContentDefinition()
                {
                    StartColumn = f.StartColumn,
                    Value = f.Value,
                    ValueExternal = f.ValueExternal,
                    ValueFormula = f.ValueFormula
                };

                if (f.EndColumn != int.MinValue)
                {
                    rowContent.EndColumn = f.EndColumn;
                }

                Style style;
                StyleFlag styleFlag;
                GetStyle(f, out style, out styleFlag);
                rowContent.Style = style;
                rowContent.StyleFlag = styleFlag;

                row.RowContents.Add(rowContent);
            });

            // See if there are worksheet styles.
            var worksheetStyleAttributes = typeof(T)
                .GetCustomAttributes(typeof(WorksheetStyleAttribute), true)
                .Select(s => s as WorksheetStyleAttribute)
                .ToList();
            worksheetStyleAttributes.ForEach(f =>
            {
                var styleDefinition = new WorksheetStyleDefinition()
                {
                    StartColumn = f.StartColumn,
                    StartRow = f.StartRow,
                };
                if (f.EndColumn != int.MinValue)
                {
                    styleDefinition.EndColumn = f.EndColumn;
                }
                if (f.EndRow != int.MinValue)
                {
                    styleDefinition.EndRow = f.EndRow;
                }
// ReSharper disable once CompareOfFloatsByEqualityOperator
                if (f.Height != double.MinValue)
                {
                    styleDefinition.Height = f.Height;
                }

                Style style;
                StyleFlag styleFlag;
                GetStyle(f, out style, out styleFlag);
                styleDefinition.Style = style;
                styleDefinition.StyleFlag = styleFlag;

                reportDefinition.Styles.Add(styleDefinition);
            });

            // Get all properties decorated with the WorksheetColumnAttribute.
            reportDefinition.Columns.AddRange(typeof(T)
                .GetProperties()
                .Where(w => w.GetCustomAttributes(typeof(WorksheetColumnAttribute), true).Any())
                .Select(s =>
                {
                    var worksheetColumnAttribute = (WorksheetColumnAttribute)s
                        .GetCustomAttributes(typeof(WorksheetColumnAttribute), true)
                        .First();

                    var column = new ColumnDefinition<T>()
                    {
                        Ordinal = worksheetColumnAttribute.Ordinal,
                        Name = worksheetColumnAttribute.Name,
                        Value = s.GetGetMethod().CreateGetter<T>()
                    };
// ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (worksheetColumnAttribute.Width != double.MinValue)
                    {
                        column.Width = worksheetColumnAttribute.Width;
                    }

                    // Look for a style attribute. This is the base class, so make sure it excludes anything derived from
                    // this.
                    var worksheetColumnStyleAttribute = (WorksheetColumnStyleAttribute)s
                        .GetCustomAttributes(typeof(WorksheetColumnStyleAttribute), true)
                        .SingleOrDefault(w => w.GetType() != typeof(WorksheetColumnHeaderStyleAttribute));

                    if (worksheetColumnStyleAttribute != null)
                    {
                        Style style;
                        StyleFlag styleFlag;
                        GetStyle(worksheetColumnStyleAttribute, out style, out styleFlag);
                        column.Style = new StyleDefinition
                        {
                            Style = style,
                            StyleFlag = styleFlag
                        };
                    }

                    var worksheetColumnHeaderStyleAttribute = (WorksheetColumnHeaderStyleAttribute)s
                        .GetCustomAttributes(typeof(WorksheetColumnHeaderStyleAttribute), true)
                        .SingleOrDefault();

                    if (worksheetColumnHeaderStyleAttribute != null)
                    {
                        Style style;
                        StyleFlag styleFlag;
                        GetStyle(worksheetColumnHeaderStyleAttribute, out style, out styleFlag);
                        column.HeaderStyle = new StyleDefinition
                        {
                            Style = style,
                            StyleFlag = styleFlag
                        };
                    }

                    return column;
                })
                .OrderBy(o => o.Ordinal)
                .ToList());
            var columnIndex = 0;
            reportDefinition.Columns.ForEach(f => f.ColumnIndex = columnIndex++);
            return reportDefinition;
        }

        private static void GetStyle(StyleAttribute attribute, out Style style, out StyleFlag styleFlag)
        {
            style = new Style() {IsTextWrapped = attribute.IsTextWrapped};
            styleFlag = new StyleFlag {WrapText = true};

            if (!string.IsNullOrEmpty(attribute.HorizontalAlign))
            {
                styleFlag.HorizontalAlignment = true;
                style.HorizontalAlignment = (TextAlignmentType)Enum.Parse(typeof(TextAlignmentType), attribute.HorizontalAlign);
            }
            if (!string.IsNullOrEmpty(attribute.Pattern))
            {
                styleFlag.CellShading = true;
                style.Pattern = (BackgroundType)Enum.Parse(typeof(BackgroundType), attribute.Pattern);
            }
            if (!string.IsNullOrEmpty(attribute.BackgroundColor))
            {
                styleFlag.CellShading = true;
                style.BackgroundColor = ColorTranslator.FromHtml(attribute.BackgroundColor);
            }
            if (!string.IsNullOrEmpty(attribute.ForegroundColor))
            {
                styleFlag.CellShading = true;
                style.ForegroundColor = ColorTranslator.FromHtml(attribute.ForegroundColor);
            }
            if (!string.IsNullOrEmpty(attribute.Custom))
            {
                styleFlag.NumberFormat = true;
                style.Custom = attribute.Custom;
            }
            if (attribute.NumberFormat)
            {
                styleFlag.NumberFormat = attribute.NumberFormat;
            }
            if (attribute.FontBold)
            {
                style.Font.IsBold = true;
                styleFlag.FontBold = true;
            }
            if (attribute.FontItalic)
            {
                style.Font.IsItalic = true;
                styleFlag.FontItalic = true;
            }
            if (!string.IsNullOrEmpty(attribute.FontUnderline))
            {
                style.Font.Underline = (FontUnderlineType)Enum.Parse(typeof(FontUnderlineType), attribute.FontUnderline);
                styleFlag.FontUnderline = true;
            }
            if (!string.IsNullOrEmpty(attribute.FontColor))
            {
                styleFlag.FontColor = true;
                style.Font.Color = ColorTranslator.FromHtml(attribute.FontColor);
            }
            if (!string.IsNullOrEmpty(attribute.TopBorderLineStyle))
            {
                styleFlag.TopBorder = true;
                style.Borders[BorderType.TopBorder].LineStyle = (CellBorderType)Enum.Parse(typeof(CellBorderType), attribute.TopBorderLineStyle);
            }
            if (!string.IsNullOrEmpty(attribute.TopBorderColor))
            {
                styleFlag.TopBorder = true;
                style.Borders[BorderType.TopBorder].Color = ColorTranslator.FromHtml(attribute.TopBorderColor);
            }
            if (!string.IsNullOrEmpty(attribute.BottomBorderLineStyle))
            {
                styleFlag.BottomBorder = true;
                style.Borders[BorderType.BottomBorder].LineStyle = (CellBorderType)Enum.Parse(typeof(CellBorderType), attribute.BottomBorderLineStyle);
            }
            if (!string.IsNullOrEmpty(attribute.BottomBorderColor))
            {
                styleFlag.BottomBorder = true;
                style.Borders[BorderType.BottomBorder].Color = ColorTranslator.FromHtml(attribute.BottomBorderColor);
            }
            if (!string.IsNullOrEmpty(attribute.LeftBorderLineStyle))
            {
                styleFlag.LeftBorder = true;
                style.Borders[BorderType.LeftBorder].LineStyle = (CellBorderType)Enum.Parse(typeof(CellBorderType), attribute.LeftBorderLineStyle);
            }
            if (!string.IsNullOrEmpty(attribute.LeftBorderColor))
            {
                styleFlag.LeftBorder = true;
                style.Borders[BorderType.LeftBorder].Color = ColorTranslator.FromHtml(attribute.LeftBorderColor);
            }
            if (!string.IsNullOrEmpty(attribute.RightBorderLineStyle))
            {
                styleFlag.RightBorder = true;
                style.Borders[BorderType.RightBorder].LineStyle = (CellBorderType)Enum.Parse(typeof(CellBorderType), attribute.RightBorderLineStyle);
            }
            if (!string.IsNullOrEmpty(attribute.RightBorderColor))
            {
                styleFlag.RightBorder = true;
                style.Borders[BorderType.RightBorder].Color = ColorTranslator.FromHtml(attribute.RightBorderColor);
            }
        }
    }
}
