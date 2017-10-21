using Fujitsu.SLM.Enumerations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fujitsu.SLM.Constants
{
    public struct TemplateTypeNames
    {
        public static readonly string SLM;
        public static readonly string SORT;
        public static readonly List<string> Descriptions;
        public static readonly List<string> Values;

        static TemplateTypeNames()
        {
            SLM = TemplateType.SLM.GetAttributeOfType<DescriptionAttribute>().Description;
            SORT = TemplateType.SORT.GetAttributeOfType<DescriptionAttribute>().Description;
            Descriptions = new List<string>
            {
                SLM,
                SORT
            };
            Values = new List<string>
            {
                TemplateType.SLM.ToString(),
                TemplateType.SORT.ToString(),
            };
        }
    }
}