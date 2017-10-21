using Fujitsu.SLM.Model;
using System.Linq;

namespace Fujitsu.SLM.ModelHelpers
{
    public class ResolverHelper : IResolverHelper
    {
        public bool CanDelete(Resolver resolver)
        {
            return resolver.OperationalProcessTypes == null ||
                    !resolver.OperationalProcessTypes.Any();
        }
    }
}