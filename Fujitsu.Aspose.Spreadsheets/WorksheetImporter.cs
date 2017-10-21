using Aspose.Cells;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetImporter
    {
        public ImportResult<T> Import<T>(Worksheet worksheet)
        {
            // get the item attribute
            var itemAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(WorksheetItemAttribute)) as WorksheetItemAttribute;

            // if this Item does not support the Attribute then throw exception
            if (itemAttribute == null)
            {
                throw new MissingAttributeException<T, WorksheetItemAttribute>();
            }

            // get cell references for the type
            var cellReferenceDictionary = Activator.CreateInstance<T>().ItemCellReferenceDictionary();

            // if we don't have any then throw and exception
            if (cellReferenceDictionary.Count == 0)
            {
                throw new MissingValueAttributeException<T, WorksheetItemValueAttribute>();
            }


            // create the response
            var response = new ImportResult<T>
            {
                Result = Activator.CreateInstance<T>()
            };


            // iterate the propertyInfos we are wanting to populate
            foreach (var cellReference in cellReferenceDictionary.Keys)
            {
                // get the cell
                var cell = worksheet.Cells[cellReference];

                if (cell.Value != null)
                {
                    // get the value from the row
                    var value = cell.Value;

                    // get the property info and set the value
                    var propInfo = cellReferenceDictionary[cellReference];

                    if (value != null)
                    {
                        // In case of a nullable property type, determine the underlying (non-nullable) property type
                        var targetType = Nullable.GetUnderlyingType(propInfo.PropertyType)
                                          ?? propInfo.PropertyType;

                        try
                        {
                            // convert the cell value to the target property type
                            var convertedValue = Convert.ChangeType(value, targetType);

                            // set the target property
                            propInfo.SetValue(response.Result, convertedValue);

                        }
                        catch (Exception)
                        {
                            response.ValidationResults.Add(
                                new ImportValidationResult
                                {
                                    Reference = cellReference,
                                    ValidationResult = new ValidationResult("Could not convert cell value",
                                                                                new string[] { propInfo.Name })
                                });
                        }
                    }
                }
            }

            #region Validation

            // now validate the object
            var context = new ValidationContext(response.Result, null, null);
            var validationResults = new Collection<ValidationResult>();

            // try the validation
            if (!Validator.TryValidateObject(response.Result, context, validationResults, true))
            {
                // get a list of member names to cell rerferences
                var cellToPropertyNameDictionary = cellReferenceDictionary.ToDictionary(k => k.Value.Name,
                                                                                        v => v.Key);

                // now iterate the validation results
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        // do we have a cell reference for that item
                        var cellReference = cellToPropertyNameDictionary[memberName];
                        if (!String.IsNullOrEmpty(cellReference))
                        {

                            // create validation items and add
                            var newValidationItem = new ImportValidationResult
                            {
                                Reference = cellReference,
                                ValidationResult = validationResult
                            };
                            response.ValidationResults.Add(newValidationItem);
                        }
                    }
                }
            }

            #endregion

            return response;

        }


        #region Private Methods



        #endregion

    }
}
