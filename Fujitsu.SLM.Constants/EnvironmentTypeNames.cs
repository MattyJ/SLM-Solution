using Fujitsu.SLM.Enumerations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fujitsu.SLM.Constants
{
    public struct EnvironmentTypeNames
    {
        public static readonly string Development;
        public static readonly string Test;
        public static readonly string Production;
        public static readonly List<string> Descriptions;
        public static readonly List<string> Values;

        static EnvironmentTypeNames()
        {
            Development = EnvironmentType.Development.GetAttributeOfType<DescriptionAttribute>().Description;
            Test = EnvironmentType.Test.GetAttributeOfType<DescriptionAttribute>().Description;
            Production = EnvironmentType.Production.GetAttributeOfType<DescriptionAttribute>().Description;
            Descriptions = new List<string>
            {
                Development,
                Test,
                Production
            };
            Values = new List<string>
            {
                EnvironmentType.Development.ToString(),
                EnvironmentType.Test.ToString(),
                EnvironmentType.Production.ToString()
            };
        }
    }
}