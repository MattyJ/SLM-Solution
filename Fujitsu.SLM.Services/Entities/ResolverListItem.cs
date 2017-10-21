namespace Fujitsu.SLM.Services.Entities
{
    public class ResolverListItem
    {
        public int Id { get; set; }

        public int ServiceDeskId { get; set; }
        public string ServiceDeskName { get; set; }
        public string ServiceDomainName { get; set; }
        public string ServiceFunctionName { get; set; } // Name or alternative
        public int ServiceComponentId { get; set; }
        public string ServiceComponentName { get; set; }
        public string ServiceDeliveryOrganisationTypeName { get; set; }
        public string ServiceDeliveryUnitTypeName { get; set; }
        public string ResolverGroupName { get; set; }
        public string ServiceDeliveryUnitNotes { get; set; }
        public string ServiceDeliveryOrganisationNotes { get; set; }
    }
}