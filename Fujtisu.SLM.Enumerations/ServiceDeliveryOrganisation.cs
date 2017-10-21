using System.ComponentModel;

namespace Fujitsu.SLM.Enumerations
{
    public enum ServiceDeliveryOrganisationType
    {
        [Description("Fujitsu")]
        Fujitsu = 1,
        [Description("Customer")]
        Customer = 2,
        [Description("Customer Third Party")]
        CustomerThirdParty = 3
    }
}