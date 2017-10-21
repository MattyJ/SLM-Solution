using AutoMapper;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EnumExtensions = Fujitsu.SLM.Enumerations.EnumExtensions;

namespace Fujitsu.SLM.Web.Controllers
{
    public class ServiceComponentController : BaseServiceComponentController
    {
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;
        private readonly IServiceComponentService _serviceComponentService;
        private readonly IServiceFunctionService _serviceFunctionService;
        private readonly IServiceDeliveryOrganisationTypeRefDataService _serviceDeliveryOrganisationTypeRefDataService;
        private readonly IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitTypeRefDataService;
        private readonly IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private readonly IServiceComponentHelper _serviceComponentHelper;
        private readonly IParameterService _parameterService;


        public ServiceComponentController(IContextManager contextManager,
            IAppUserContext appUserContext,
            IServiceComponentService serviceComponentService,
            IServiceFunctionService serviceFunctionService,
            IServiceDeliveryOrganisationTypeRefDataService serviceDeliveryOrganisationTypeRefDataService,
            IServiceDeliveryUnitTypeRefDataService serviceDeliveryUnitTypeRefDataService,
            IResolverGroupTypeRefDataService resolverGroupTypeRefDataService,
            IParameterService parameterService,
            IServiceComponentHelper serviceComponentHelper) : base(contextManager,
                serviceComponentService,
                serviceFunctionService,
                serviceDeliveryOrganisationTypeRefDataService,
                serviceDeliveryUnitTypeRefDataService,
                resolverGroupTypeRefDataService,
                parameterService,
                appUserContext,
                serviceComponentHelper)
        {
            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            if (serviceComponentService == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentService));
            }

