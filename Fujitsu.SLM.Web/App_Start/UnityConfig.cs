using Fujitsu.SLM.Core.Caching;
using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Core.Log;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Data.Repository;
using Fujitsu.SLM.DataImporters;
using Fujitsu.SLM.DataImporters.Interfaces;
using Fujitsu.SLM.DataImportProcessors;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Diagrams.Generators;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;
using Fujitsu.SLM.Services;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.TemplateProcessors;
using Fujitsu.SLM.TemplateProcessors.Interface;
using Fujitsu.SLM.Transformers;
using Fujitsu.SLM.Transformers.Interfaces;
using Fujitsu.SLM.Web.Context;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Session;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Diagram = Fujitsu.SLM.Constants.Diagram;

namespace Fujitsu.SLM.Web
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {

            #region SLM Data Context

            const string connectionString = "Name=SLMDataContext";
            var injection = new InjectionConstructor(connectionString);

            container.RegisterType<ILoggingManager, LoggingManager>(new PerRequestLifetimeManager());
            container.RegisterType<ICacheManager, CacheManager>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectBuilder, ObjectBuilder>(new PerRequestLifetimeManager());
            container.RegisterType<ISLMDataContext, SLMDataContext>(new PerRequestLifetimeManager(), injection);
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager());


            #endregion

            #region Identity Data Context

            const string connString = "Name=IdentityDataContext";
            var identityInjection = new InjectionConstructor(connString);

            container.RegisterType<IdentityDataContext>(new PerRequestLifetimeManager(), identityInjection);
            container.RegisterType<ApplicationSignInManager>(new PerRequestLifetimeManager());
            container.RegisterType<ApplicationUserManager>(new PerRequestLifetimeManager());
            container.RegisterType<ApplicationRoleManager>(new PerRequestLifetimeManager());
            container.RegisterType<EmailService>(new PerRequestLifetimeManager());

            container.RegisterType<IAuthenticationManager>(new PerRequestLifetimeManager(),
                new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new PerRequestLifetimeManager(),
                new InjectionConstructor(typeof(IdentityDataContext)));

            container.RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole>>(new PerRequestLifetimeManager(),
                new InjectionConstructor(typeof(IdentityDataContext)));

            #endregion

            #region Repositories

            container.RegisterType<IRepository<Customer>, Repository<Customer>>();
            container.RegisterType<IRepository<Contributor>, Repository<Contributor>>();
            container.RegisterType<IRepository<ServiceDesk>, Repository<ServiceDesk>>();
            container.RegisterType<IRepository<DeskInputType>, Repository<DeskInputType>>();
            container.RegisterType<IRepository<ServiceDomain>, Repository<ServiceDomain>>();
            container.RegisterType<IRepository<ServiceFunction>, Repository<ServiceFunction>>();
            container.RegisterType<IRepository<ServiceComponent>, Repository<ServiceComponent>>();
            container.RegisterType<IRepository<Parameter>, Repository<Parameter>>();
            container.RegisterType<IRepository<Model.Diagram>, Repository<Model.Diagram>>();
            container.RegisterType<IRepository<CustomerPack>, Repository<CustomerPack>>();
            container.RegisterType<IRepository<ServiceComponent>, Repository<ServiceComponent>>();
            container.RegisterType<IRepository<DomainTypeRefData>, Repository<DomainTypeRefData>>();
            container.RegisterType<IRepository<FunctionTypeRefData>, Repository<FunctionTypeRefData>>();
            container.RegisterType<IRepository<InputTypeRefData>, Repository<InputTypeRefData>>();
            container.RegisterType<IRepository<OperationalProcessTypeRefData>, Repository<OperationalProcessTypeRefData>>();
            container.RegisterType<IRepository<ServiceDeliveryOrganisationTypeRefData>, Repository<ServiceDeliveryOrganisationTypeRefData>>();
            container.RegisterType<IRepository<ServiceDeliveryUnitTypeRefData>, Repository<ServiceDeliveryUnitTypeRefData>>();
            container.RegisterType<IRepository<ResolverGroupTypeRefData>, Repository<ResolverGroupTypeRefData>>();
            container.RegisterType<IRepository<Resolver>, Repository<Resolver>>();
            container.RegisterType<IRepository<OperationalProcessType>, Repository<OperationalProcessType>>();
            container.RegisterType<IRepository<ContextHelpRefData>, Repository<ContextHelpRefData>>();
            container.RegisterType<IRepository<RegionTypeRefData>, Repository<RegionTypeRefData>>();
            container.RegisterType<IRepository<ApplicationUser>, Repository<ApplicationUser>>();
            container.RegisterType<IRepository<Template>, Repository<Template>>();
            container.RegisterType<IRepository<TemplateRow>, Repository<TemplateRow>>();
            container.RegisterType<IRepository<TemplateDomain>, Repository<TemplateDomain>>();
            container.RegisterType<IRepository<TemplateFunction>, Repository<TemplateFunction>>();
            container.RegisterType<IRepository<TemplateComponent>, Repository<TemplateComponent>>();
            container.RegisterType<IRepository<TemplateResolver>, Repository<TemplateResolver>>();
            container.RegisterType<IRepository<Asset>, Repository<Asset>>();
            container.RegisterType<IRepository<Audit>, Repository<Audit>>();

            #endregion

            #region Services

            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<IContributorService, ContributorService>();
            container.RegisterType<IServiceDeskService, ServiceDeskService>();
            container.RegisterType<IServiceDomainService, ServiceDomainService>();
            container.RegisterType<IServiceFunctionService, ServiceFunctionService>();
            container.RegisterType<IParameterService, ParameterService>();
            container.RegisterType<IDiagramService, DiagramService>();
            container.RegisterType<ICustomerPackService, CustomerPackService>();
            container.RegisterType<IServiceComponentService, ServiceComponentService>();
            container.RegisterType<IResolverService, ResolverService>();
            container.RegisterType<IServiceDeskService, ServiceDeskService>();
            container.RegisterType<ITemplateService, TemplateService>();
            container.RegisterType<IAssetService, AssetService>();
            container.RegisterType<IAuditService, AuditService>();

            container.RegisterType<IDomainTypeRefDataService, DomainTypeRefDataService>();
            container.RegisterType<IFunctionTypeRefDataService, FunctionTypeRefDataService>();
            container.RegisterType<IInputTypeRefDataService, InputTypeRefDataService>();
            container.RegisterType<IOperationalProcessTypeRefDataService, OperationalProcessTypeRefDataService>();
            container.RegisterType<IServiceDeliveryOrganisationTypeRefDataService, ServiceDeliveryOrganisationTypeRefDataService>();
            container.RegisterType<IServiceDeliveryUnitTypeRefDataService, ServiceDeliveryUnitTypeRefDataService>();
            container.RegisterType<IResolverGroupTypeRefDataService, ResolverGroupTypeRefDataService>();
            container.RegisterType<IContextHelpRefDataService, ContextHelpRefDataService>();
            container.RegisterType<IRegionTypeRefDataService, RegionTypeRefDataService>();

            #endregion

            #region Diagrams

            // Level 0
            container.RegisterType<IDiagramGenerator, FujitsuDomains>(Diagram.FujitsuDomains, new PerRequestLifetimeManager());
            container.RegisterType<IDiagramGenerator, CustomerServices>(Diagram.CustomerServices, new PerRequestLifetimeManager());

            // Level 1 & 2
            container.RegisterType<IDiagramGenerator, ServiceDeskDotMatrix>(Diagram.ServiceDeskDotMatrix, new PerRequestLifetimeManager());
            container.RegisterType<IDiagramGenerator, ServiceDeskStructure>(Diagram.ServiceDeskStructure, new PerRequestLifetimeManager());
            container.RegisterType<IDiagramGenerator, FujitsuServiceOrganisation>(Diagram.FujitsuServiceOrganisation, new PerRequestLifetimeManager());
            container.RegisterType<IDiagramGenerator, CustomerServiceOrganisation>(Diagram.CustomerServiceOrganisation, new PerRequestLifetimeManager());
            container.RegisterType<IDiagramGenerator, CustomerThirdPartyServiceOrganisation>(Diagram.CustomerThirdPartyServiceOrganisation, new PerRequestLifetimeManager());

            #endregion

            #region Importers

            container.RegisterType<IServiceDecompositionImporter, ServiceDecompositionImporter>();
            container.RegisterType<IServiceDecompositionTemplateDataImportProcessor, ServiceDecompositionTemplateDataImportProcessor>(new PerRequestLifetimeManager());
            container.RegisterType<IServiceDecompositionDesignDataImportProcessor, ServiceDecompositionDesignDataImportProcessor>(new PerRequestLifetimeManager());
            container.RegisterType<ITemplateProcessor, TemplateProcessor>();
            container.RegisterType<ITransformSpreadsheetToTemplate, TransformSpreadsheetToTemplate>();
            container.RegisterType<ITransformTemplateToDesign, TransformTemplateToDesign>();

            #endregion

            #region Model Helpers

            container.RegisterType<IServiceComponentHelper, ServiceComponentHelper>();
            container.RegisterType<IResolverHelper, ResolverHelper>();

            #endregion

            #region Context Managers

            container.RegisterType<IContextManager, ContextManager>();
            container.RegisterType<ISessionManager, SessionManager>();
            container.RegisterType<IRequestManager, RequestManager>();
            container.RegisterType<IResponseManager, ResponseManager>();
            container.RegisterType<IApplicationManager, ApplicationManager>();
            container.RegisterType<IUserManager, UserManager>();
            //container.RegisterType<IWindowsContextManager, WindowsContextManager>();
            //container.RegisterType<IWindowsUserManager, WindowsUserManager>();
            container.RegisterType<IAppUserContext, AppUserContext>(new PerRequestLifetimeManager());

            #endregion

            #region Core

            container.RegisterType<IUserIdentity, UserManager>(new InjectionConstructor());

            #endregion

            #region External Initializers

            Fujitsu.Exceptions.Framework.UnityConfig.RegisterTypes(container, () => new PerRequestLifetimeManager());

            #endregion
        }
    }
}
