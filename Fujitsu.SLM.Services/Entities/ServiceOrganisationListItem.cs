using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Entities
{
    public class ServiceOrganisationListItem
    {
        public int ServiceDeskId { get; set; }
        public string ServiceDeskName { get; set; }
        public string CustomerName { get; set; }
        public string ServiceDeliveryOrganisationTypeName { get; set; }
        public string ServiceDeliveryUnitTypeName { get; set; }
        public string ResolverGroupTypeName { get; set; }
        public ServiceComponent ServiceComponent { get; set; }
        public string ServiceActivities { get; set; }
        public Resolver Resolver { get; set; }


    }
}