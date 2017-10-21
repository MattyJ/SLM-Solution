using System.Linq;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.ModelHelpers
{
    public class ServiceComponentHelper : IServiceComponentHelper
    {
        public ServiceComponentEditState GetEditState(ServiceComponent serviceComponent)
        {
            if (serviceComponent == null)
            {
                return ServiceComponentEditState.None;
            }
            if (serviceComponent.ComponentLevel == (int) ServiceComponentLevel.Level2)
            {
                return ServiceComponentEditState.Level2;
            }
            if ((serviceComponent.ChildServiceComponents == null || !serviceComponent.ChildServiceComponents.Any()) &&
                serviceComponent.Resolver == null &&
                serviceComponent.ComponentLevel == (int) ServiceComponentLevel.Level1)
            {
                return ServiceComponentEditState.Level1WithNoChildComponentOrResolver;
            }
            if (serviceComponent.ChildServiceComponents != null && serviceComponent.ChildServiceComponents.Any() &&
                serviceComponent.ComponentLevel == (int) ServiceComponentLevel.Level1)
            {
                return ServiceComponentEditState.Level1WithChildComponent;
            }
            if ((serviceComponent.Resolver != null) &&
                serviceComponent.ComponentLevel == (int) ServiceComponentLevel.Level1)
            {
                return ServiceComponentEditState.Level1WithResolver;
            }

            return ServiceComponentEditState.None;
        }

        public bool CanDelete(ServiceComponent serviceComponent)
        {
            return serviceComponent != null &&
                   (serviceComponent.ChildServiceComponents == null || !serviceComponent.ChildServiceComponents.Any()) &&
                   serviceComponent.Resolver == null;
        }
    }
}