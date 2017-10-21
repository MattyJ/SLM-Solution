using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Fujitsu.SLM.Constants;

namespace Fujitsu.SLM.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetProperty(this object o, string propertyName, object propertyValue)
        {
            if (o == null)
            {
                return;
            }

            o.GetType()
                .GetProperty(propertyName)
                .SetValue(o, propertyValue);
        }

        public static object GetProperty(this object o, string propertyName)
        {
            if (o == null)
            {
                return null;
            }

            return o.GetType()
                .GetProperty(propertyName)
                .GetValue(o);
        }

        public static T GetPropertyAttribute<T>(this Type type, string propertyName) where T : Attribute
        {
            var property = type.GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }
            var attributes = property.GetCustomAttributes(false);
            if (!attributes.Any())
            {
                return null;
            }

            return attributes.SingleOrDefault(x => x.GetType() == typeof(T)) as T;
        }

        public static PropertyInfo GetPropertyInfo(this object o, string propertyName)
        {
            if (o == null)
            {
                return null;
            }

            return o.GetType()
                .GetProperty(propertyName);
        }

        public static List<PropertyInfo> GetProperties(this object o, Func<PropertyInfo, bool> query)
        {
            if (o == null)
            {
                return null;
            }

            return o.GetType()
                .GetProperties()
                .Where(query)
                .ToList();
        }

        public static TValue GetClassAttributeValue<TAttribute, TValue>(this object o,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
            where TValue : class
        {
            if (o == null)
            {
                return null;
            }
            var att = o.GetType()
                .GetCustomAttributes(typeof(TAttribute), true)
                .FirstOrDefault() as TAttribute;
            return att != null ? valueSelector(att) : default(TValue);
        }

        public static TAttribute GetMethodAttribute<TAttribute>(this object o,
            string methodName,
            Type[] parameterTypes = null)
            where TAttribute : Attribute
        {
            if (o == null)
            {
                return null;
            }

            MethodInfo method = null;
            if (parameterTypes != null)
            {
                method = o
                    .GetType()
                    .GetMethod(methodName, parameterTypes);
            }
            else
            {
                method = o
                    .GetType()
                    .GetMethod(methodName);
            }

            return method
                .GetCustomAttributes(typeof(TAttribute), false)
                .FirstOrDefault() as TAttribute;
        }

        public static TValue GetMethodAttributeValue<TAttribute, TValue>(this object o,
            string methodName,
            Func<TAttribute, TValue> valueSelector,
            Type[] parameterTypes = null)
            where TAttribute : Attribute
            where TValue : class
        {
            if (o == null)
            {
                return null;
            }

            var att = GetMethodAttribute<TAttribute>(o, methodName, parameterTypes);
            return att != null ? valueSelector(att) : default(TValue);
        }

        public static bool IsActionMethodHttpGet(this object o,
                string methodName,
                Type[] parameterTypes = null)
        {
            if (o == null)
            {
                return false;
            }

            MethodInfo method = null;
            if (parameterTypes != null)
            {
                method = o
                    .GetType()
                    .GetMethod(methodName, parameterTypes);
            }
            else
            {
                method =
                    o.GetType().GetMethod(methodName);

            }

            return method.GetCustomAttributes().Any(attr => attr is HttpGetAttribute);
        }

        public static bool IsActionMethodHttpPost(this object o, string methodName, Type[] parameterTypes = null)
        {
            if (o == null)
            {
                return false;
            }

            MethodInfo method = null;
            if (parameterTypes != null)
            {
                method = o
                    .GetType()
                    .GetMethod(methodName, parameterTypes);
            }
            else
            {
                method =
                    o.GetType().GetMethod(methodName);

            }

            return method.GetCustomAttributes().Any(attr => attr is HttpPostAttribute);
        }

        public static string ConvertGenericValueToString<T>(this T value)
        {
            if (value == null)
            {
                return null;
            }
            string valueString;
            if (value is DateTime)
            {
                valueString = ((DateTime)(object)value).ToString(Database.DateTimeFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                valueString = value.ToString();
            }
            return valueString;
        }
    }

}