            if (serviceFunctionService == null)
            {
                throw new ArgumentNullException(nameof(serviceFunctionService));
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

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            if (serviceComponentHelper == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentHelper));
            }

            _contextManager = contextManager;
            _appUserContext = appUserContext;
            _serviceComponentService = serviceComponentService;
            _serviceFunctionService = serviceFunctionService;
            _serviceDeliveryOrganisationTypeRefDataService = serviceDeliveryOrganisationTypeRefDataService;
            _serviceDeliveryUnitTypeRefDataService = serviceDeliveryUnitTypeRefDataService;
            _resolverGroupTypeRefDataService = resolverGroupTypeRefDataService;
            _parameterService = parameterService;
            _serviceComponentHelper = serviceComponentHelper;
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            var serviceFunctionCount = _serviceFunctionService
                .CustomerServiceFunctions(_appUserContext.Current.CurrentCustomer.Id)
                .Select(s => s.Id)
                .Count();

            var model = new ViewServiceComponentViewModel()
            {
                EditLevel = level,
                CanMoveServiceComponent = serviceFunctionCount > 1
            };

            return View(level, model);
        }


        [Authorize(Roles = UserRoles.Viewer)]
        [HttpPost]
        public ActionResult ReadAjaxServiceComponentsGrid([DataSourceRequest] DataSourceRequest request, int? serviceFunctionId)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var all = new List<ServiceComponentViewModel>();

                    // Get all the level 1 results. These will have the level 2's inside so we don't have to do another query.
                    var parentComponentListItems = _serviceComponentService
                        .GetByCustomerWithHierarchy(_appUserContext.Current.CurrentCustomer.Id)
                        .Where(x => x.ServiceComponent.ComponentLevel == (int)ServiceComponentLevel.Level1 &&
                            (!serviceFunctionId.HasValue || x.ServiceFunctionId == serviceFunctionId.Value))
                        .OrderBy(o => o.ServiceComponent.DiagramOrder).ThenBy(o => o.ServiceComponent.ComponentName)
                        .ToList();

                    // Add all level 1's to the list, but insert the level 2's so they will appear underneath.
                    foreach (var parentServiceComponent in parentComponentListItems)
                    {
                        all.Add(Mapper.Map<ServiceComponentViewModel>(parentServiceComponent));

                        if (parentServiceComponent.ServiceComponent.ChildServiceComponents == null ||
                            !parentServiceComponent.ServiceComponent.ChildServiceComponents.Any()) continue;

                        // Now overwrite with the specific level 2 info.
                        foreach (var childComponent in parentServiceComponent.ServiceComponent.ChildServiceComponents.OrderBy(o => o.DiagramOrder).ThenBy(o => o.ComponentName))
                        {
                            // We repeat the above for the level 2 as the main parent info will be the same as the level 1.
                            var child = Mapper.Map<ServiceComponentViewModel>(parentServiceComponent);
                            var childServiceComponent = Mapper.Map(childComponent, child);
                            all.Add(childServiceComponent);
                        }
                    }

                    // Return to the grid.
                    result = all.ToDataSourceResult(request);
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [Authorize(Roles = UserRoles.Viewer)]
        [HttpPost]
        public ActionResult ReadAjaxServiceComponentsLevelTwoGrid([DataSourceRequest] DataSourceRequest request, int? parentComponentId)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var all = new List<ServiceComponentViewModel>();

                    // Obtain the Parent Component
                    var parentComponent = _serviceComponentService
                        .GetByCustomerWithHierarchy(_appUserContext.Current.CurrentCustomer.Id)
                        .Where(x => x.ServiceComponent.Id == parentComponentId &&
                                x.ServiceComponent.ComponentLevel == (int)ServiceComponentLevel.Level1)
                        .OrderBy(o => o.ServiceComponent.DiagramOrder).ThenBy(o => o.ServiceComponent.ComponentName)
                        .First();

                    if (parentComponent.ServiceComponent?.ChildServiceComponents != null && parentComponent.ServiceComponent.ChildServiceComponents.Any())
                    {
                        // Now overwrite with the specific level 2 info.
                        foreach (var childComponent in parentComponent.ServiceComponent.ChildServiceComponents.OrderBy(o => o.DiagramOrder).ThenBy(o => o.ComponentName))
                        {
                            // We repeat the above for the level 2 as the main parent info will be the same as the level 1.
                            var child = Mapper.Map<ServiceComponentViewModel>(parentComponent);
                            var childServiceComponent = Mapper.Map(childComponent, child);
                            all.Add(childServiceComponent);
                        }

                    }

                    // Return to the grid.
                    result = all.ToDataSourceResult(request);
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult UpdateAjaxServiceComponentsGrid([DataSourceRequest]DataSourceRequest request, ServiceComponentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteAjaxServiceComponentsGrid([DataSourceRequest] DataSourceRequest request,
            ServiceComponentViewModel model)
        {
            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var serviceComponent = _serviceComponentService
                        .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                        .SingleOrDefault(x => x.Id == model.Id);

                    if (serviceComponent != null)
                    {
                        if (_serviceComponentHelper.CanDelete(serviceComponent))
                        {
                            _serviceComponentService.Delete(serviceComponent);
                        }
                        else
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ServiceComponentCannotBeDeletedDueToDependents,
                                WebResources.ServiceComponentCannotBeDeletedDueToDependents);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ServiceComponentCannotBeFound,
                                WebResources.ServiceComponentCannotBeFound);
                    }
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult MoveLevel1()
        {
            var model = new MoveServiceComponentLevel1ViewModel();

            // Get all functions for customer the move.
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                model.ServiceFunctions = GetServiceFunctionHierarchy();
            }
            return PartialView("_MoveL1", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult MoveLevel1(MoveServiceComponentLevel1ViewModel model)
        {
            var result = GetJsonSuccessResponse();
            if (ModelState.IsValid)
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    try
                    {
                        var component = _serviceComponentService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .SingleOrDefault(x => x.Id == model.ServiceComponentId);
                        if (component == null)
                        {
                            result = GetJsonErrorResponse(WebResources.ServiceComponentCannotBeFound);
                        }
                        else
                        {
                            component.ServiceFunctionId = model.DestinationServiceFunctionId;
                            component.UpdatedBy = _contextManager.UserManager.Name;
                            component.UpdatedDate = DateTime.Now;
                            if (component.ChildServiceComponents != null && component.ChildServiceComponents.Any())
                            {
                                // Update all of the underlying child service components
                                foreach (var childComponent in component.ChildServiceComponents)
                                {
                                    childComponent.ParentServiceComponentId = component.ParentServiceComponentId;
                                    childComponent.ServiceFunctionId = model.DestinationServiceFunctionId;
                                    childComponent.UpdatedBy = _contextManager.UserManager.Name;
                                    childComponent.UpdatedDate = DateTime.Now;
                                }
                            }
                            _serviceComponentService.Update(component);
                        }
                    }
                    catch (Exception ex)
                    {
                        result = GetJsonErrorResponse(ex.Message);
                    }
                }
            }
            else
            {
                result = GetJsonErrorResponse(ModelState.GetModelStateMesssages().First());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult MoveLevel2()
        {
            var model = new MoveServiceComponentLevel2ViewModel();

            // Get all functions for customer the move.
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                model.ServiceComponents = GetServiceComponentHierarchy(null);
            }
            return PartialView("_MoveL2", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult MoveLevel2(MoveServiceComponentLevel2ViewModel model)
        {
            var result = GetJsonSuccessResponse();
            if (ModelState.IsValid)
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    try
                    {
                        var component = _serviceComponentService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .SingleOrDefault(x => x.Id == model.ServiceComponentId);
                        var destinationComponent = _serviceComponentService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .SingleOrDefault(x => x.Id == model.DestinationServiceComponentId);

                        if (component == null || destinationComponent == null)
                        {
                            result = GetJsonErrorResponse(WebResources.ServiceComponentCannotBeFound);
                        }
                        else
                        {
                            component.UpdatedBy = _contextManager.UserManager.Name;
                            component.UpdatedDate = DateTime.Now;
                            component.ServiceFunctionId = destinationComponent.ServiceFunctionId;
                            component.ParentServiceComponentId = destinationComponent.Id;

                            if (destinationComponent.ChildServiceComponents == null)
                            {
                                destinationComponent.ChildServiceComponents = new List<ServiceComponent>();
                            }

                            destinationComponent.ChildServiceComponents.Add(component);
                            destinationComponent.UpdatedBy = _contextManager.UserManager.Name;
                            destinationComponent.UpdatedDate = DateTime.Now;

                            _serviceComponentService.Update(destinationComponent);
                        }
                    }
                    catch (Exception ex)
                    {
                        result = GetJsonErrorResponse(ex.Message);
                    }
                }
            }
            else
            {
                result = GetJsonErrorResponse(ModelState.GetModelStateMesssages().First());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult AddLevel1(string level, int? id)
        {
            var vm = new AddServiceComponentLevel1ViewModel
            {
                EditLevel = level,
                HasServiceFunctionContext = id.HasValue && id.Value != 0,
                ServiceFunctionId = id
            };

            vm.ReturnUrl = vm.HasServiceFunctionContext
                ? Url.Action("Edit", "ServiceFunction", new { level = vm.EditLevel, id = vm.ServiceFunctionId })
                : Url.Action("Index", "ServiceComponent", new { level = vm.EditLevel });

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var customerId = _appUserContext.Current.CurrentCustomer.Id;
                var serviceFunctions = _serviceFunctionService.CustomerServiceFunctions(customerId).ToList();

                if (id.HasValue && id.Value > 0)
                {
                    vm.ServiceFunctionName = serviceFunctions.First(x => x.Id == id.Value).FunctionName;
                }
                else if (serviceFunctions.Count == 1)
                {
                    vm.ServiceFunctionId = serviceFunctions.First().Id;
                    vm.ServiceFunctionName = serviceFunctions.First().FunctionName;
                    vm.HasServiceFunctionContext = true;
                }
                else
                {
                    vm.ServiceFunctions.AddRange(GetServiceFunctionHierarchy());
                }
            }

            return View("Add" + level + "L1", vm);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult AddLevel2(string level, int id)
        {
            var vm = new AddServiceComponentLevel2ViewModel
            {
                EditLevel = level,
                ServiceComponentId = id,
                ServiceComponentName = _serviceComponentService.GetById(id).ComponentName
            };

            vm.ReturnUrl = Url.Action("Edit", "ServiceComponent", new { level = vm.EditLevel, id = vm.ServiceComponentId });

            return View("Add" + level + "L2", vm);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxServiceAddComponentLevel1Grid([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<BulkServiceComponentViewModel> serviceComponents)
        {
            try
            {
                if (_appUserContext.Current.CurrentCustomer != null &&
                    _appUserContext.Current.CurrentCustomer.Id > 0 &&
                    serviceComponents != null)
                {
                    var now = DateTime.Now;
                    var userName = _contextManager.UserManager.Name;

                    var newComponents = new List<ServiceComponent>(serviceComponents
                        .Where(x => x.ServiceFunctionId.HasValue &&
                            x.ServiceFunctionId.Value != 0 &&
                            !string.IsNullOrEmpty(x.ComponentName))
                        .Select(x => new ServiceComponent
                        {
                            ComponentLevel = (int)ServiceComponentLevel.Level1,
                            ComponentName = x.ComponentName,
                            DiagramOrder = x.DiagramOrder == null || x.DiagramOrder < 1 ? 5 : x.DiagramOrder.Value,
                            ServiceFunctionId = x.ServiceFunctionId.Value,
                            InsertedBy = userName,
                            InsertedDate = now,
                            UpdatedBy = userName,
                            UpdatedDate = now
                        }));

                    if (newComponents.Any())
                    {
                        _serviceComponentService.Create(newComponents);
                    }
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            if (serviceComponents == null)
            {
                serviceComponents = new List<BulkServiceComponentViewModel>();
            }

            return Json(serviceComponents.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxServiceAddComponentLevel2Grid([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<BulkServiceComponentViewModel> serviceComponents)
        {
            var bulkServiceComponentViewModels = new List<BulkServiceComponentViewModel>();

            try
            {
                if (_appUserContext.Current.CurrentCustomer != null &&
                    _appUserContext.Current.CurrentCustomer.Id > 0 &&
                    serviceComponents != null)
                {

                    bulkServiceComponentViewModels = serviceComponents.ToList();

                    // Get the components to be processed. These must have both level 1 component and component name specified
                    var processServiceComponents = bulkServiceComponentViewModels
                        .Where(x => x.ServiceComponentLevel1Id.HasValue &&
                            x.ServiceComponentLevel1Id != 0 &&
                            !string.IsNullOrEmpty(x.ComponentName))
                        .ToList();

                    var now = DateTime.Now;
                    var userName = _contextManager.UserManager.Name;
                    var updateComponents = new List<ServiceComponent>();

                    // We can only add a Level 2 component from a Level 1 component
                    var parentServiceComponentId = bulkServiceComponentViewModels[0].ServiceComponentLevel1Id;

                    // Get the parent component.
                    var parentComponent = _serviceComponentService
                        .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                        .SingleOrDefault(x => x.Id == parentServiceComponentId);

                    if (parentComponent == null ||
                        parentComponent.ComponentLevel != (int)ServiceComponentLevel.Level1)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ServiceComponentCannotBeFound,
                            WebResources.ServiceComponentCannotBeFound);
                    }
                    else
                    {
                        foreach (var serviceComponent in processServiceComponents)
                        {
                            parentComponent.UpdatedBy = userName;
                            parentComponent.UpdatedDate = now;
                            if (parentComponent.ChildServiceComponents == null)
                            {
                                parentComponent.ChildServiceComponents = new List<ServiceComponent>();
                            }

                            var childComponent = new ServiceComponent
                            {
                                ComponentLevel = (int)ServiceComponentLevel.Level2,
                                ComponentName = serviceComponent.ComponentName,
                                DiagramOrder = serviceComponent.DiagramOrder == null || serviceComponent.DiagramOrder < 1 ? 5 : serviceComponent.DiagramOrder.Value,
                                ParentServiceComponentId = parentServiceComponentId,
                                ServiceFunctionId = parentComponent.ServiceFunctionId,
                                InsertedBy = userName,
                                InsertedDate = now,
                                UpdatedBy = userName,
                                UpdatedDate = now
                            };

                            parentComponent.ChildServiceComponents.Add(childComponent);

                        }

                        if (processServiceComponents.Any())
                        {
                            updateComponents.Add(parentComponent);
                            _serviceComponentService.Update(updateComponents);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(bulkServiceComponentViewModels.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public JsonResult GetServiceComponentLevels()
        {
            var levels = EnumExtensions.AsSelectListItems<ServiceComponentLevel>();
            return Json(levels, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Edit(string level, int id)
        {
            // Get the Service Component.
            var component = _serviceComponentService
                .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .SingleOrDefault(x => x.Id == id);

            var redirectToIndex = RedirectToAction("Index", new { level = level });

            // Component cannot be found, so send user back to Index.
            if (component == null)
            {
                return redirectToIndex;
            }

            // Check what kind of edit is required.
            var editState = _serviceComponentHelper.GetEditState(component);
            EditServiceComponentViewModel model;

            switch (editState)
            {
                case ServiceComponentEditState.Level1WithChildComponent:
                    model = Mapper.Map<EditServiceComponentLevel1WithChildComponentViewModel>(component);
                    model.ParentName = component.ServiceFunction.FunctionType.FunctionName;
                    model.ReturnUrl = GetRedirect(level, component.ServiceFunctionId);
                    break;
                case ServiceComponentEditState.Level1WithNoChildComponentOrResolver:
                    model = Mapper.Map<EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel>(component);
                    model.ParentName = component.ServiceFunction.FunctionType.FunctionName;
                    model.ReturnUrl = GetRedirect(level, component.ServiceFunctionId);
                    break;
                case ServiceComponentEditState.Level1WithResolver:
                    if (component.Resolver.ServiceDeliveryUnitType == null)
                    {
                        component.Resolver.ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData();
                    }
                    if (component.Resolver.ResolverGroupType == null)
                    {
                        component.Resolver.ResolverGroupType = new ResolverGroupTypeRefData();
                    }
                    model = Mapper.Map<EditServiceComponentLevel1WithResolverViewModel>(component);
                    model.ParentName = component.ServiceFunction.FunctionType.FunctionName;
                    model.ReturnUrl = GetRedirect(level, component.ServiceFunctionId);
                    break;
                case ServiceComponentEditState.Level2:
                    if (component.Resolver == null)
                    {
                        // Initialise the Resolver as the mapper will assume you are editing a level 2 with resolver
                        component.Resolver = new Resolver
                        {
                            ServiceDeliveryOrganisationType = new ServiceDeliveryOrganisationTypeRefData(),
                            ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData(),
                            ResolverGroupType = new ResolverGroupTypeRefData(),
                        };
                    }
                    else
                    {
                        // Initialise the types as required as mapping will assume you are editing a level 2 with resolver
                        if (component.Resolver.ServiceDeliveryOrganisationType == null)
                        {
                            component.Resolver.ServiceDeliveryOrganisationType = new ServiceDeliveryOrganisationTypeRefData();
                        }
                        if (component.Resolver.ServiceDeliveryUnitType == null)
                        {
                            component.Resolver.ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData();
                        }
                        if (component.Resolver.ResolverGroupType == null)
                        {
                            component.Resolver.ResolverGroupType = new ResolverGroupTypeRefData();
                        }
                    }
                    model = Mapper.Map<EditServiceComponentLevel2ViewModel>(component);
                    model.ParentName = component.ParentServiceComponent.ComponentName;
                    model.ReturnUrl = GetRedirect(level, component.ServiceFunctionId, component.ParentServiceComponentId.Value);
                    break;
                default:
                    return redirectToIndex;
            }

            if (component.Resolver != null && component.Resolver.Id > 0)
            {
                model.EditUrl = Url.Action("Edit", "Resolver", new { level = level, component.Resolver.Id });
            }

            model.EditLevel = level;

            return View("Edit" + level, model);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevel1WithChildComponent(EditServiceComponentLevel1WithChildComponentViewModel model)
        {
            var preChecks = SaveServiceComponentPreChecks(model, ServiceComponentEditState.Level1WithChildComponent);
            if (!preChecks.IsValid)
            {
                return preChecks.Result;
            }

            var component = preChecks.ServiceComponent;

            component.UpdatedDate = DateTime.Now;
            component.UpdatedBy = _contextManager.UserManager.Name;
            component.ComponentName = model.ComponentName.ComponentName;
            component.DiagramOrder = model.DiagramOrder.DiagramOrder ?? 5;
            component.ServiceActivities = model.ServiceActivities.ServiceActivities;

            _serviceComponentService.Update(component);

            return GetRedirectAction(model.EditLevel, component.ServiceFunctionId);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevel1WithNoChildComponentOrResolver(EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel model)
        {
            var preChecks = SaveServiceComponentPreChecks(model, ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            if (!preChecks.IsValid)
            {
                return preChecks.Result;
            }

            var component = preChecks.ServiceComponent;
            var now = preChecks.ServiceComponent.UpdatedDate;
            var userName = _contextManager.UserManager.Name;

            component.ComponentName = model.ComponentName.ComponentName;
            component.DiagramOrder = model.DiagramOrder.DiagramOrder ?? 5;
            component.ServiceActivities = model.ServiceActivities.ServiceActivities;

            if (model.InputMode == 0)
            {
                if (model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId.HasValue &&
                    model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId > 0)
                {
                    // Add Resolver
                    component.Resolver = new Resolver
                    {
                        ServiceDeskId = component.ServiceFunction.ServiceDomain.ServiceDeskId,
                        InsertedDate = now,
                        InsertedBy = userName,
                        UpdatedDate = now,
                        UpdatedBy = userName,
                    };

                    SaveServiceComponentResolver(component, model);
                }
            }
            else if (model.InputMode == 1)
            {
                // Add Child Component
                if (!string.IsNullOrEmpty(model.ChildComponent.ComponentName))
                {
                    var childServiceComponent = new ServiceComponent
                    {
                        ComponentLevel = (int)ServiceComponentLevel.Level2,
                        ComponentName = model.ChildComponent.ComponentName,
                        ServiceFunctionId = component.ServiceFunctionId,
                        InsertedBy = userName,
                        InsertedDate = now,
                        UpdatedBy = userName,
                        UpdatedDate = now
                    };

                    if (component.ChildServiceComponents != null)
                    {
                        component.ChildServiceComponents.Add(childServiceComponent);
                    }
                    else
                    {
                        component.ChildServiceComponents = new List<ServiceComponent> { childServiceComponent };
                    }
                }
            }

            component.UpdatedDate = now;
            component.UpdatedBy = userName;

            _serviceComponentService.Update(component);

            return GetRedirectAction(model.EditLevel, component.ServiceFunctionId);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevel1WithResolver(EditServiceComponentLevel1WithResolverViewModel model)
        {
            var preChecks = SaveServiceComponentPreChecks(model, ServiceComponentEditState.Level1WithResolver);
            if (!preChecks.IsValid)
            {
                return preChecks.Result;
            }

            var component = preChecks.ServiceComponent;
            component.ComponentName = model.ComponentName.ComponentName;
            component.DiagramOrder = model.DiagramOrder.DiagramOrder ?? 5;
            component.ServiceActivities = model.ServiceActivities.ServiceActivities;

            SaveServiceComponentResolver(component, model);

            _serviceComponentService.Update(component);

            return GetRedirectAction(model.EditLevel, component.ServiceFunctionId);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevel2(EditServiceComponentLevel2ViewModel model)
        {
            var preChecks = SaveServiceComponentPreChecks(model, ServiceComponentEditState.Level2);

            if (!preChecks.IsValid)
            {
                return preChecks.Result;
            }

            var component = preChecks.ServiceComponent;
            component.ComponentName = model.ComponentNameLevel.ComponentName;
            component.DiagramOrder = model.ComponentNameLevel.DiagramOrder ?? 5;
            component.ServiceActivities = model.ServiceActivities.ServiceActivities;

            if (model.ComponentNameLevel.ComponentLevel == ServiceComponentLevel.Level1.ToString())
            {
                component.ComponentLevel = (int)ServiceComponentLevel.Level1;
                component.ParentServiceComponent.ChildServiceComponents = null;
                component.ParentServiceComponent = null;
            }

            var now = preChecks.ServiceComponent.UpdatedDate;
            var userName = _contextManager.UserManager.Name;

            if (component.Resolver == null)
            {
                if (model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId.HasValue &&
                    model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId > 0)
                {
                    component.Resolver = new Resolver
                    {
                        ServiceDeskId = component.ServiceFunction.ServiceDomain.ServiceDeskId,
                        InsertedDate = now,
                        InsertedBy = userName,
                        UpdatedDate = now,
                        UpdatedBy = userName,
                    };

                    SaveServiceComponentResolver(component, model);
                }
            }
            else
            {
                SaveServiceComponentResolver(component, model);
            }

            _serviceComponentService.Update(component);

            return GetRedirectAction(model.EditLevel, component.ServiceFunctionId, component.ParentServiceComponentId.Value);
        }

        #region Helpers

        protected string GetRedirect(string level, int serviceFunctionId = 0, int serviceComponentId = 0)
        {
            if (!string.IsNullOrEmpty(level))
            {
                if (serviceComponentId > 0)
                {
                    return Url.Action("Edit", "ServiceComponent", new { level, Id = serviceComponentId });
                }

                if (serviceFunctionId > 0)
                {
                    return Url.Action("Edit", "ServiceFunction", new { level, Id = serviceFunctionId });
                }

                return Url.Action("Index", "ServiceComponent", new { level });
            }

            if (serviceComponentId > 0)
            {
                return Url.Action("Edit", "ServiceComponent", new { level, Id = serviceComponentId });
            }

            return Url.Action("Edit", "ServiceFunction", new { level, Id = serviceFunctionId });
        }

        protected ActionResult GetRedirectAction(string level, int serviceFunctionId = 0, int serviceComponentId = 0)
        {
            if (!string.IsNullOrEmpty(level))
            {
                if (serviceComponentId > 0)
                {
                    return RedirectToAction("Edit", "ServiceComponent", new { level, Id = serviceComponentId });
                }

                if (serviceFunctionId > 0)
                {
                    return RedirectToAction("Edit", "ServiceFunction", new { level, Id = serviceFunctionId });
                }

                return RedirectToAction("Index", "ServiceComponent", new { level });
            }

            if (serviceComponentId > 0)
            {
                return RedirectToAction("Edit", "ServiceComponent", new { level, Id = serviceComponentId });
            }

            return RedirectToAction("Edit", "ServiceFunction", new { level, Id = serviceFunctionId });
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteServiceComponent(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // RetryableOperation to Log any Unexpected Errors and Alert Error Instance id.
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        var serviceComponent = _serviceComponentService.GetById(id);
                        _serviceComponentService.Delete(serviceComponent);
                    });
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", Message = ex.Message };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Add Service Component Grid - Not used for any function, but required by Inline edit to work.

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxServiceAddComponentGrid([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult UpdateAjaxServiceAddComponentGrid([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DestroyAjaxServiceAddComponentGrid([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);
        }

        #endregion
    }
}