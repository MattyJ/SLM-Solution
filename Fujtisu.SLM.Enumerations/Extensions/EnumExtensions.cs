using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace Fujitsu.SLM.Enumerations
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static List<SelectListItem> AsSelectListItems<T>(bool useEnumInt = false)
        {
            // set response item
            var response = new List<SelectListItem>();

            // get the type of the enum and types
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType);

            // iterate the types
            foreach (var value in values)
            {
                // get the reflection field info and attributes
                var fieldInfo = value.GetType().GetField(value.ToString());
                var attributes =
                    (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                // get the "default" text as the value
                var text = value.ToString();

                // if we have a description use as the text
                if (attributes.Length > 0)
                {
                    text = attributes[0].Description;
                }

                // add a new select list item
                response.Add(new SelectListItem
                {
                    Text = text,
                    Value = useEnumInt ? ((int)value).ToString() : value.ToString()
                });

            }

            return response;
        }

        public static Dictionary<string, string> ConvertEnumAndDescriptionToList<T>()
        {
            // set response item
            var response = new Dictionary<string, string>();

            // get the type of the enum and types
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType);

            // iterate the types
            foreach (var value in values)
            {
                // get the reflection field info and attributes
                var fieldInfo = value.GetType().GetField(value.ToString());
                var attributes =
                    (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                // if we have a description use as the text
                if (attributes.Length > 0)
                {
                    response.Add(value.ToString(), attributes[0].Description);
                }
            }

            return response;
        }

        public static List<string> ConvertEnumToList<T>()
        {
            // set response item
            var response = new List<string>();

            // get the type of the enum and types
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType);

            // iterate the types
            foreach (var value in values)
            {
                response.Add(value.ToString());
            }

            return response;
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToEnumText<T>(this int value)
        {
            var e = (T)(object)value;
            return e.ToString();
        }
    }
}
