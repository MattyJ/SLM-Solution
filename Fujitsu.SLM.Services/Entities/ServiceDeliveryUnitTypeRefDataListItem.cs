namespace Fujitsu.SLM.Services.Entities
{
    public class ServiceDeliveryUnitTypeRefDataListItem
    {
        public int Id { get; set; }

        public string ServiceDeliveryUnitTypeName { get; set; }

        public int SortOrder { get; set; }

        public bool Visible { get; set; }

        public int UsageCount { get; set; }
    }
}