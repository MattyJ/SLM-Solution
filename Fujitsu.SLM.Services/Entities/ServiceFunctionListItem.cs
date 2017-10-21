using System;

namespace Fujitsu.SLM.Services.Entities
{
    public class ServiceFunctionListItem
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ServiceDeskName { get; set; }
        public int ServiceDomainId { get; set; }
        public string ServiceDomainName { get; set; }
        public int FunctionTypeId { get; set; }
        public string FunctionName { get; set; }
        public string AlternativeName { get; set; }
        public int DiagramOrder { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
