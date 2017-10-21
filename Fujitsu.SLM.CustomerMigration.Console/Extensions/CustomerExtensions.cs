using Fujitsu.SLM.Data;
using Fujitsu.SLM.Model;
using System.Data.Entity;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Extensions
{
    public static class CustomerExtensions
    {
        public static Customer GetSourceCustomer(this Customer value, int sourceCustomerId)
        {
            using (var sourceDb = new SLMDataContext("Name=SLMSourceDataContext"))
            {
                // For expediency grab everything and store in memory
                return sourceDb.Customers.AsNoTracking()
                    .Include(c => c.Contributors)
                    .Include(c => c.ServiceDesks.Select(s => s.DeskInputTypes.Select(i => i.InputTypeRefData)))
                    .Include(c => c.ServiceDesks.Select(s => s.Resolvers.Select(r => r.OperationalProcessTypes)))
                    .Include(c => c.ServiceDesks.Select(s => s.Resolvers.Select(r => r.ServiceDeliveryOrganisationType)))
                    .Include(c => c.ServiceDesks.Select(s => s.Resolvers.Select(r => r.ServiceDeliveryUnitType)))
                    .Include(c => c.ServiceDesks.Select(s => s.Resolvers.Select(r => r.ResolverGroupType)))
                    .Include(c => c.ServiceDesks.Select(s => s.Resolvers.Select(sc => sc.ServiceComponent)))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.DomainType)))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions)))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(ft => ft.FunctionType))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver.OperationalProcessTypes.Select(op => op.OperationalProcessTypeRefData))))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver.ServiceDeliveryOrganisationType)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver.ServiceDeliveryUnitType)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver.ResolverGroupType)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver.ServiceComponent)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(r => r.Resolver.ServiceDesk)))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver))))))
                    .Include(
                        c =>
                            c.ServiceDesks.Select(
                                s =>
                                    s.ServiceDomains.Select(
                                        d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.OperationalProcessTypes.Select(op => op.OperationalProcessTypeRefData)))))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceDeliveryOrganisationType))))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceDeliveryUnitType))))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ResolverGroupType))))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceComponent))))))
                    .Include(c => c.ServiceDesks.Select(s => s.ServiceDomains.Select(d => d.ServiceFunctions.Select(sc => sc.ServiceComponents.Select(cc => cc.ChildServiceComponents.Select(ccr => ccr.Resolver.ServiceDesk))))))
                    .FirstOrDefault(x => x.Id == sourceCustomerId);
            }
        }
    }

}