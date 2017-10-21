using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests
{
    public static class UnitTestHelper
    {
        public static T GenerateRandomData<T>()
        {
            var random = new Random();
            var response = Activator.CreateInstance<T>();
            var properties = response.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var propertyInstance = property.GetValue(response);

                    //if (propertyInstance == null)
                    //{
                    //    Activator.CreateInstance(property.PropertyType, null);
                    //}

                    // now random generate
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(response, Guid.NewGuid().ToString());
                    }
                    else if ((property.PropertyType == typeof(int)) || (property.PropertyType == typeof(int?)))
                    {
                        property.SetValue(response, random.Next(Int32.MaxValue));
                    }
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                    {
                        property.SetValue(response, Convert.ToDecimal(random.Next(Int32.MaxValue)));
                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        property.SetValue(response, DateTime.Now.AddDays(random.Next(500)));
                    }
                    else if (property.PropertyType == typeof(Guid))
                    {
                        property.SetValue(response, Guid.NewGuid());
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(response, random.Next() % 2 == 0);
                    }
                }

            }

            return response;

        }
    }
}