using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fujitsu.SLM.Extensions;

namespace Fujitsu.SLM.UnitTesting
{
    [ExcludeFromCodeCoverage]
    public class UnitTestHelper
    {
        private static readonly Random Random;

        static UnitTestHelper()
        {
            Random = new Random();
        }

        public static void PropertiesMatch(object source, object destination, string[] fieldsToIgnore = null)
        {
            var sourceProperties = source.GetType().GetProperties();

            // iterate all the properties on the Source Object
            foreach (var sourceProperty in sourceProperties)
            {
                // see if we have a matching property on the destination
                var destinationProperty = destination.GetType().GetProperty(sourceProperty.Name);

                // we have one
                if (destinationProperty != null)
                {
                    // check that it's not in the list of fields we want to ignore
                    if ((fieldsToIgnore == null) || (!fieldsToIgnore.Contains(sourceProperty.Name)))
                    {
                        var sourceValue = sourceProperty.GetValue(source);
                        var sourceAsString = sourceValue as string;
                        var destinationValue = destinationProperty.GetValue(destination);

                        if (sourceProperty.PropertyType.IsPrimitive || sourceValue is string || sourceValue is Guid ||
                            sourceValue is Decimal || sourceValue is DateTime)
                        {
                            if (sourceValue.GetType().Name == destinationValue.GetType().Name)
                            {
                                Assert.AreEqual(sourceValue, destinationValue,
                                    String.Format("The values of property '{0}' do not match.", sourceProperty.Name));
                            }
                            else if ((destinationValue is int) && (sourceValue is string))
                            {
                                Assert.AreEqual(sourceAsString.ToInt(), destinationValue,
                                    String.Format("The integer values of property '{0}' do not match.", sourceProperty.Name));
                            }
                            else if ((destinationValue is DateTime) && (sourceValue is string))
                            {
                                Assert.AreEqual(sourceAsString.ToDateTime(), destinationValue,
                                    String.Format("The DateTime values of property '{0}' do not match.", sourceProperty.Name));
                            }
                            else if ((destinationValue is bool) && (sourceValue is string))
                            {
                                Assert.AreEqual(sourceAsString.ToBool(), destinationValue,
                                    String.Format("The boolean values of property '{0}' do not match.", sourceProperty.Name));
                            }
                            else
                            {
                                throw new InternalTestFailureException("Unexpected Destination Value Type in UnitTestHelper.PropertiesMatch. " +
                                                                        "SourceType = " + sourceValue.GetType().Name +
                                                                         ", DestinationType =" + destinationValue.GetType().Name);
                            }
                        }
                        else
                        {
                            //PropertiesMatch(sourceValue, destinationValue);
                            throw new NotImplementedException();
                        }
                    }
                }
            }

        }

        public static T GenerateRandomData<T>()
        {
            return GenerateRandomData<T>(null);
        }

        public static T GenerateRandomData<T>(Action<T> postGenerateAction)
        {
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

                    // Now random generate
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(response, Guid.NewGuid().ToString());
                    }
                    else if ((property.PropertyType == typeof(int)) || (property.PropertyType == typeof(int?)))
                    {
                        property.SetValue(response, Random.Next(Int32.MaxValue));
                    }
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                    {
                        property.SetValue(response, Convert.ToDecimal(Random.Next(Int32.MaxValue)));
                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        property.SetValue(response, DateTime.Now.AddDays(Random.Next(500)));
                    }
                    else if (property.PropertyType == typeof(Guid))
                    {
                        property.SetValue(response, Guid.NewGuid());
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(response, Random.Next() % 2 == 0);
                    }
                    else if (property.PropertyType == typeof(byte[]))
                    {
                        const int size = 5 * 1024;
                        var generateBuffer = new RandomBuffer(size);
                        property.SetValue(response, generateBuffer.GenerateBufferFromSeed(size));
                    }
                }

            }

            if (postGenerateAction != null)
            {
                postGenerateAction(response);
            }

            return response;

        }

        public static bool PropertiesAllPopulated(object obj)
        {
            var properties = obj.GetType().GetProperties();

            return properties.All(property => property.GetValue(obj) != null);
        }

        public static void ClearMemoryCache()
        {
            var cache = MemoryCache.Default;
            var cacheKeys = cache.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                cache.Remove(cacheKey);
            }
        }

    }
}
