using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Types;

namespace Fujitsu.Aspose.Spreadsheets
{
    public static class AttributeExtensions
    {
        public static Dictionary<string, PropertyInfo> RowItemValueDictionary<T>(this T instance) where T : class
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorksheetRowItemValueAttribute)) as
                    WorksheetRowItemValueAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    if (attribute.ColumnIndex >= 0)
                    {
                        response.Add(attribute.ColumnIndex.ToString(CultureInfo.InvariantCulture), propertyInfo);
                    }
                    else
                    {
                        response.Add(attribute.ColumnName.Trim(), propertyInfo);
                    }
                }

            }

            return response;
        }

        public static Dictionary<string, PropertyInfo> MultipleRowItemValueDictionary<T>(this T instance) where T : class
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorksheetRowMultipleItemValueAttribute)) as
                    WorksheetRowMultipleItemValueAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    if (attribute.StartIndex >= 0)
                    {
                        response.Add(attribute.StartIndex.ToString(CultureInfo.InvariantCulture), propertyInfo);
                    }
                    else
                    {
                        response.Add(attribute.ColumnPrefix.Trim(), propertyInfo);
                    }
                }
            }

            return response;
        }

        public static Dictionary<string, PropertyInfo> WorksheetRowMultipleRegExItemValueAttribute<T>(this T instance) where T : class
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorksheetRowMultipleRegExItemValueAttribute)) as
                    WorksheetRowMultipleRegExItemValueAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    response.Add(attribute.Expression.Trim(), propertyInfo);
                }
            }

            return response;
        }

        public static Dictionary<int, PropertyInformation> PropertyInfoDictionaryForRowItem(this Worksheet worksheet, int titleRow, Dictionary<string, PropertyInfo>
                                                                                            rowItemValueAttributes)
        {
            // create the response
            var response = new Dictionary<int, PropertyInformation>();
            var row = worksheet.Cells.Rows[titleRow];

            if (row.IsBlank)
            {
                return response;
            }

            // get cell enumerator and move to first item
            var cellEnumerator = row.GetEnumerator();

            // while the cell is something
            while (cellEnumerator.MoveNext())
            {
                var cell = cellEnumerator.Current as Cell;

                // ge the key as a string
                string key = cell.Column.ToString(CultureInfo.InvariantCulture);
                // does it have any value in the cell
                string stringValue = cell.StringValue.Trim();

                if (rowItemValueAttributes.ContainsKey(key))
                {
                    response.Add(cell.Column, new PropertyInformation() { PropertyInfo = rowItemValueAttributes[key], Name = stringValue });
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(stringValue))
                    {
                        // do we have a proeprty that needs a value from a column with that value
                        if (rowItemValueAttributes.ContainsKey(stringValue))
                        {
                            // add to the response with the column index
                            response.Add(cell.Column, new PropertyInformation { PropertyInfo = rowItemValueAttributes[stringValue], Name = stringValue });
                        }
                    }
                }

            }

            // return the response.
            return response;

        }

        public static void MultiPropertyInfoDictionaryForRowItem(this Worksheet worksheet, int titleRow, Dictionary<string, PropertyInfo>
                                                                                           rowItemValueAttributes, Dictionary<int, PropertyInformation> response)
        {
            // create the response
            var row = worksheet.Cells.Rows[titleRow];

            if (row.IsBlank)
            {
                return;
            }

            // get cell enumerator and move to first item
            var cellEnumerator = row.GetEnumerator();

            // while the cell is something
            while (cellEnumerator.MoveNext())
            {
                var cell = cellEnumerator.Current as Cell;

                // get the key as a string
                string key = cell.Column.ToString(CultureInfo.InvariantCulture);
                // does it have any value in the cell
                string stringValue = cell.StringValue.Trim();

                if (rowItemValueAttributes.ContainsKey(key))
                {
                    response.Add(cell.Column, new PropertyInformation { PropertyInfo = rowItemValueAttributes[key], Name = stringValue });
                }
                else
                {


                    if (!String.IsNullOrWhiteSpace(stringValue))
                    {
                        var propInfo = rowItemValueAttributes.FirstOrDefault(x => stringValue.StartsWith(x.Key));

                        // do we have a proeprty that needs a value from a column with that value
                        if (propInfo.Value != null)
                        {
                            // add to the response with the column index
                            response.Add(cell.Column, new PropertyInformation { PropertyInfo = propInfo.Value, Name = stringValue });
                        }
                    }
                }

            }
        }

        public static void MultiPropertyInfoDictionaryForRegExRowItem(this Worksheet worksheet, int titleRow, Dictionary<string, PropertyInfo>
                                                                                   rowItemValueAttributes, Dictionary<int, PropertyInformation> response)
        {
            // create the response
            var row = worksheet.Cells.Rows[titleRow];

            if (row.IsBlank)
            {
                return;
            }

            // get cell enumerator and move to first item
            var cellEnumerator = row.GetEnumerator();

            // while the cell is something
            while (cellEnumerator.MoveNext())
            {
                var cell = cellEnumerator.Current as Cell;

                // does it have any value in the cell
                string stringValue = cell.StringValue.Trim();

                if (!String.IsNullOrWhiteSpace(stringValue))
                {
                    var propInfo = rowItemValueAttributes.FirstOrDefault(x => Regex.IsMatch(stringValue, x.Key));

                    // do we have a property that meets a value from a column with that value
                    if (propInfo.Value != null)
                    {
                        // add to the response with the column index
                        response.Add(cell.Column, new PropertyInformation { PropertyInfo = propInfo.Value, Name = stringValue });
                    }
                }
            }

        }


        public static Dictionary<string, PropertyInfo> ItemCellReferenceDictionary<T>(this T instance)
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorksheetItemValueAttribute)) as WorksheetItemValueAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    response.Add(attribute.CellReference, propertyInfo);
                }
            }


            // return response
            return response;

        }

        public static Dictionary<string, PropertyInfo> WorkbookBuiltInPropertyDictionary<T>(this T instance)
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorkbookBuiltInPropertyAttribute)) as WorkbookBuiltInPropertyAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    response.Add(attribute.PropertyName, propertyInfo);
                }
            }


            // return response
            return response;

        }

        public static Dictionary<string, PropertyInfo> WorkbookCustomPropertyDictionary<T>(this T instance)
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorkbookCustomPropertyAttribute)) as WorkbookCustomPropertyAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    response.Add(attribute.PropertyName, propertyInfo);
                }
            }


            // return response
            return response;

        }

        public static Dictionary<string, PropertyInfo> WorkbookNamedRangeDictionary<T>(this T instance)
        {
            // create the response
            var response = new Dictionary<string, PropertyInfo>();

            // get all properties from the type
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                // does it contain the property we are interested in
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(WorkbookNamedRangeAttribute)) as WorkbookNamedRangeAttribute;

                // yes.  so add to the dictionary
                if (attribute != null)
                {
                    response.Add(attribute.NamedRangeName, propertyInfo);
                }
            }


            // return response
            return response;

        }



    }
    public static class CellExtensions
    {
        public static void SetHyperLinkText(this Cell cell)
        {
            var style = cell.GetStyle();
            style.Font.Color = Color.Blue;
            style.Font.Underline = FontUnderlineType.Single;

            cell.SetStyle(style);
        }
    }
}
