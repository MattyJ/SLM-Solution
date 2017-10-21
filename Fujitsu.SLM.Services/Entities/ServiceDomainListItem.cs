using System;

namespace Fujitsu.SLM.Services.Entities
{
    public class ServiceDomainListItem
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int ServiceDeskId { get; set; }
        public string ServiceDeskName { get; set; }
        public int DomainTypeId { get; set; }
        public string DomainName { get; set; }
        public string AlternativeName { get; set; }
        public int DiagramOrder { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
