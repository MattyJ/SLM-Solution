using Fujitsu.SLM.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Fujitsu.SLM.Extensions
{
    public static class StringExtensions
    {
        public static bool SafeEquals(this string value, string compare, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (value == null)
            {
                return compare == null;
            }

            return value.Equals(compare, comparison);
        }

        public static bool SafeEquals(this string value, string[] compares, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (value == null)
            {
                return false;
            }

            return compares.Any(str => value.Equals(str, comparison));
        }

        public static bool SafeContains(this string value, string compare)
        {
            if (value == null)
            {
                return compare == null;
            }

            return value.ToUpper().Contains(compare.ToUpper());
        }


        public static string SafeTrim(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim();
        }


        public static string SafeTrim(this string value, int maxLength)
        {
            if (value == null)
            {
                return string.Empty;
            }

            value = value.Trim();

            if (value.Length > maxLength)
            {
                return value.Substring(0, maxLength).Trim();
            }
            else
            {
                return value;
            }
        }

        public static int? ToInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int tmpInt;
            if (int.TryParse(value, out tmpInt))
            {
                return tmpInt;
            }
            else
            {
                throw new ArgumentException("Invalid Integer value : " + value);
            }
        }

        public static decimal? ToDecimal(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            decimal tmpDecimal;
            if (decimal.TryParse(value, out tmpDecimal))
            {
                return tmpDecimal;
            }
            else
            {
                throw new ArgumentException("Invalid Decimal value : " + value);
            }
        }

        public static bool ToBool(this string value)
        {
            if (value.SafeEquals("yes") || value.SafeEquals("y") || value.SafeEquals("true") || value.SafeEquals("t") || (value.SafeEquals("1")))
            {
                return true;
            }
            else if (value.SafeEquals("no") || value.SafeEquals("n") || value.SafeEquals("false") || value.SafeEquals("f") || (value.SafeEquals("0")))
            {
                return false;
            }

            throw new ArgumentException("Invalid Boolean value : " + value);
        }

        public static DateTime? ToDateTime(this string value)
        {
            DateTime d;

            if (value == null)
            {
                return null;
            }

            var s = value.Trim().ToUpper();

            if (s.Length == 0)
            {
                return null;
            }
            if (DateTime.TryParse(s, out d))
            {
                return d;
            }
            if ((s.Length == 4) && (s.Substring(2, 1).ToUpper() == "Q"))
            {
                // Year and Quarter, i.e. 12Q1 = Quarter 1, 2012
                int quarter, year;
                if (int.TryParse(s.Substring(0, 2), out year) && int.TryParse(s.Substring(3, 1), out quarter))
                {
                    switch (quarter)
                    {
                        case 1:
                            return new DateTime(year + 2000, 4, 1);  // 13Q1 = 1.Apr.2014
                        case 2:
                            return new DateTime(year + 2000, 7, 1);  // 13Q2 = 1.Jul.2014
                        case 3:
                            return new DateTime(year + 2000, 10, 1);  // 13Q3 = 1.Oct.2014
                        case 4:
                            return new DateTime(year + 2001, 1, 1);   // 13Q4 = 1.Jan.2014
                        default:
                            break;
                    }
                }
            }

            throw new ArgumentException("Invalid Date value : " + value);
        }

        public static T ConvertTo<T>(this string value)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value);
            }

            var convertor = TypeDescriptor.GetConverter(typeof(T));
            return (T)convertor.ConvertFromString(value);
        }

        public static T ConvertStringToGenericValue<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            T result;

            if (typeof(T) == typeof(DateTime))
            {
                result = (T)(object)DateTime.ParseExact(value, Database.DateTimeFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                result = value.ConvertTo<T>();
            }

            return result;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        private static readonly HashSet<char> DefaultNonWordCharacters
          = new HashSet<char> { ',', '.', ':', ';' };

        public static string CropWholeWords(
          this string value,
          int length,
          HashSet<char> nonWordCharacters = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (length < 0)
            {
                throw new ArgumentException("Negative values not allowed.", nameof(length));
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            if (length >= value.Length)
            {
                return value;
            }
            int end = length;

            for (int i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace())
                {
                    break;
                }

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                {
                    //Removing a character that isn't whitespace but not part
                    //of the word either (ie ".") given that the character is
                    //followed by whitespace or the end of the string makes it
                    //possible to include the word, so we do that.
                    break;
                }
                end--;
            }

            if (end == 0)
            {
                //Return nothing at all if the first word is longer than the length, could
                // also favour returing what we have (e.g. end = length)
                end = 0;
            }

            return value.Substring(0, end);
        }

        public static Dictionary<int, string> CropWholeWordsIntoChunks(
          this string value,
          int chunks,
          int length,
          HashSet<char> nonWordCharacters = null)
        {
            var result = new Dictionary<int, string>();

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (chunks < 1)
            {
                throw new ArgumentException("Value less than one is not allowed.", nameof(chunks));
            }

            if (length < 0)
            {
                throw new ArgumentException("Negative values not allowed.", nameof(length));
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            var completed = false;
            for (var i = 1; i <= chunks; i++)
            {
                if (!completed)
                {
                    var stringValue = value.CropWholeWords(length, nonWordCharacters);
                    result.Add(i, stringValue);
                    if (stringValue.Length + 1 < value.Length)
                    {
                        value = value.Substring(stringValue.Length + 1);
                    }
                    else
                    {
                        completed = true;
                    }
                }
                else
                {
                    // Fill remaining dictionary values with empty strings
                    result.Add(i, string.Empty);
                }

            }

            return result;
        }

        private static bool IsWhitespace(this char character)
        {
            return character == ' ';
        }
    }

}
