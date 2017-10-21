using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IServiceDeliveryOrganisationTypeRefDataService : IService<ServiceDeliveryOrganisationTypeRefData>
    {
        bool IsServiceDeliveryOrganisationTypeReferenced(int id);
    }
}