namespace Fujitsu.SLM.Services.Entities
{
    public class TemplateRowListItem
    {
        public int Id { get; set; }
        public string ServiceDomain { get; set; }

        public string ServiceFunction { get; set; }

        public string ServiceComponentLevel1 { get; set; }

        public string ServiceComponentLevel2 { get; set; }

        public string ServiceActivities { get; set; }

        public string ServiceDeliveryOrganisation { get; set; }

        public string ServiceDeliveryUnit { get; set; }

        public string ResolverGroup { get; set; }
    }
}