using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fujitsu.Aspose.Spreadsheets.Types;

namespace Fujitsu.Aspose.Spreadsheets.Extensions
{
    public static class PropertyExtensions
    {
        public static bool IsNonStringEnumerable(this PropertyInfo pi)
        {
            return pi != null && pi.PropertyType.IsNonStringEnumerable();
        }

        public static bool IsNonStringEnumerable(this object instance)
        {
            return instance != null && instance.GetType().IsNonStringEnumerable();
        }

        public static bool IsNonStringEnumerable(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsGenericList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition()
                   == typeof (List<>);
        }

        public static bool IsNamedValue(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition()
                   == typeof(NamedValue<>);
        }

        public static bool IsFormattedValue(this PropertyInfo propInfo)
        {
            var itemAttribute =
                Attribute.GetCustomAttribute(propInfo, typeof (WorksheetItemValueAttribute)) as
                    WorksheetItemValueAttribute;

            if (itemAttribute != null)
            {
                return itemAttribute.IsFormattedValue;
            }

           var rowAttribute =
                Attribute.GetCustomAttribute(propInfo, typeof(WorksheetRowItemValueAttribute)) as
                    WorksheetRowItemValueAttribute;

           if (rowAttribute != null)
            {
                return rowAttribute.IsFormattedValue;
            }

            return false;
        }

        public static string FormatPropertyValue(this PropertyInfo propertyInfo, object entity)
        {
            var itemAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(WorksheetItemValueAttribute)) as WorksheetItemValueAttribute;
            if (itemAttribute != null)
            {
                var propertyNames = itemAttribute.PropertyArgumentNames.Split(';');

                return String.Format(itemAttribute.FormatString,
                    propertyNames.Select(propertyName => entity.GetType().GetProperty(propertyName).GetValue(entity))
                        .ToArray());
            }

            var rowAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(WorksheetRowItemValueAttribute)) as WorksheetRowItemValueAttribute;
            if (rowAttribute != null)
            {
                var propertyNames = rowAttribute.PropertyArgumentNames.Split(';');

                return String.Format(rowAttribute.FormatString,
                    propertyNames.Select(propertyName => entity.GetType().GetProperty(propertyName).GetValue(entity))
                        .ToArray());
            }

            return String.Empty;
        }

        public static Func<T, object> CreateGetter<T>(this MethodInfo method) where T : class
        {
            // First fetch the generic form
            var genericHelper = typeof(PropertyExtensions).GetMethod("CreateGetterGeneric",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            var constructedHelper = genericHelper.MakeGenericMethod(typeof(T), method.ReturnType);

            // Now call it. The null argument is because it's a static method.
            var ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<T, object>)ret;
        }

        private static Func<TTarget, object> CreateGetterGeneric<TTarget, TReturn>(MethodInfo method) where TTarget : class
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            var func = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<TTarget, object> ret = target => func(target);
            return ret;
        }
    }
}