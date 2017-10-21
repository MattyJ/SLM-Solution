using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Extensions;
using Fujitsu.Aspose.Spreadsheets.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetListImporter
    {
        /// <summary>
        ///  Takes the worksheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worksheet"></param>
        /// <param name="overrideTitleRow"></param>
        /// <param name="overrideAlternativeTitleRow"></param>
        /// <param name="overrideStartRow"></param>
        /// <param name="overrideEndRow"></param>
        /// <returns></returns>
        public ImportListResult<T> Import<T>(Worksheet worksheet, int? overrideTitleRow = null, int? overrideStartRow = null, int? overrideEndRow = null, int? overrideAlternativeTitleRow = null) where T : class
        {
            // get the row item
            var rowAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(WorksheetRowItemAttribute)) as WorksheetRowItemAttribute;

            // if this Item does not support the Row Item Attribute then throw exception
            if (rowAttribute == null)
            {
                throw new MissingAttributeException<T, WorksheetRowItemAttribute>();
            }

            // create dictionaries of property info and the row item value attributes
            var rowItemValueAttributes = Activator.CreateInstance<T>().RowItemValueDictionary();
            var multipleRowItemValueAttributes = Activator.CreateInstance<T>().MultipleRowItemValueDictionary();
            var multipleRegExRowItemValueAttributes = Activator.CreateInstance<T>().WorksheetRowMultipleRegExItemValueAttribute();

            // if we don't have any then throw and exception
            if (rowItemValueAttributes.Count == 0 && multipleRowItemValueAttributes.Count == 0)
            {
                throw new MissingValueAttributeException<T, WorksheetRowItemValueAttribute>();
            }

            var titleRow = overrideTitleRow ?? rowAttribute.TitleRow;
            var startRow = overrideStartRow ?? rowAttribute.StartRow;
            var alternativeTitleRow = overrideAlternativeTitleRow ?? rowAttribute.AlternativeTitleRow;
            var endRow = overrideEndRow ?? rowAttribute.EndRow;


            var propertyInfoDictionary = worksheet.PropertyInfoDictionaryForRowItem(titleRow, rowItemValueAttributes);
            worksheet.MultiPropertyInfoDictionaryForRowItem(titleRow, multipleRowItemValueAttributes, propertyInfoDictionary);

            // Multiple Reg Ex Item can use either the specified title row or an alternative title row
            worksheet.MultiPropertyInfoDictionaryForRegExRowItem(alternativeTitleRow, multipleRegExRowItemValueAttributes, propertyInfoDictionary);

            var response = new ImportListResult<T>();

            // only continue if we have properties
            if (propertyInfoDictionary.Count > 0)
            {
                // get the start row
                var currentRow = startRow;

                // now try to get the first row item
                var responseItem = CreateAndPopulateItemFromRow<T>(worksheet.Cells.Rows[currentRow],
                                                                  propertyInfoDictionary, response.ValidationResults);

                // if it isn't null then add to the list, increment the row cound and get the next row item
                while ((responseItem != null || !rowAttribute.StopAtFirstEmptyRow) && (currentRow <= worksheet.Cells.MaxRow) && (endRow == 0 || currentRow <= endRow))
                {
                    if (responseItem != null)
                    {
                        var context = new ValidationContext(responseItem, null, null);

                        var validationResults = new Collection<ValidationResult>();

                        if (Validator.TryValidateObject(responseItem, context, validationResults, true))
                        {
                            response.Results.Add(responseItem);
                        }
                        else
                        {
                            foreach (var validationResult in validationResults)
                            {
                                response.ValidationResults.Add(
                                    new ImportValidationResult
                                    {
                                        Reference = "Row " + (currentRow + 1).ToString(CultureInfo.InvariantCulture),
                                        ValidationResult = validationResult
                                    });
                            }
                        }
                    }
                    currentRow++;

                    // do a double check to see if we should be ending so as not to get a "rogue" row appearing with validation results.
                    if ((currentRow <= worksheet.Cells.MaxRow) &&
                        (endRow == 0 || currentRow <= endRow))
                    {
                        responseItem = CreateAndPopulateItemFromRow<T>(worksheet.Cells.Rows[currentRow],
                                                                       propertyInfoDictionary,
                                                                       response.ValidationResults);
                    }
                    else
                    {
                        // we must have finished now, so break out.
                        break;
                    }
                }
            }

            // return the response
            return response;
        }

        #region Private Methods

        #region CreateAndPopulateItemFromRow

        /// <summary>
        /// Creates a Generic Type based on the spreadsheet row, and the dictionary of property information
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="row">the row containing the values</param>
        /// <param name="propertyInfoDictionary">Dictionary of Property Info from the type to return</param>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        private static T CreateAndPopulateItemFromRow<T>(Row row, Dictionary<int, PropertyInformation> propertyInfoDictionary, List<ImportValidationResult> validationResults)
        {
            // if the row is blank
            if (row.IsBlank)
            {
                return default(T);
            }

            // create the response
            var response = Activator.CreateInstance<T>();

            // iterate the propertyInfos we are wanting to populate
            foreach (var columnIndex in propertyInfoDictionary.Keys)
            {
                // get the cell
                var cell = row.GetCellOrNull(columnIndex);

                if (cell != null)
                {
                    // get the value from the row
                    var value = cell.Value;

                    // get the property info and set the value
                    var propInfo = propertyInfoDictionary[columnIndex];

                    if (value != null)
                    {
                        // In case of a nullable property type, determine the underlying (non-nullable) property type
                        Type targetType = Nullable.GetUnderlyingType(propInfo.PropertyInfo.PropertyType)
                                            ?? propInfo.PropertyInfo.PropertyType;

                        try
                        {
                            if (propInfo.PropertyInfo.IsNonStringEnumerable())
                            {
                                var propValue = propInfo.PropertyInfo.GetValue(response);

                                if (propValue == null)
                                {
                                    propValue = Activator.CreateInstance(targetType);
                                    propInfo.PropertyInfo.SetValue(response, propValue);
                                }

                                var list = propValue as IList;

                                if (list != null)
                                {
                                    // is a generic list
                                    if (propInfo.PropertyInfo.PropertyType.IsGenericList())
                                    {
                                        // get generic list type
                                        var listGenericType = propInfo.PropertyInfo.PropertyType.GetGenericArguments()[0];

                                        // is one of our special named valued
                                        if (listGenericType.IsNamedValue())
                                        {
                                            // get the special type
                                            var namedGenericType = listGenericType.GetGenericArguments()[0];

                                            // convert the cell value to the target property type
                                            var convertedValue = Convert.ChangeType(value, namedGenericType);

                                            var listItem = Activator.CreateInstance(listGenericType);

                                            if (value != null)
                                            {
                                                var valueProperty = listGenericType.GetProperty("Value");
                                                valueProperty.SetValue(listItem, convertedValue);
                                            }
                                            var nameProperty = listGenericType.GetProperty("Name");
                                            nameProperty.SetValue(listItem, propInfo.Name);

                                            list.Add(listItem);
                                        }

                                    }

                                }

                            }
                            else
                            {
                                if (value != null)
                                {
                                    // convert the cell value to the target property type
                                    var convertedValue = Convert.ChangeType(value, targetType);

                                    // set the target property
                                    propInfo.PropertyInfo.SetValue(response, convertedValue);
                                }
                            }

                        }
                        catch (Exception)
                        {
                            validationResults.Add(
                                new ImportValidationResult
                                {
                                    Reference = "Row " + (row.Index + 1).ToString(CultureInfo.InvariantCulture),
                                    ValidationResult = new ValidationResult("Could not convert cell value",
                                            new string[] { propInfo.Name })
                                });
                        }
                    }
                }
            }

            return response;
        }

        #endregion

        #endregion
    }
}
