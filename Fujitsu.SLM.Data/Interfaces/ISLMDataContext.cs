using Fujitsu.SLM.Model;
using System;
using System.Data.Entity;

namespace Fujitsu.SLM.Data.Interfaces
{
    public interface ISLMDataContext : IDisposable
    {
        IDbSet<Customer> Customers { get; set; }
        IDbSet<ServiceDesk> ServiceDesks { get; set; }
        IDbSet<DeskInputType> DeskInputTypes { get; set; }
        IDbSet<ServiceDomain> ServiceDomains { get; set; }
        IDbSet<ServiceFunction> ServiceFunctions { get; set; }
        IDbSet<ServiceComponent> ServiceComponents { get; set; }
        IDbSet<Resolver> Resolver { get; set; }
        IDbSet<OperationalProcessType> OperationalProcessType { get; set; }
        IDbSet<Contributor> Contributors { get; set; }
        IDbSet<Diagram> Diagrams { get; set; }
        IDbSet<Asset> Assets { get; set; }

        // Reference Data
        IDbSet<InputTypeRefData> InputTypeRefData { get; set; }
        IDbSet<DomainTypeRefData> DomainTypeRefData { get; set; }
        IDbSet<FunctionTypeRefData> FunctionTypeRefData { get; set; }
        IDbSet<ServiceDeliveryOrganisationTypeRefData> ServiceDeliveryOrganisationTypeRefData { get; set; }
        IDbSet<ServiceDeliveryUnitTypeRefData> ServiceDeliveryUnitTypeRefData { get; set; }
        IDbSet<ResolverGroupTypeRefData> ResolverGroupTypeRefData { get; set; }
        IDbSet<OperationalProcessTypeRefData> OperationalProcessTypeRefData { get; set; }
        IDbSet<ContextHelpRefData> ContextHelpRefData { get; set; }
        IDbSet<RegionTypeRefData> RegionTypeRefData { get; set; }

        // Template Data
        IDbSet<Template> Templates { get; set; }
        IDbSet<TemplateRow> TemplateRows { get; set; }
        IDbSet<TemplateDomain> TemplateDomains { get; set; }
        IDbSet<TemplateFunction> TemplateFunctions { get; set; }
        IDbSet<TemplateComponent> TemplateComponents { get; set; }
        IDbSet<TemplateResolver> TemplateResolvers { get; set; }

        // Audit
        IDbSet<Audit> Audits { get; set; }
    }
}
