using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.ModelHelpers
{
    public interface IServiceComponentHelper
    {
        ServiceComponentEditState GetEditState(ServiceComponent serviceComponent);
        bool CanDelete(ServiceComponent serviceComponent);
    }
}