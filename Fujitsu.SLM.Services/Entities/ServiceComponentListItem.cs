using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Entities
{
    public class ServiceComponentListItem
    {
        public string ServiceDeskName { get; set; }
        public string ServiceDomainName { get; set; }
        public int ServiceFunctionId { get; set; }
        public string ServiceFunctionName { get; set; }
        public ServiceComponent ServiceComponent { get; set; }
    }
}