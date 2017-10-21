using System.ComponentModel;

namespace Fujitsu.SLM.Enumerations
{
    public enum NavigationLevelName
    {
        [Description("LevelZero")]
        LevelZero = 0,
        [Description("LevelOne")]
        LevelOne = 1,
        [Description("LevelTwo")]
        LevelTwo = 2,
        [Description("None")]
        None = 99
    }
}