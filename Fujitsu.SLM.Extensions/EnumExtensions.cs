using System;

namespace Fujitsu.SLM.Extensions
{
    public static class EnumExtensions
    {
        public static int GetEnumIntFromText<T>(this string enumText)
        {
            var e = Enum.Parse(typeof (T), enumText, true) as Enum;
            return Convert.ToInt32(e);
        }
    }
}