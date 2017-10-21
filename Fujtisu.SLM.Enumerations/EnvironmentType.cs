using System.ComponentModel;

namespace Fujitsu.SLM.Enumerations
{
    public enum EnvironmentType
    {
        [Description("Dev")]
        Development,

        [Description("Test")]
        Test,

        [Description("Prod")]
        Production
    }
}