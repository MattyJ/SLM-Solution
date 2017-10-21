using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.SLM.Data
{
    [ExcludeFromCodeCoverage]
    public class SLMDataContext : DbContext, ISLMDataContext
    {
        static SLMDataContext()
        {
            Database.SetInitializer<SLMDataContext>(null);
            //SqlProviderServices.TruncateDecimalsToScale = false;
        }

        public SLMDataContext()
            : base("SLMDataContext")
        {
        }

        public SLMDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<ServiceDesk> ServiceDesks { get; set; }
        public IDbSet<DeskInputType> DeskInputTypes { get; set; }
        public IDbSet<Parameter> Parameters { get; set; }
        public IDbSet<ServiceDomain> ServiceDomains { get; set; }
        public IDbSet<ServiceFunction> ServiceFunctions { get; set; }
        public IDbSet<ServiceComponent> ServiceComponents { get; set; }
        public IDbSet<Resolver> Resolver { get; set; }
        public IDbSet<OperationalProcessType> OperationalProcessType { get; set; }
        public IDbSet<Diagram> Diagrams { get; set; }
        public IDbSet<CustomerPack> CustomerPacks { get; set; }
        public IDbSet<Contributor> Contributors { get; set; }
        public IDbSet<Asset> Assets { get; set; }

        // Reference Data
        public IDbSet<InputTypeRefData> InputTypeRefData { get; set; }
        public IDbSet<DomainTypeRefData> DomainTypeRefData { get; set; }
        public IDbSet<FunctionTypeRefData> FunctionTypeRefData { get; set; }
        public IDbSet<ServiceDeliveryOrganisationTypeRefData> ServiceDeliveryOrganisationTypeRefData { get; set; }
        public IDbSet<ServiceDeliveryUnitTypeRefData> ServiceDeliveryUnitTypeRefData { get; set; }
        public IDbSet<ResolverGroupTypeRefData> ResolverGroupTypeRefData { get; set; }
        public IDbSet<OperationalProcessTypeRefData> OperationalProcessTypeRefData { get; set; }
        public IDbSet<ContextHelpRefData> ContextHelpRefData { get; set; }
        public IDbSet<RegionTypeRefData> RegionTypeRefData { get; set; }

        // Template Data
        public IDbSet<Template> Templates { get; set; }
        public IDbSet<TemplateRow> TemplateRows { get; set; }
        public IDbSet<TemplateDomain> TemplateDomains { get; set; }
        public IDbSet<TemplateFunction> TemplateFunctions { get; set; }
        public IDbSet<TemplateComponent> TemplateComponents { get; set; }
        public IDbSet<TemplateResolver> TemplateResolvers { get; set; }

        // Audit
        public IDbSet<Audit> Audits { get; set; }

        // Fluent API
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // 0..1 to Many recursive relationship
            modelBuilder.Entity<ServiceComponent>()
                .HasOptional(x => x.ParentServiceComponent)
                .WithMany(x => x.ChildServiceComponents)
                .HasForeignKey(x => x.ParentServiceComponentId);

            // 0..1 to 0..1 relationship between Service Component and Resolver with foreign keys
            modelBuilder.Entity<ServiceComponent>()
                .HasOptional(x => x.Resolver)
                .WithOptionalPrincipal()
                .Map(x => x.MapKey("ServiceComponent_Id"));

            modelBuilder.Entity<Resolver>()
                .HasOptional(x => x.ServiceComponent)
                .WithOptionalPrincipal()
                .Map(x => x.MapKey("ResolverId"));

            modelBuilder.Entity<TemplateFunction>()
                .HasMany(x => x.TemplateComponents);

            // 0..1 to Many recursive relationship
            modelBuilder.Entity<TemplateComponent>()
                .HasOptional(x => x.ParentTemplateComponent)
                .WithMany(x => x.ChildTemplateComponents)
                .HasForeignKey(x => x.ParentTemplateComponentId);
        }
    }
}