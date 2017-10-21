using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    public abstract class BaseServiceComponentController : BaseController
    {
        private readonly IContextManager _contextManager;
        private readonly IServiceComponentService _serviceComponentService;
        private readonly IServiceFunctionService _serviceFunctionService;
        private readonly IServiceDeliveryOrganisationTypeRefDataService _serviceDeliveryOrganisationTypeRefDataService;
        private readonly IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitTypeRefDataService;
        private readonly IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private readonly IParameterService _parameterService;
        private readonly IAppUserContext _appUserContext;
        private readonly IServiceComponentHelper _serviceComponentHelper;

        protected BaseServiceComponentController(IContextManager contextManager,
            IServiceComponentService serviceComponentService,
            IServiceFunctionService serviceFunctionService,
            IServiceDeliveryOrganisationTypeRefDataService serviceDeliveryOrganisationTypeRefDataService,
            IServiceDeliveryUnitTypeRefDataService serviceDeliveryUnitTypeRefDataService,
            IResolverGroupTypeRefDataService resolverGroupTypeRefDataService,
            IParameterService parameterService,
            IAppUserContext appUserContext,
            IServiceComponentHelper serviceComponentHelper)
            : base(contextManager)
        {
            _contextManager = contextManager;
            _serviceComponentService = serviceComponentService;
            _serviceFunctionService = serviceFunctionService;
            _serviceDeliveryOrganisationTypeRefDataService = serviceDeliveryOrganisationTypeRefDataService;
            _serviceDeliveryUnitTypeRefDataService = serviceDeliveryUnitTypeRefDataService;
            _resolverGroupTypeRefDataService = resolverGroupTypeRefDataService;
            _parameterService = parameterService;
            _appUserContext = appUserContext;
            _serviceComponentHelper = serviceComponentHelper;
        }

        #region Helpers

        protected void SaveServiceComponentResolver(ServiceComponent component, EditServiceComponentWithResolverViewModel model)
        {
            if (!model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId.HasValue ||
                !(model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId > 0)) return;

            component.Resolver.ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypeRefDataService.GetById(model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId.Value);

            component.Resolver.ServiceDeliveryOrganisationNotes = model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationNotes;

            if (model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId.HasValue &&
                model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId > 0)
            {
                component.Resolver.ServiceDeliveryUnitType =
                    _serviceDeliveryUnitTypeRefDataService.GetById(
                        model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId.Value);
            }
            else
            {
                component.Resolver.ServiceDeliveryUnitType = null;
            }

            component.Resolver.ServiceDeliveryUnitNotes =
                model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitNotes;

            if (model.ResolverGroup.ResolverGroupTypeId.HasValue && model.ResolverGroup.ResolverGroupTypeId > 0)
            {
                var resolverGroupType = _resolverGroupTypeRefDataService.GetById(model.ResolverGroup.ResolverGroupTypeId.Value);
                component.Resolver.ResolverGroupType = resolverGroupType;

                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                if (!resolverGroupType.Visible &&
                    _resolverGroupTypeRefDataService.GetNumberOfResolverGroupTypeReferences(resolverGroupType.Id) >= customerSpecificTypeThreshold - 1)
                {
                    resolverGroupType.Visible = true;
                    _resolverGroupTypeRefDataService.Update(resolverGroupType);
                }
            }
            else
            {
                component.Resolver.ResolverGroupType = null;
            }
        }

        protected SaveServiceComponentPreChecksResult SaveServiceComponentPreChecks(EditServiceComponentViewModel model,
            ServiceComponentEditState requiredState)
        {
            return SaveServiceComponentPreChecks(model, new[] { requiredState });
        }

        protected SaveServiceComponentPreChecksResult SaveServiceComponentPreChecks(EditServiceComponentViewModel model,
            IEnumerable<ServiceComponentEditState> requiredStates)
        {
            var result = new SaveServiceComponentPreChecksResult
            {
                IsValid = false
            };

            if (!ModelState.IsValid)
            {
                result.Result = View("Edit" + model.EditLevel, model);
                return result;
            }

            // Get the existing Service Component.
            result.ServiceComponent = _serviceComponentService
                .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .SingleOrDefault(x => x.Id == model.Id);

            if (result.ServiceComponent == null)
            {
                ModelState.AddModelError(ModelStateErrorNames.ServiceComponentEditCannotBeFound, WebResources.ServiceComponentEditCannotBeFound);
                result.Result = View("Edit" + model.EditLevel, model);
                return result;
            }

            var state = _serviceComponentHelper.GetEditState(result.ServiceComponent);
            if (!requiredStates.Contains(state))
            {
                ModelState.AddModelError(ModelStateErrorNames.ServiceComponentIncorrectState, WebResources.ServiceComponentIncorrectState);
                result.Result = View("Edit" + model.EditLevel, model);
                return result;
            }

            var now = DateTime.Now;
            var userName = _contextManager.UserManager.Name;
            result.ServiceComponent.UpdatedBy = userName;
            result.ServiceComponent.UpdatedDate = now;

            result.IsValid = true;
            return result;
        }

        protected SaveServiceComponentPreChecksResult SaveServiceComponentPreChecks(EditResolverViewModel model,
            IEnumerable<ServiceComponentEditState> requiredStates)
        {
            var result = new SaveServiceComponentPreChecksResult
            {
                IsValid = false
            };

            if (!ModelState.IsValid)
            {
                result.Result = View("Edit" + model.EditLevel, model);
                return result;
            }

            // Get the existing Service Component.
            result.ServiceComponent = _serviceComponentService
                .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .SingleOrDefault(x => x.Resolver != null && x.Resolver.Id == model.Id);

            if (result.ServiceComponent == null)
            {
                ModelState.AddModelError(ModelStateErrorNames.ServiceComponentEditCannotBeFound, WebResources.ServiceComponentEditCannotBeFound);
                result.Result = View("Edit" + model.EditLevel, model);
                return result;
            }

            var state = _serviceComponentHelper.GetEditState(result.ServiceComponent);
            if (!requiredStates.Contains(state))
            {
                ModelState.AddModelError(ModelStateErrorNames.ServiceComponentIncorrectState, WebResources.ServiceComponentIncorrectState);
                result.Result = View("Edit" + model.EditLevel, model);
                return result;
            }

            var now = DateTime.Now;
            var userName = _contextManager.UserManager.Name;
            result.ServiceComponent.UpdatedBy = userName;
            result.ServiceComponent.UpdatedDate = now;

            result.IsValid = true;
            return result;
        }

        protected List<SelectListItem> GetServiceFunctionHierarchy()
        {
            return _serviceFunctionService
                    .CustomerServiceFunctions(_appUserContext.Current.CurrentCustomer.Id)
                    .Select(x => new SelectListItem
                    {
                        Text = string.Concat(x.ServiceDeskName,
                            UnicodeCharacters.DoubleArrowRight,
                            x.ServiceDomainName,
                            UnicodeCharacters.DoubleArrowRight,
                            x.AlternativeName ?? x.FunctionName),
                        Value = x.Id.ToString()
                    })
                    .ToList();
        }

        protected List<SelectListItem> GetServiceComponentHierarchy(int? serviceFunctionIdFilter)
        {

            return _serviceComponentService
                .GetByCustomerWithHierarchy(_appUserContext.Current.CurrentCustomer.Id)
                .Where(x => x.ServiceComponent.ComponentLevel == (int)ServiceComponentLevel.Level1 &&
                    x.ServiceComponent.Resolver == null &&
                    (!serviceFunctionIdFilter.HasValue || x.ServiceFunctionId == serviceFunctionIdFilter.Value))
                .Select(x => new SelectListItem
                {
                    Text = string.Concat(x.ServiceDeskName,
                        UnicodeCharacters.DoubleArrowRight,
                        x.ServiceDomainName,
                        UnicodeCharacters.DoubleArrowRight,
                        x.ServiceFunctionName,
                        UnicodeCharacters.DoubleArrowRight,
                        x.ServiceComponent.ComponentName),
                    Value = x.ServiceComponent.Id.ToString()
                })
                .ToList();

        }

        #endregion

    }
}
