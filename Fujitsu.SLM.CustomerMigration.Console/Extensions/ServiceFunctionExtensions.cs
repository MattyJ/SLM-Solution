using Fujitsu.SLM.Data;
using Fujitsu.SLM.Model;
using System.Data.Entity;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Extensions
{
    public static class ServiceFunctionExtensions
    {
        public static ServiceFunction GetSourceFunction(this ServiceFunction value, int serviceFunctionId)
        {
            using (var sourceDb = new SLMDataContext("Name=SLMSourceDataContext"))
            {
                // For expediency grab everything and store in memory
                return sourceDb.ServiceFunctions.AsNoTracking()
                    .Include(c => c.ServiceDomain)
                    .Include(c => c.FunctionType)
                    .Include(c => c.ServiceComponents)
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver.OperationalProcessTypes.Select(op => op.OperationalProcessTypeRefData)))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver.ServiceDeliveryOrganisationType))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver.ServiceDeliveryUnitType))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver.ResolverGroupType))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver.ServiceComponent))
                    .Include(c => c.ServiceComponents.Select(r => r.Resolver.ServiceDesk))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver)))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.OperationalProcessTypes.Select(op => op.OperationalProcessTypeRefData))))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceDeliveryOrganisationType)))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceDeliveryUnitType)))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ResolverGroupType)))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceComponent)))
                    .Include(c => c.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceDesk)))
                    .FirstOrDefault(x => x.Id == serviceFunctionId);
            }
        }
    }
}