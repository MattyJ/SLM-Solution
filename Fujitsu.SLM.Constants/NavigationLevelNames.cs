using Fujitsu.SLM.Enumerations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fujitsu.SLM.Constants
{
    public struct NavigationLevelNames
    {
        public static readonly string None;
        public static readonly string LevelZero;
        public static readonly string LevelOne;
        public static readonly string LevelTwo;
        public static readonly List<string> Descriptions;
        public static readonly List<string> Values;

        static NavigationLevelNames()
        {
            None = string.Empty; // This is meant to be empty string so don't touch ;)
            LevelZero = NavigationLevelName.LevelZero.GetAttributeOfType<DescriptionAttribute>().Description;
            LevelOne = NavigationLevelName.LevelOne.GetAttributeOfType<DescriptionAttribute>().Description;
            LevelTwo = NavigationLevelName.LevelTwo.GetAttributeOfType<DescriptionAttribute>().Description;
            Descriptions = new List<string>
            {
                LevelZero,
                LevelOne,
                LevelTwo
            };
            Values = new List<string>
            {
                NavigationLevelName.LevelZero.ToString(),
                NavigationLevelName.LevelOne.ToString(),
                NavigationLevelName.LevelTwo.ToString()
            };
        }
    }
}