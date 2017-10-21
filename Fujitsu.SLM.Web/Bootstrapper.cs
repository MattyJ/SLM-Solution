using Fujitsu.SLM.Data;
using Fujitsu.SLM.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Web
{
    using AutoMapper;
    using Enumerations;
    using System.ComponentModel;
    using Diagram = Diagrams.Entities;
    using Model = Model;
    using Service = Services.Entities;
    using Web = Models;

    //TODO: Review commented out Service Activities

    public static class Bootstrapper
    {
        internal static void SetupAutoMapper()
        {
            SetupAutoMapperLocal();
            SetupAutoMapperExternal();
        }

        internal static void SetupAutoMapperExternal()
        {
        }

        internal static void SetupAutoMapperLocal()
        {
            Mapper.CreateMap<Model.Customer, Web.CustomerViewModel>()
                .ForMember(src => src.ReturnUrl, opt => opt.Ignore())
                .ForMember(src => src.Owner, opt => opt.Ignore());

            Mapper.CreateMap<Web.CustomerViewModel, Model.Customer>()
                .ForMember(src => src.ServiceDesks, opt => opt.Ignore())
                .ForMember(src => src.Contributors, opt => opt.Ignore())
                .ForMember(src => src.Audits, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.Contributor, Web.ContributorViewModel>()
                .ForMember(dest => dest.CustomerId, x => x.MapFrom(y => y.Customer.Id))
                .ForMember(dest => dest.CustomerName, x => x.MapFrom(y => y.Customer.CustomerName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            Mapper.CreateMap<Web.BulkCustomerContributorViewModel, Model.Contributor>()
                .ForMember(src => src.Customer, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.Contributor, Web.BulkCustomerContributorViewModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName));

            Mapper.CreateMap<Model.DeskInputType, Model.InputTypeRefData>()
                .ForMember(src => src.Id, x => x.MapFrom(y => y.InputTypeRefData.Id))
                .ForMember(src => src.InputTypeNumber, x => x.MapFrom(y => y.InputTypeRefData.InputTypeNumber))
                .ForMember(src => src.Default, x => x.MapFrom(y => y.InputTypeRefData.Default))
                .ForMember(src => src.SortOrder, x => x.MapFrom(y => y.InputTypeRefData.SortOrder))
                .ForMember(src => src.InputTypeName, x => x.MapFrom(y => y.InputTypeRefData.InputTypeName));

            Mapper.CreateMap<Service.ServiceDeskListItem, Web.ServiceDeskViewModel>()
                .ForMember(dest => dest.CustomerId, src => src.Ignore())
                .ForMember(dest => dest.CustomerName, src => src.Ignore());

            Mapper.CreateMap<Model.ServiceDesk, Web.ServiceDeskViewModel>()
                .ForMember(src => src.CustomerName, x => x.MapFrom(y => y.Customer.CustomerName))
                .ForMember(src => src.DeskInputTypes, x => x.MapFrom(y => y.DeskInputTypes));

            Mapper.CreateMap<Web.ServiceDeskViewModel, Model.ServiceDesk>()
                .ForMember(src => src.Customer, opt => opt.Ignore())
                .ForMember(src => src.DeskInputTypes, opt => opt.Ignore())
                .ForMember(src => src.ServiceDomains, opt => opt.Ignore())
                .ForMember(src => src.Resolvers, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.ServiceDomain, Service.ServiceDomainListItem>()
                .ForMember(dest => dest.CustomerName, src => src.Ignore())
                .ForMember(dest => dest.ServiceDeskName, src => src.Ignore())
                .ForMember(dest => dest.DomainName, src => src.Ignore());

            Mapper.CreateMap<Service.ServiceDomainListItem, Web.SearchServiceDomainViewModel>()
                .ForMember(src => src.DecompositionName, opt => opt.Ignore())
                .ForMember(src => src.DeskName, opt => opt.Ignore())
                .ForMember(src => src.ServiceFunctions, opt => opt.Ignore());

            Mapper.CreateMap<Web.SearchServiceDomainViewModel, Model.ServiceDomain>()
                .ForMember(src => src.DomainTypeId, opt => opt.Ignore())
                .ForMember(src => src.DomainType, opt => opt.Ignore())
                .ForMember(src => src.AlternativeName, opt => opt.Ignore())
                .ForMember(src => src.DiagramOrder, opt => opt.Ignore())
                .ForMember(src => src.ServiceDeskId, opt => opt.Ignore())
                .ForMember(src => src.ServiceDesk, opt => opt.Ignore())
                .ForMember(src => src.ServiceFunctions, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Service.ServiceDomainListItem, Web.ServiceDomainViewModel>();

            Mapper.CreateMap<Model.ServiceDomain, Web.ServiceDomainViewModel>()
                .ForMember(dest => dest.ServiceDeskName, opt => opt.MapFrom(src => src.ServiceDesk.DeskName))
                .ForMember(dest => dest.DomainTypeId, opt => opt.MapFrom(src => src.DomainTypeId))
                .ForMember(dest => dest.DomainName, opt => opt.MapFrom(src => src.DomainType.DomainName))
                .ForMember(dest => dest.AlternativeName, opt => opt.MapFrom(src => src.AlternativeName))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src.DiagramOrder));

            Mapper.CreateMap<Model.ServiceDomain, Web.BulkServiceDomainViewModel>()
                .ForMember(dest => dest.ServiceDeskName, opt => opt.MapFrom(src => src.ServiceDesk.DeskName))
                .ForMember(dest => dest.DomainTypeId, opt => opt.MapFrom(src => src.DomainTypeId))
                .ForMember(dest => dest.DomainName, opt => opt.MapFrom(src => src.DomainType.DomainName))
                .ForMember(dest => dest.AlternativeName, opt => opt.MapFrom(src => src.AlternativeName))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src.DiagramOrder));

            Mapper.CreateMap<Web.ServiceDomainViewModel, Model.ServiceDomain>()
                .ForMember(src => src.DomainType, opt => opt.Ignore())
                .ForMember(src => src.ServiceDesk, opt => opt.Ignore())
                .ForMember(src => src.ServiceFunctions, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Web.BulkServiceDomainViewModel, Model.ServiceDomain>()
                .ForMember(src => src.ServiceFunctions, opt => opt.Ignore())
                .ForMember(src => src.DomainType, opt => opt.Ignore())
                .ForMember(src => src.ServiceDesk, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.ServiceFunction, Service.ServiceFunctionListItem>()
                .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceDeskName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceDomainName, opt => opt.Ignore())
                .ForMember(dest => dest.FunctionName, opt => opt.Ignore());

            Mapper.CreateMap<Service.ServiceFunctionListItem, Web.ServiceFunctionViewModel>();

            Mapper.CreateMap<Model.ServiceFunction, Web.ServiceFunctionViewModel>()
                .ForMember(dest => dest.ServiceDomainName, opt => opt.MapFrom(src => src.ServiceDomain.DomainType.DomainName))
                .ForMember(dest => dest.ServiceDeskName, opt => opt.Ignore())
                .ForMember(dest => dest.FunctionTypeId, opt => opt.MapFrom(src => src.FunctionTypeId))
                .ForMember(dest => dest.FunctionName, opt => opt.MapFrom(src => src.FunctionType.FunctionName));

            Mapper.CreateMap<Model.ServiceFunction, Web.BulkServiceFunctionViewModel>()
                .ForMember(dest => dest.ServiceDomainName, opt => opt.MapFrom(src => src.ServiceDomain.DomainType.DomainName))
                .ForMember(dest => dest.ServiceDeskName, opt => opt.Ignore())
                .ForMember(dest => dest.FunctionTypeId, opt => opt.MapFrom(src => src.FunctionTypeId))
                .ForMember(dest => dest.FunctionName, opt => opt.MapFrom(src => src.FunctionType.FunctionName));

            Mapper.CreateMap<Web.ServiceFunctionViewModel, Model.ServiceFunction>()
                .ForMember(src => src.FunctionType, opt => opt.Ignore())
                .ForMember(src => src.ServiceDomain, opt => opt.Ignore())
                .ForMember(src => src.ServiceComponents, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Web.BulkServiceFunctionViewModel, Model.ServiceFunction>()
                .ForMember(src => src.FunctionType, opt => opt.Ignore())
                .ForMember(src => src.ServiceDomain, opt => opt.Ignore())
                .ForMember(src => src.ServiceComponents, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            // Reference Data
            Mapper.CreateMap<Service.DomainTypeRefDataListItem, Web.DomainTypeRefDataViewModel>();

            Mapper.CreateMap<Web.DomainTypeRefDataViewModel, Model.DomainTypeRefData>();

            Mapper.CreateMap<Service.FunctionTypeRefDataListItem, Web.FunctionTypeRefDataViewModel>();

            Mapper.CreateMap<Web.FunctionTypeRefDataViewModel, Model.FunctionTypeRefData>();

            Mapper.CreateMap<Service.InputTypeRefDataListItem, Web.InputTypeRefDataViewModel>();

            Mapper.CreateMap<Web.InputTypeRefDataViewModel, Model.InputTypeRefData>();

            Mapper.CreateMap<Model.DeskInputType, Model.InputTypeRefData>();

            Mapper.CreateMap<Model.Parameter, Web.ParameterViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(m => m.Type.GetAttributeOfType<DescriptionAttribute>().Description));

            Mapper.CreateMap<Web.ServiceDeskViewModel, Web.EditServiceDeskViewModel>()
                .ForMember(src => src.ServiceDesk, opt => opt.Ignore())
                .ForMember(src => src.CanMoveServiceDomain, opt => opt.Ignore())
                .ForMember(src => src.EditLevel, opt => opt.Ignore())
                .ForMember(src => src.ReturnUrl, opt => opt.Ignore())
                .ForMember(src => src.CanImportServiceConfiguratorTemplate, opt => opt.Ignore())
                .ForMember(src => src.CanImportServiceLandscapeTemplate, opt => opt.Ignore());

            Mapper.CreateMap<Model.Diagram, Web.DiagramViewModel>();

            Mapper.CreateMap<Model.CustomerPack, Web.CustomerPackViewModel>();

            Mapper.CreateMap<ApplicationUser, Web.UserViewModel>()
                .ForMember(src => src.RegionName, opt => opt.Ignore());

            Mapper.CreateMap<Service.ServiceComponentListItem, Web.ServiceComponentViewModel>()
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(m => m.ServiceComponent.ServiceComponentHelper().CanDelete(m.ServiceComponent)))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(m => m.ServiceComponent.Id))
                .ForMember(dest => dest.ComponentLevel, opt => opt.MapFrom(m => m.ServiceComponent.ComponentLevel))
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(m => m.ServiceComponent.ComponentName))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(m => m.ServiceComponent.DiagramOrder))
                .ForMember(dest => dest.ServiceActivities, opt => opt.MapFrom(m => m.ServiceComponent.ServiceActivities))
                .ForMember(dest => dest.InsertedBy, opt => opt.MapFrom(m => m.ServiceComponent.InsertedBy))
                .ForMember(dest => dest.InsertedDate, opt => opt.MapFrom(m => m.ServiceComponent.InsertedDate))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(m => m.ServiceComponent.UpdatedBy))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(m => m.ServiceComponent.UpdatedDate))
                .ForMember(dest => dest.ParentServiceComponentId, opt => opt.MapFrom(m => m.ServiceComponent.ParentServiceComponentId));

            Mapper.CreateMap<Model.ServiceComponent, Web.ServiceComponentViewModel>()
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(m => m.ServiceComponentHelper().CanDelete(m)))
                .ForMember(dest => dest.ServiceDeskName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceDomainName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceFunctionId, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceFunctionName, opt => opt.Ignore());

            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentNameViewModel>()
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src.ComponentName));

            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentDiagramOrderViewModel>()
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src.DiagramOrder));

            Mapper.CreateMap<Model.ServiceComponent, Web.ServiceActivityViewModel>()
                .ForMember(dest => dest.ServiceActivities, opt => opt.MapFrom(src => src.ServiceActivities));

            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentChildViewModel>()
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src.ComponentName))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src.DiagramOrder));

            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentNameLevelViewModel>()
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src.ComponentName))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src.DiagramOrder))
                .ForMember(dest => dest.ComponentLevel, opt => opt.MapFrom(src => src.ComponentLevel.ToEnumText<ServiceComponentLevel>()));

            Mapper.CreateMap<Model.Resolver, Web.OperationalProcessTypesViewModel>()
                .ForMember(dest => dest.OperationalProcessTypes, opt => opt.MapFrom(src => src.OperationalProcessTypes.Select(s => s.OperationalProcessTypeRefDataId).ToArray()));

            Mapper.CreateMap<Model.Resolver, Web.EditResolverServiceDeliveryOrganisationViewModel>()
                .ForMember(dest => dest.ServiceDeliveryOrganisationTypeId,
                    opt => opt.MapFrom(src => src.ServiceDeliveryOrganisationType.Id))
                .ForMember(dest => dest.ServiceDeliveryOrganisationNotes,
                    opt => opt.MapFrom(src => src.ServiceDeliveryOrganisationNotes));

            Mapper.CreateMap<Model.Resolver, Web.EditResolverServiceDeliveryUnitViewModel>()
                .ForMember(dest => dest.ServiceDeliveryUnitTypeId,
                    opt => opt.MapFrom(src => src.ServiceDeliveryUnitType.Id))
                .ForMember(dest => dest.ServiceDeliveryUnitNotes, opt => opt.MapFrom(src => src.ServiceDeliveryUnitNotes));

            Mapper.CreateMap<Model.Resolver, Web.EditResolverServiceDeliveryUnitLevelZeroViewModel>()
                .ForMember(dest => dest.ServiceDeliveryUnitTypeId,
                    opt => opt.MapFrom(src => src.ServiceDeliveryUnitType.Id))
                .ForMember(dest => dest.ServiceDeliveryUnitNotes, opt => opt.MapFrom(src => src.ServiceDeliveryUnitNotes));

            // ServiceComponentEditState.Level1WithChildComponent
            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentLevel1WithChildComponentViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ServiceActivities, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ResolverServiceDeliveryOrganisation, opt => opt.Ignore())
                .ForMember(dest => dest.ResolverServiceDeliveryUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ResolverGroup, opt => opt.Ignore())
                .ForMember(dest => dest.EditUrl, opt => opt.Ignore())
                .ForMember(dest => dest.EditLevel, opt => opt.Ignore())
                .ForMember(dest => dest.ReturnUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ParentName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceComponent, opt => opt.Ignore());

            // ServiceComponentEditState.Level1WithNoChildComponentOrResolver
            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.DiagramOrder, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ServiceActivities, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ResolverServiceDeliveryOrganisation, opt => opt.Ignore())
                .ForMember(dest => dest.ResolverServiceDeliveryUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ResolverGroup, opt => opt.Ignore())
                .ForMember(dest => dest.ChildComponent, opt => opt.Ignore())
                .ForMember(dest => dest.EditLevel, opt => opt.Ignore())
                .ForMember(dest => dest.EditUrl, opt => opt.Ignore())
                .ForMember(dest => dest.InputMode, opt => opt.Ignore())
                .ForMember(dest => dest.ReturnUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ParentName, opt => opt.Ignore());


            Mapper.CreateMap<Model.ResolverGroupTypeRefData, Web.EditResolverResolverGroupViewModel>()
                .ForMember(dest => dest.ResolverGroupTypeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InsertedBy, opt => opt.Ignore())
                .ForMember(dest => dest.InsertedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.ResolverGroupTypeRefData, Web.EditResolverResolverGroupViewModel>()
                .ForMember(dest => dest.ResolverGroupTypeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InsertedBy, opt => opt.Ignore())
                .ForMember(dest => dest.InsertedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.ServiceDeliveryOrganisationTypeRefData, Web.EditResolverServiceDeliveryOrganisationViewModel>()
                .ForMember(dest => dest.ServiceDeliveryOrganisationTypeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(src => src.ServiceDeliveryOrganisationNotes, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.ServiceDeliveryUnitTypeRefData, Web.EditResolverServiceDeliveryUnitViewModel>()
                .ForMember(dest => dest.ServiceDeliveryUnitTypeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(src => src.ServiceDeliveryUnitNotes, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore());

            Mapper.CreateMap<Model.Resolver, Web.EditResolverLevelZeroViewModel>()
                .ForMember(dest => dest.ResolverServiceDeliveryOrganisation, opt => opt.ResolveUsing(model => new Web.EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = model.ServiceDeliveryOrganisationType.Id,
                    ServiceDeliveryOrganisationNotes = model.ServiceDeliveryOrganisationNotes
                }))
                .ForMember(dest => dest.ResolverServiceDeliveryUnit, opt => opt.ResolveUsing(model => new Web.EditResolverServiceDeliveryUnitLevelZeroViewModel
                {
                    ServiceDeliveryUnitTypeId = model.ServiceDeliveryUnitType.Id,
                    ServiceDeliveryUnitNotes = model.ServiceDeliveryUnitNotes
                }))
                .ForMember(dest => dest.ResolverGroup, opt => opt.MapFrom(src => src.ResolverGroupType))
                .ForMember(dest => dest.ServiceDeskName, opt => opt.MapFrom(src => src.ServiceDesk.DeskName))
                .ForMember(dest => dest.EditLevel, opt => opt.Ignore())
                .ForMember(dest => dest.ReturnUrl, opt => opt.Ignore());

            // ServiceComponentEditState.Level1WithResolver
            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentLevel1WithResolverViewModel>()
                .Include<Model.ServiceComponent, Web.EditResolverViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ComponentName, opt => opt.ResolveUsing(model => new Web.EditServiceComponentNameViewModel
                {
                    ComponentName = model.ComponentName
                }))
                .ForMember(dest => dest.DiagramOrder, opt => opt.ResolveUsing(model => new Web.EditServiceComponentDiagramOrderViewModel
                {
                    DiagramOrder = model.DiagramOrder
                }))
                .ForMember(dest => dest.ServiceActivities, opt => opt.ResolveUsing(model => new Web.ServiceActivityViewModel
                {
                    ServiceActivities = model.ServiceActivities
                }))
                .ForMember(dest => dest.ResolverServiceDeliveryOrganisation, opt => opt.ResolveUsing(model => new Web.EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = model.Resolver.ServiceDeliveryOrganisationType.Id,
                    ServiceDeliveryOrganisationNotes = model.Resolver.ServiceDeliveryOrganisationNotes,
                }))
                .ForMember(dest => dest.ResolverServiceDeliveryUnit, opt => opt.ResolveUsing(model => new Web.EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = model.Resolver.ServiceDeliveryUnitType.Id,
                    ServiceDeliveryUnitNotes = model.Resolver.ServiceDeliveryUnitNotes,
                }))
                .ForMember(dest => dest.ResolverGroup, opt => opt.MapFrom(src => src.Resolver.ResolverGroupType))
                .ForMember(dest => dest.EditUrl, opt => opt.Ignore())
                .ForMember(dest => dest.EditLevel, opt => opt.Ignore())
                .ForMember(dest => dest.ReturnUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ParentName, opt => opt.Ignore());

            // ServiceComponentEditState.Level2
            Mapper.CreateMap<Model.ServiceComponent, Web.EditServiceComponentLevel2ViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ComponentNameLevel, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ServiceActivities, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ResolverServiceDeliveryOrganisation, opt => opt.ResolveUsing(model => new Web.EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = model.Resolver.ServiceDeliveryOrganisationType.Id,
                    ServiceDeliveryOrganisationNotes = model.Resolver.ServiceDeliveryOrganisationNotes,
                }))
                .ForMember(dest => dest.ResolverServiceDeliveryUnit, opt => opt.ResolveUsing(model => new Web.EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = model.Resolver.ServiceDeliveryUnitType.Id,
                    ServiceDeliveryUnitNotes = model.Resolver.ServiceDeliveryUnitNotes,
                }))
                .ForMember(dest => dest.ResolverGroup, opt => opt.MapFrom(src => src.Resolver.ResolverGroupType))
                .ForMember(dest => dest.EditUrl, opt => opt.Ignore())
                .ForMember(dest => dest.EditLevel, opt => opt.Ignore())
                .ForMember(dest => dest.CanEdit, opt => opt.MapFrom(src => src.Resolver.Id > 0))
                .ForMember(dest => dest.ReturnUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ParentName, opt => opt.Ignore());

            // Edit Resolver
            Mapper.CreateMap<Model.ServiceComponent, Web.EditResolverViewModel>()
                .ForMember(dest => dest.OperationalProcesses, opt => opt.MapFrom(src => src.Resolver));

            Mapper.CreateMap<Service.ResolverListItem, Web.ResolverViewModel>();
            Mapper.CreateMap<Service.ResolverListItem, Web.ResolverLevelZeroViewModel>();

            // Setup like-for-like for Service Component move.
            Mapper.CreateMap<Model.OperationalProcessType, Model.OperationalProcessType>()
                .ForMember(dest => dest.Resolver, opt => opt.Ignore());
            Mapper.CreateMap<Model.Resolver, Model.Resolver>();

            Mapper.CreateMap<Model.ServiceDeliveryOrganisationTypeRefData, Web.ServiceDeliveryOrganisationTypeRefDataViewModel>();
            Mapper.CreateMap<Web.ServiceDeliveryOrganisationTypeRefDataViewModel, Model.ServiceDeliveryOrganisationTypeRefData>();

            Mapper.CreateMap<Service.OperationalProcessTypeRefDataListItem, Web.OperationalProcessTypeRefDataViewModel>();
            Mapper.CreateMap<Web.OperationalProcessTypeRefDataViewModel, Model.OperationalProcessTypeRefData>();

            Mapper.CreateMap<Service.ServiceDeliveryUnitTypeRefDataListItem, Web.ServiceDeliveryUnitTypeRefDataViewModel>();
            Mapper.CreateMap<Web.ServiceDeliveryUnitTypeRefDataViewModel, Model.ServiceDeliveryUnitTypeRefData>();

            Mapper.CreateMap<Service.ResolverGroupTypeRefDataListItem, Web.ResolverGroupTypeRefDataViewModel>();

            Mapper.CreateMap<Web.ResolverGroupTypeRefDataViewModel, Model.ResolverGroupTypeRefData>()
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.Order));

            // DotMatrix Map to List<Dictionary<string, object>>
            Mapper.CreateMap<List<Service.DotMatrixListItem>, Dictionary<string, object>>()
                .ConvertUsing(cu =>
                {
                    if (cu == null)
                    {
                        return null;
                    }
                    var result = new Dictionary<string, object>();
                    cu.ForEach(f => result.Add(f.Name, f.Value));
                    return result;
                });

            Mapper.CreateMap<Web.ContextHelpRefDataViewModel, Model.ContextHelpRefData>()
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.AssetId, opt => opt.Ignore());

            Mapper.CreateMap<Diagram.ChartDataListItem, Web.ChartDataViewModel>();

            // Templates
            Mapper.CreateMap<Model.Template, Service.TemplateListItem>()
                .ForMember(dest => dest.TemplateType, opt => opt.MapFrom(o => o.TemplateType.ToEnumText<TemplateType>()));
            Mapper.CreateMap<Service.TemplateListItem, Web.TemplateViewModel>();

            Mapper.CreateMap<Model.TemplateDomain, Service.TemplateDomainListItem>();
            Mapper.CreateMap<Service.TemplateDomainListItem, Web.TemplateDomainViewModel>()
                .ForMember(dest => dest.ServiceDeskId, opt => opt.Ignore())
                .ForMember(dest => dest.Selected, opt => opt.Ignore());

            Mapper.CreateMap<Web.TemplateDomainViewModel, Service.TemplateDomainListItem>();

            Mapper.CreateMap<Model.TemplateRow, Service.TemplateRowListItem>();
            Mapper.CreateMap<Service.TemplateRowListItem, Web.TemplateRowViewModel>();


            Mapper.CreateMap<Model.Asset, Web.AssetViewModel>();

            Mapper.CreateMap<Service.RegionTypeRefDataListItem, Web.RegionTypeRefDataViewModel>();

            Mapper.CreateMap<Model.RegionTypeRefData, Service.RegionTypeRefDataListItem>()
                .ForMember(dest => dest.RegionTypeName, opt => opt.MapFrom(src => src.RegionName))
                .ForMember(dest => dest.UsageCount, opt => opt.Ignore());

            Mapper.CreateMap<Web.RegionTypeRefDataViewModel, Model.RegionTypeRefData>()
                .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.RegionTypeName));

            Mapper.CreateMap<Model.Audit, Web.AuditViewModel>();
            Mapper.CreateMap<Web.AuditViewModel, Model.Audit>()
                .ForMember(dest => dest.Customer, opt => opt.Ignore());
        }
    }
}