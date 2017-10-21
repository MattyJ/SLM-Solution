using System.Collections.Generic;
using System.ComponentModel;
using Fujitsu.SLM.Enumerations;

namespace Fujitsu.SLM.Constants
{
    public struct ServiceComponentLevelNames
    {
        public static readonly string Level1;
        public static readonly string Level2;
        public static readonly List<string> Descriptions;
        public static readonly List<string> Values;

        static ServiceComponentLevelNames()
        {
            Level1 = ServiceComponentLevel.Level1.GetAttributeOfType<DescriptionAttribute>().Description;
            Level2 = ServiceComponentLevel.Level2.GetAttributeOfType<DescriptionAttribute>().Description;
            Descriptions = new List<string>
            {
                Level1,
                Level2
            };
            Values = new List<string>
            {
                ServiceComponentLevel.Level1.ToString(),
                ServiceComponentLevel.Level2.ToString()
            };
        }
    }
}