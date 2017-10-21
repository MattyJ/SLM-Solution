using Fujitsu.SLM.Enumerations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fujitsu.SLM.Constants
{
    public struct ServiceDeliveryOrganisationNames
    {
        public static readonly string Fujitsu;
        public static readonly string Customer;
        public static readonly string CustomerThirdParty;
        public static readonly List<string> Descriptions;
        public static readonly List<string> Values;

        static ServiceDeliveryOrganisationNames()
        {
            Fujitsu = ServiceDeliveryOrganisationType.Fujitsu.GetAttributeOfType<DescriptionAttribute>().Description;
            Customer = ServiceDeliveryOrganisationType.Customer.GetAttributeOfType<DescriptionAttribute>().Description;
            CustomerThirdParty = ServiceDeliveryOrganisationType.CustomerThirdParty.GetAttributeOfType<DescriptionAttribute>().Description;
            Descriptions = new List<string>
            {
                Fujitsu,
                Customer,
                CustomerThirdParty
            };
            Values = new List<string>
            {
                ServiceDeliveryOrganisationType.Fujitsu.ToString(),
                ServiceDeliveryOrganisationType.Customer.ToString(),
                ServiceDeliveryOrganisationType.CustomerThirdParty.ToString()
            };
        }
    }
}
