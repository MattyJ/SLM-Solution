using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Transformers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Transformers
{
    public class TransformTemplateToDesign : ITransformTemplateToDesign
    {
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IServiceDomainService _serviceDomainService;
        private readonly IServiceFunctionService _serviceFunctionService;
        private readonly IServiceComponentService _serviceComponentService;
        private readonly IDomainTypeRefDataService _domainTypeRefDataService;
        private readonly IFunctionTypeRefDataService _functionTypeRefDataService;
        private readonly IServiceDeliveryOrganisationTypeRefDataService _serviceDeliveryOrganisationTypeRefDataService;
        private readonly IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitTypeRefDataService;
        private readonly IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private readonly IUserIdentity _userIdentity;

        public TransformTemplateToDesign(IServiceDeskService serviceDeskService,
            IServiceDomainService serviceDomainService,
            IServiceFunctionService serviceFunctionService,
            IServiceComponentService serviceComponentService,
            IDomainTypeRefDataService domainTypeRefDataService,
            IFunctionTypeRefDataService functionTypeRefDataService,
            IServiceDeliveryOrganisationTypeRefDataService serviceDeliveryOrganisationTypeRefDataService,
            IServiceDeliveryUnitTypeRefDataService serviceDeliveryUnitTypeRefDataService,
            IResolverGroupTypeRefDataService resolverGroupTypeRefDataService,
            IUserIdentity userIdentity)
        {
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }
            if (serviceDomainService == null)
            {
                throw new ArgumentNullException(nameof(serviceDomainService));
            }
            if (serviceFunctionService == null)
            {
                throw new ArgumentNullException(nameof(serviceFunctionService));
            }
            if (serviceComponentService == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentService));
            }
            if (domainTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(domainTypeRefDataService));
            }
            if (functionTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(functionTypeRefDataService));
            }
            if (serviceDeliveryOrganisationTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeliveryOrganisationTypeRefDataService));
            }
            if (serviceDeliveryUnitTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeliveryUnitTypeRefDataService));
            }
            if (resolverGroupTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(resolverGroupTypeRefDataService));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            _serviceDeskService = serviceDeskService;
            _serviceDomainService = serviceDomainService;
            _serviceFunctionService = serviceFunctionService;
            _serviceComponentService = serviceComponentService;
            _domainTypeRefDataService = domainTypeRefDataService;
            _functionTypeRefDataService = functionTypeRefDataService;
            _serviceDeliveryOrganisationTypeRefDataService = serviceDeliveryOrganisationTypeRefDataService;
            _serviceDeliveryUnitTypeRefDataService = serviceDeliveryUnitTypeRefDataService;
            _resolverGroupTypeRefDataService = resolverGroupTypeRefDataService;
            _userIdentity = userIdentity;
        }

        public void Transform(int serviceDeskId, List<TemplateRow> templateRows)
        {
            var serviceDesk = _serviceDeskService.GetById(serviceDeskId);

            foreach (var templateRow in templateRows)
            {
                TransformServiceDomain(serviceDesk, templateRow);
            }
        }
        private void TransformServiceDomain(ServiceDesk serviceDesk, TemplateRow templateRow)
        {
            var serviceDomain = serviceDesk.ServiceDomains.FirstOrDefault(d => d.DomainType.DomainName.Trim() == templateRow.ServiceDomain.Trim());

            if (serviceDomain == null)
            {
                var dateTimeNow = DateTime.Now;
                serviceDomain = new ServiceDomain
                {
                    DomainType = _domainTypeRefDataService.InsertorUpdate(templateRow.ServiceDomain),
                    ServiceFunctions = new List<ServiceFunction>(),
                    DiagramOrder = 5,
                    InsertedBy = _userIdentity.Name,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow
                };

                serviceDesk.ServiceDomains.Add(serviceDomain);
                _serviceDeskService.Update(serviceDesk);
            }

            if (!string.IsNullOrEmpty(templateRow.ServiceFunction))
            {
                TransformServiceFunction(serviceDomain, templateRow);
            }
        }

        private void TransformServiceFunction(ServiceDomain serviceDomain, TemplateRow templateRow)
        {
            var serviceFunction = serviceDomain.ServiceFunctions.FirstOrDefault(d => d.FunctionType.FunctionName.Trim() == templateRow.ServiceFunction.Trim());

            if (serviceFunction == null)
            {
                var dateTimeNow = DateTime.Now;
                serviceFunction = new ServiceFunction
                {
                    ServiceDomain = serviceDomain,
                    FunctionType = _functionTypeRefDataService.InsertorUpdate(templateRow.ServiceFunction),
                    ServiceComponents = new List<ServiceComponent>(),
                    DiagramOrder = 5,
                    InsertedBy = _userIdentity.Name,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow
                };

                serviceDomain.ServiceFunctions.Add(serviceFunction);
                _serviceDomainService.Update(serviceDomain);
            }

            if (!string.IsNullOrEmpty(templateRow.ServiceComponentLevel1))
            {
                TransformComponentLevelOne(serviceFunction, templateRow);
            }

        }

        private void TransformComponentLevelOne(ServiceFunction serviceFunction, TemplateRow templateRow)
        {
            var serviceComponent = serviceFunction.ServiceComponents.FirstOrDefault(d => d.ComponentName == templateRow.ServiceComponentLevel1 && d.ComponentLevel == 1);

            if (serviceComponent == null)
            {
                var dateTimeNow = DateTime.Now;
                serviceComponent = new ServiceComponent
                {
                    ServiceFunction = serviceFunction,
                    ComponentName = templateRow.ServiceComponentLevel1,
                    ComponentLevel = 1,
                    ServiceActivities = string.IsNullOrEmpty(templateRow.ServiceComponentLevel2) &&
                                        !string.IsNullOrEmpty(templateRow.ServiceActivities)
                        ? templateRow.ServiceActivities
                        : string.Empty,
                    DiagramOrder = 5,
                    InsertedBy = _userIdentity.Name,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow
                };

                serviceFunction.ServiceComponents.Add(serviceComponent);
                _serviceFunctionService.Update(serviceFunction);
            }
            //else if (!string.IsNullOrEmpty(templateRow.ServiceActivities))
            //{
            //    if (!string.IsNullOrEmpty(serviceComponent.ServiceActivities))
            //        serviceComponent.ServiceActivities += $"\r\n{templateRow.ServiceActivities}";
            //    else
            //    {
            //        serviceComponent.ServiceActivities = templateRow.ServiceActivities;
            //    }

            //    _serviceComponentService.Update(serviceComponent);
            //}

            if (!string.IsNullOrEmpty(templateRow.ServiceComponentLevel2))
            {
                if (serviceComponent.Resolver != null)
                {
                    throw new DataImportException(
                        $"Error reading Service Decomposition Design spreadsheet. Worksheet, Resolvers and Childs Components detected on Component [{templateRow.ServiceComponentLevel1}].");
                }

                TransformComponentLevelTwo(serviceComponent, templateRow);
            }
            else if (!string.IsNullOrEmpty(templateRow.ServiceDeliveryOrganisation))
            {
                if (serviceComponent.Resolver != null)
                {
                    throw new DataImportException(
                        $"Error reading Service Decomposition Design spreadsheet. Worksheet, Multiple Resolvers per Component detected on Component [{templateRow.ServiceComponentLevel1}].");
                }

                TransformResolver(serviceComponent, templateRow);
            }
        }

        private void TransformComponentLevelTwo(ServiceComponent parentComponent, TemplateRow templateRow)
        {
            var serviceComponent = parentComponent.ChildServiceComponents?.FirstOrDefault(d => d.ComponentName == templateRow.ServiceComponentLevel2 && d.ComponentLevel == 2);

            if (serviceComponent == null)
            {
                if (parentComponent.ChildServiceComponents == null)
                {
                    parentComponent.ChildServiceComponents = new List<ServiceComponent>();
                }

                var dateTimeNow = DateTime.Now;
                serviceComponent = new ServiceComponent
                {
                    ParentServiceComponent = parentComponent,
                    ServiceFunction = parentComponent.ServiceFunction,
                    ComponentName = templateRow.ServiceComponentLevel2,
                    ComponentLevel = 2,
                    ServiceActivities = !string.IsNullOrEmpty(templateRow.ServiceActivities) ? templateRow.ServiceActivities : string.Empty,
                    DiagramOrder = 5,
                    InsertedBy = _userIdentity.Name,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow
                };

                parentComponent.ChildServiceComponents.Add(serviceComponent);
                _serviceComponentService.Update(parentComponent);
            }
            //else if (!string.IsNullOrEmpty(templateRow.ServiceActivities))
            //{
            //    if (!string.IsNullOrEmpty(serviceComponent.ServiceActivities))
            //        serviceComponent.ServiceActivities += $"\r\n{templateRow.ServiceActivities}";
            //    else
            //    {
            //        serviceComponent.ServiceActivities = templateRow.ServiceActivities;
            //    }

            //    _serviceComponentService.Update(serviceComponent);
            //}

            if (!string.IsNullOrEmpty(templateRow.ServiceDeliveryOrganisation))
            {
                if (serviceComponent.Resolver != null)
                {
                    throw new DataImportException(
                        $"Error reading Service Decomposition Design spreadsheet. Worksheet, Multiple Resolvers per Component detected on Component [{templateRow.ServiceComponentLevel2}].");
                }

                TransformResolver(serviceComponent, templateRow);
            }
        }

        private void TransformResolver(ServiceComponent serviceComponent, TemplateRow templateRow)
        {

            if (!ServiceDeliveryOrganisationNames.Descriptions.Contains(templateRow.ServiceDeliveryOrganisation))
            {
                throw new DataImportException($"Error reading Service Decomposition Design spreadsheet. Invalid Responsible Organisation Value - {templateRow.ServiceDeliveryOrganisation}.");
            }

            var dateTimeNow = DateTime.Now;
            var resolver = new Resolver
            {
                ServiceDesk = serviceComponent.ServiceFunction.ServiceDomain.ServiceDesk,
                ServiceComponent = serviceComponent,
                ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypeRefDataService.All().Single(x => x.ServiceDeliveryOrganisationTypeName == templateRow.ServiceDeliveryOrganisation),
                ServiceDeliveryUnitType = !string.IsNullOrEmpty(templateRow.ServiceDeliveryUnit) ? _serviceDeliveryUnitTypeRefDataService.InsertorUpdate(templateRow.ServiceDeliveryUnit) : null,
                ResolverGroupType = !string.IsNullOrEmpty(templateRow.ResolverGroup) ? _resolverGroupTypeRefDataService.InsertorUpdate(templateRow.ResolverGroup) : null,
                InsertedBy = _userIdentity.Name,
                InsertedDate = dateTimeNow,
                UpdatedBy = _userIdentity.Name,
                UpdatedDate = dateTimeNow
            };

            serviceComponent.Resolver = resolver;
            _serviceComponentService.Update(serviceComponent);
        }

    }
}
