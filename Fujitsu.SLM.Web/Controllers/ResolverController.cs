using AutoMapper;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;
using Fujitsu.SLM.Services.Entities;
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

namespace Fujitsu.SLM.Web.Controllers
{
    public class ResolverController : BaseServiceComponentController
    {
        private readonly IServiceComponentService _serviceComponentService;
        private readonly IOperationalProcessTypeRefDataService _operationalProcessTypeRefDataService;
        private readonly IServiceDeliveryOrganisationTypeRefDataService _serviceDeliveryOrganisationTypeRefDataService;
        private readonly IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitTypeRefDataService;
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private readonly IResolverService _resolverService;
        private readonly IServiceFunctionService _serviceFunctionService;
        private readonly IParameterService _parameterService;
        private readonly IRepository<OperationalProcessType> _operationalTypeRepository;
        private readonly IResolverHelper _resolverHelper;
        private readonly IServiceComponentHelper _serviceComponentHelper;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public ResolverController(IServiceComponentService serviceComponentService,
            IOperationalProcessTypeRefDataService operationalProcessTypeRefDataService,
            IServiceDeliveryOrganisationTypeRefDataService serviceDeliveryOrganisationTypeRefDataService,
            IServiceDeliveryUnitTypeRefDataService serviceDeliveryUnitTypeRefDataService,
            IResolverGroupTypeRefDataService resolverGroupTypeRefDataService,
            IResolverService resolverService,
            IServiceDeskService serviceDeskService,
            IServiceFunctionService serviceFunctionService,
            IParameterService parameterService,
            IRepository<OperationalProcessType> operationalTypeRepository,
            IResolverHelper resolverHelper,
            IServiceComponentHelper serviceComponentHelper,
            IContextManager contextManager,
            IAppUserContext appUserContext)
            : base(contextManager,
                serviceComponentService,
                serviceFunctionService,
                serviceDeliveryOrganisationTypeRefDataService,
                serviceDeliveryUnitTypeRefDataService,
                resolverGroupTypeRefDataService,
                parameterService,
                appUserContext,
                serviceComponentHelper)
        {
            if (serviceComponentService == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentService));
            }

            if (operationalProcessTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(operationalProcessTypeRefDataService));
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

            if (resolverService == null)
            {
                throw new ArgumentNullException(nameof(resolverService));
            }

            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }

            if (serviceFunctionService == null)
            {
                throw new ArgumentNullException(nameof(serviceFunctionService));
            }

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            if (operationalTypeRepository == null)
            {
                throw new ArgumentNullException(nameof(operationalTypeRepository));
            }

            if (resolverHelper == null)
            {
                throw new ArgumentNullException(nameof(resolverHelper));
            }

            if (serviceComponentHelper == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentHelper));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            _serviceComponentService = serviceComponentService;
            _operationalProcessTypeRefDataService = operationalProcessTypeRefDataService;
            _serviceDeliveryOrganisationTypeRefDataService = serviceDeliveryOrganisationTypeRefDataService;
            _serviceDeliveryUnitTypeRefDataService = serviceDeliveryUnitTypeRefDataService;
            _resolverGroupTypeRefDataService = resolverGroupTypeRefDataService;
            _resolverService = resolverService;
            _serviceDeskService = serviceDeskService;
            _serviceFunctionService = serviceFunctionService;
            _parameterService = parameterService;
            _operationalTypeRepository = operationalTypeRepository;
            _resolverHelper = resolverHelper;
            _serviceComponentHelper = serviceComponentHelper;
            _contextManager = contextManager;
            _appUserContext = appUserContext;
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            _operationalProcessTypeRefDataService.PurgeOrphans();

            var customerId = _appUserContext.Current.CurrentCustomer.Id;
            var serviceComponentCount = _serviceComponentService
                .GetByCustomerWithHierarchy(customerId)
                .Select(s => s.ServiceComponent.Id)
                .Count();
            var serviceDeskCount = _serviceDeskService.GetByCustomer(customerId).Select(s => s.Id).Count();

            var model = new ViewResolverViewModel
            {
                EditLevel = level,
                CanMoveResolver = level.Equals(LevelName.LevelZero.ToString()) ? serviceDeskCount > 1 : serviceComponentCount > 1
            };
            return View(level, model);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Add(string level, int? id)
        {
            object vm;
            if (level.Equals(LevelName.LevelZero.ToString()))
            {
                vm = AddResolverLevelZero(level);
            }
            else
            {
                vm = AddResolver(level, id);
            }

            return View("Add" + level, vm);
        }

        private AddResolverViewModel AddResolver(string level, int? id)
        {
            var vm = new AddResolverViewModel
            {
                EditLevel = level,
                HasServiceComponentContext = id.HasValue && id.Value != 0,
                ServiceComponentId = id
            };

            vm.ReturnUrl = vm.HasServiceComponentContext
                ? Url.Action("Edit", "ServiceComponent", new { level = vm.EditLevel, id = vm.ServiceComponentId })
                : Url.Action("Index", "Resolver", new { level = vm.EditLevel });

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var customerId = _appUserContext.Current.CurrentCustomer.Id;
                var serviceComponents = _serviceComponentService.GetByCustomer(customerId).ToList();

                if (id.HasValue && id.Value > 0)
                {
                    vm.ServiceComponentName = serviceComponents.First(x => x.Id == id.Value).ComponentName;
                }
                else if (serviceComponents.Count == 1)
                {
                    vm.ServiceComponentId = serviceComponents.First().Id;
                    vm.ServiceComponentName = serviceComponents.First().ComponentName;
                    vm.HasServiceComponentContext = true;
                }

                AddResolverReferenceData(vm);
            }
            return vm;
        }

        private AddResolverLevelZeroViewModel AddResolverLevelZero(string level)
        {
            var vm = new AddResolverLevelZeroViewModel
            {
                EditLevel = level,
                ReturnUrl = Url.Action("Index", "Resolver", new { level = level })
            };

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var customerId = _appUserContext.Current.CurrentCustomer.Id;
                vm.ServiceDesks.AddRange(_serviceDeskService.GetByCustomer(customerId)
                    .Select(x => new SelectListItem { Text = x.DeskName, Value = x.Id.ToString() })
                    .ToList());

                if (vm.ServiceDesks.Count == 1)
                {
                    vm.IsSingleDeskContext = true;
                    vm.SingleDeskId = Convert.ToInt32(vm.ServiceDesks[0].Value);
                }

                AddResolverLevelZeroReferenceData(vm);
            }
            return vm;
        }

        [Authorize(Roles = UserRoles.Viewer)]
        [HttpPost]
        public ActionResult ReadAjaxResolverGrid([DataSourceRequest] DataSourceRequest request, int? serviceComponentId)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    result = _serviceComponentService
                        .GetResolverByCustomerWithHierarchy(_appUserContext.Current.CurrentCustomer.Id)
                        .Where(w => (!serviceComponentId.HasValue || w.ServiceComponentId == serviceComponentId) &&
                                    (!string.IsNullOrEmpty(w.ResolverGroupName) ||
                                     !string.IsNullOrEmpty(w.ServiceDeliveryOrganisationTypeName) ||
                                     !string.IsNullOrEmpty(w.ServiceDeliveryUnitTypeName)))
                        .ToDataSourceResult(request, Mapper.Map<ResolverListItem, ResolverViewModel>);
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
        public ActionResult ReadAjaxResolverLevelZeroGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    // as agreed with Matt 12/11/2015 return ALL customer resolvers for the main Level 0 grid
                    result = _resolverService.GetListByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .ToDataSourceResult(request, Mapper.Map<ResolverListItem, ResolverLevelZeroViewModel>);
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
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxAddResolverGrid([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<BulkResolverViewModel> resolvers)
        {

            var bulkResolverViewModels = new List<BulkResolverViewModel>();

            try
            {
                if (_appUserContext.Current.CurrentCustomer != null &&
                    _appUserContext.Current.CurrentCustomer.Id > 0 &&
                    resolvers != null)
                {
                    bulkResolverViewModels = resolvers.ToList();
                    // pick any with data set, ignore empty rows
                    var processResolvers = bulkResolverViewModels
                        .Where(x => x.ServiceDeliveryOrganisationTypeId != 0 ||
                                    x.ServiceDeliveryUnitTypeId != 0 ||
                                    x.ResolverGroupTypeId != 0)
                        .ToList();

                    // now check if any are invalid
                    if (processResolvers.Any(x => x.ServiceDeliveryOrganisationTypeId == 0))
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ResolverServiceDeliveryOrgMandatory, WebResources.ResolverServiceDeliveryOrgMandatory);
                        // we are invalid here so return
                        return Json(bulkResolverViewModels.ToDataSourceResult(request, ModelState));
                    }

                    var distinctResolvers = processResolvers
                        .Select(s => s.ServiceComponentId)
                        .Distinct()
                        .ToList();

                    if (processResolvers.Count != distinctResolvers.Count)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ServiceComponentAddDuplicateParent,
                            WebResources.ResolverAddDuplicateServiceComponent);
                    }
                    else
                    {
                        var existingComponents = _serviceComponentService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .Where(x => distinctResolvers.Contains(x.Id))
                            .ToList();

                        var now = DateTime.Now;
                        var userName = _contextManager.UserManager.Name;

                        foreach (var processResolver in processResolvers)
                        {
                            var component = existingComponents
                                .SingleOrDefault(x => x.Id == processResolver.ServiceComponentId);

                            if (component == null)
                            {
                                continue;
                            }
                            var resolver = new Resolver
                            {
                                InsertedBy = userName,
                                InsertedDate = now,
                                UpdatedBy = userName,
                                UpdatedDate = now,
                                ServiceDeskId = component.ServiceFunction.ServiceDomain.ServiceDeskId,
                                ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypeRefDataService.GetById(processResolver.ServiceDeliveryOrganisationTypeId)
                            };


                            if (processResolver.ServiceDeliveryUnitTypeId > 0)
                            {
                                resolver.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDataService.GetById(processResolver.ServiceDeliveryUnitTypeId);

                            }

                            if (processResolver.ResolverGroupTypeId > 0)
                            {
                                var resolverGroupType = _resolverGroupTypeRefDataService.GetById(processResolver.ResolverGroupTypeId);
                                resolver.ResolverGroupType = resolverGroupType;

                                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                                if (!resolverGroupType.Visible &&
                                    _resolverGroupTypeRefDataService.GetNumberOfResolverGroupTypeReferences(resolverGroupType.Id) >= customerSpecificTypeThreshold - 1)
                                {
                                    resolverGroupType.Visible = true;
                                    _resolverGroupTypeRefDataService.Update(resolverGroupType);
                                }
                            }

                            resolver.ServiceComponent = component;
                            _resolverService.Create(resolver, false);

                            component.UpdatedDate = now;
                            component.UpdatedBy = userName;
                            component.Resolver = resolver;
                        }
                        _serviceComponentService.Update(existingComponents);
                    }
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(bulkResolverViewModels.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxAddResolverLevelZeroGrid([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<BulkResolverLevelZeroViewModel> resolvers)
        {

            var bulkResolverLevelZeroViewModels = new List<BulkResolverLevelZeroViewModel>();

            try
            {
                if (_appUserContext.Current.CurrentCustomer != null &&
                   _appUserContext.Current.CurrentCustomer.Id > 0 &&
                   resolvers != null)
                {
                    bulkResolverLevelZeroViewModels = resolvers.ToList();
                    // pick any with data set, ignore empty rows
                    var processResolvers = bulkResolverLevelZeroViewModels
                        .Where(x => x.ServiceDeliveryOrganisationTypeId != 0 ||
                                    x.ServiceDeliveryUnitTypeId != 0 ||
                                    x.ResolverGroupTypeId != 0)
                        .ToList();

                    // now check if any are invalid
                    if (processResolvers.Any(x => x.ServiceDeskId == 0))
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ResolverSerivceDeskMandatory,
                                    WebResources.ResolverSerivceDeskMandatory);
                    }
                    if (processResolvers.Any(x => x.ServiceDeliveryOrganisationTypeId == 0))
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ResolverServiceDeliveryOrgMandatory,
                                    WebResources.ResolverServiceDeliveryOrgMandatory);
                    }

                    if (!ModelState.IsValid)
                    {
                        return Json(bulkResolverLevelZeroViewModels.ToDataSourceResult(request, ModelState));
                    }

                    var distinctResolvers = processResolvers
                        .Select(s => s.ServiceDeskId)
                        .Distinct()
                        .ToList();

                    var existingDesks = _serviceDeskService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .Where(x => distinctResolvers.Contains(x.Id))
                            .ToList();

                    var now = DateTime.Now;
                    var userName = _contextManager.UserManager.Name;

                    foreach (var processResolver in processResolvers)
                    {
                        var desk = existingDesks
                                .SingleOrDefault(x => x.Id == processResolver.ServiceDeskId);

                        if (desk == null)
                        {
                            continue;
                        }

                        var resolver = new Resolver
                        {
                            InsertedBy = userName,
                            InsertedDate = now,
                            UpdatedBy = userName,
                            UpdatedDate = now,
                            ServiceDeskId = processResolver.ServiceDeskId,
                        };

                        if (!string.IsNullOrEmpty(processResolver.ServiceDeliveryUnitNotes))
                        {
                            resolver.ServiceDeliveryUnitNotes = processResolver.ServiceDeliveryUnitNotes.Replace("\n", "\r\n");
                        }

                        resolver.ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypeRefDataService.GetById(processResolver.ServiceDeliveryOrganisationTypeId);

                        if (processResolver.ServiceDeliveryUnitTypeId > 0)
                        {
                            resolver.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDataService.GetById(processResolver.ServiceDeliveryUnitTypeId);
                        }

                        if (processResolver.ResolverGroupTypeId > 0)
                        {
                            var resolverGroupType = _resolverGroupTypeRefDataService.GetById(processResolver.ResolverGroupTypeId);
                            resolver.ResolverGroupType = resolverGroupType;
                            var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                            if (!resolverGroupType.Visible &&
                                _resolverGroupTypeRefDataService.GetNumberOfResolverGroupTypeReferences(resolverGroupType.Id) >= customerSpecificTypeThreshold - 1)
                            {
                                resolverGroupType.Visible = true;
                                _resolverGroupTypeRefDataService.Update(resolverGroupType);
                            }
                        }

                        resolver.ServiceDesk = desk;
                        _resolverService.Create(resolver, false);

                        desk.UpdatedDate = now;
                        desk.UpdatedBy = userName;
                    }

                    _serviceDeskService.Update(existingDesks);
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(bulkResolverLevelZeroViewModels.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteAjaxResolverGrid([DataSourceRequest] DataSourceRequest request,
            ResolverViewModel model)
        {
            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var serviceComponent = _serviceComponentService
                        .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                        .SingleOrDefault(x => x.Id == model.ServiceComponentId);

                    if (serviceComponent != null)
                    {
                        if (serviceComponent.Resolver != null && !_resolverHelper.CanDelete(serviceComponent.Resolver))
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ResolverCannotBeDeletedDueToDependents,
                                WebResources.ResolverCannotBeDeletedDueToDependents);
                        }
                        else
                        {
                            var userName = _contextManager.UserManager.Name;
                            var now = DateTime.Now;

                            serviceComponent.UpdatedDate = now;
                            serviceComponent.UpdatedBy = userName;
                            serviceComponent.Resolver = null;
                            var resolver = _resolverService.GetById(model.Id);
                            _resolverService.Delete(resolver, false);
                            _serviceComponentService.Update(serviceComponent);
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

        [HttpPost]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteAjaxResolverLevelZeroGrid([DataSourceRequest] DataSourceRequest request,
            ResolverViewModel model)
        {
            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var resolver = _resolverService.GetById(model.Id);

                    if (!_resolverHelper.CanDelete(resolver))
                    {
                        // cannot delete this resolver here
                        ModelState.AddModelError(ModelStateErrorNames.ResolverCannotBeDeletedDueToDependents,
                                WebResources.ResolverCannotBeDeletedDueToDependents);
                    }
                    else
                    {
                        var desk = _serviceDeskService.GetById(resolver.ServiceDeskId);
                        var userName = _contextManager.UserManager.Name;
                        var now = DateTime.Now;

                        desk.UpdatedDate = now;
                        desk.UpdatedBy = userName;

                        _resolverService.Delete(resolver, false);
                        _serviceDeskService.Update(desk);
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

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteResolver(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // RetryableOperation to Log any Unexpected Errors and Alert Error Instance id.
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        var resolver = _resolverService.GetById(id);
                        _resolverService.Delete(resolver);
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

        [Authorize(Roles = UserRoles.Architect)]
        [HttpGet]
        public ActionResult Edit(string level, int id)
        {
            if (!string.IsNullOrEmpty(level) && level.Equals(LevelName.LevelZero.ToString()))
            {
                var resolver = _resolverService.GetById(id);
                InitialiseResolverDependencies(resolver);
                var model = Mapper.Map<EditResolverLevelZeroViewModel>(resolver);
                model.ReturnUrl = GetRedirect(level);
                model.EditLevel = level;

                return View("Edit" + level, model);
            }
            else
            {
                // Get the existing Service Component.
                var component = _serviceComponentService
                    .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .SingleOrDefault(x => x.Resolver != null && x.Resolver.Id == id);

                var redirectToIndex = RedirectToAction("Index", new { level });

                // Component cannot be found, so send user back to Index.
                if (component == null)
                {
                    return redirectToIndex;
                }

                // Check that the edit state is correct.
                var editState = _serviceComponentHelper.GetEditState(component);
                EditServiceComponentViewModel model;
                // Should have used DI here, but overkill for what is needed and more scenarios unlikely to be added.
                switch (editState)
                {
                    case ServiceComponentEditState.Level1WithResolver:
                        // Initialise the types as required as mapping will assume you are editing a level 2 with resolver
                        InitialiseResolverDependencies(component.Resolver);
                        model = Mapper.Map<EditResolverViewModel>(component);
                        model.ReturnUrl = GetRedirect(level, component.Id);
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
                            InitialiseResolverDependencies(component.Resolver);
                        }
                        model = Mapper.Map<EditResolverViewModel>(component);
                        model.ReturnUrl = GetRedirect(level, component.Id);
                        break;
                    default:
                        return redirectToIndex;
                }
                model.Id = component.Id;
                model.EditLevel = level;

                return View("Edit" + level, model);
            }
        }

        private static void InitialiseResolverDependencies(Resolver resolver)
        {
            if (resolver.ServiceDeliveryOrganisationType == null)
            {
                resolver.ServiceDeliveryOrganisationType = new ServiceDeliveryOrganisationTypeRefData();
            }
            if (resolver.ServiceDeliveryUnitType == null)
            {
                resolver.ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData();
            }
            if (resolver.ResolverGroupType == null)
            {
                resolver.ResolverGroupType = new ResolverGroupTypeRefData();
            }
            if (resolver.OperationalProcessTypes == null)
            {
                resolver.OperationalProcessTypes = new List<OperationalProcessType>();
            }
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevelZero(EditResolverLevelZeroViewModel model)
        {
            var resolver = _resolverService.GetById(model.Id);
            var desk = _serviceDeskService.GetById(resolver.ServiceDeskId);
            var userName = _contextManager.UserManager.Name;
            var now = DateTime.Now;

            desk.UpdatedDate = now;
            desk.UpdatedBy = userName;

            if (model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId.HasValue && model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId > 0)
            {
                resolver.ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypeRefDataService.GetById(model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId.Value);
            }
            else
            {
                resolver.ServiceDeliveryOrganisationType = null;
            }

            if (model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId.HasValue &&
                model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId > 0)
            {
                resolver.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDataService.GetById(model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId.Value);
            }
            else
            {
                resolver.ServiceDeliveryUnitType = null;
            }

            if (model.ResolverGroup.ResolverGroupTypeId.HasValue && model.ResolverGroup.ResolverGroupTypeId > 0)
            {
                var resolverGroupType = _resolverGroupTypeRefDataService.GetById(model.ResolverGroup.ResolverGroupTypeId.Value);
                resolver.ResolverGroupType = resolverGroupType;
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
                resolver.ResolverGroupType = null;
            }

            resolver.ServiceDeliveryUnitNotes = model.ResolverServiceDeliveryUnit.ServiceDeliveryUnitNotes;
            resolver.ServiceDeliveryOrganisationNotes = model.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationNotes;

            resolver.UpdatedDate = now;
            resolver.UpdatedBy = userName;
            desk.UpdatedDate = now;
            desk.UpdatedBy = userName;

            _resolverService.Update(resolver, false);
            _serviceDeskService.Update(desk);

            return RedirectToAction("Index", "Resolver", new { level = model.EditLevel });
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditResolverViewModel model)
        {
            var preChecks = SaveServiceComponentPreChecks(model, new[] { ServiceComponentEditState.Level1WithResolver, ServiceComponentEditState.Level2 });
            if (!preChecks.IsValid)
            {
                return preChecks.Result;
            }

            var component = preChecks.ServiceComponent;

            var now = preChecks.ServiceComponent.UpdatedDate;
            var userName = _contextManager.UserManager.Name;

            SaveServiceComponentResolver(component, model);

            var componentOperationalProcessTypes = component.Resolver.OperationalProcessTypes?.ToList() ?? new List<OperationalProcessType>();

            if (model.OperationalProcesses?.OperationalProcessTypes == null || model.OperationalProcesses.OperationalProcessTypes.Length == 0)
            {
                if (componentOperationalProcessTypes.Any())
                {
                    foreach (var operationalProcessType in componentOperationalProcessTypes)
                    {
                        _operationalTypeRepository.Delete(operationalProcessType);
                    }

                    component.Resolver.OperationalProcessTypes = null;
                }
            }
            else
            {
                if (component.Resolver.OperationalProcessTypes != null && component.Resolver.OperationalProcessTypes.Any())
                {
                    // For every operational process type which no longer exists = Remove
                    var modelOperationalProcessTypes = model.OperationalProcesses.OperationalProcessTypes.ToList();
                    foreach (var operationalProcessType in componentOperationalProcessTypes.Where(operationalProcessType => !modelOperationalProcessTypes
                        .Contains(operationalProcessType.OperationalProcessTypeRefDataId)))
                    {
                        _operationalTypeRepository.Delete(operationalProcessType);
                    }
                }

                // For every operational process type which doesn't already exist = Add
                foreach (var operationProcessTypeId in model.OperationalProcesses.OperationalProcessTypes
                    .Where(operationProcessTypeId => component.Resolver.OperationalProcessTypes.All(x => x.OperationalProcessTypeRefDataId != operationProcessTypeId)))
                {
                    component.Resolver.OperationalProcessTypes.Add(new OperationalProcessType
                    {
                        OperationalProcessTypeRefDataId = operationProcessTypeId
                    });

                    var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                    var operationalProcessType = _operationalProcessTypeRefDataService.GetById(operationProcessTypeId);
                    if (!operationalProcessType.Visible &&
                        _operationalProcessTypeRefDataService.GetNumberOfOperationalProcessTypeReferences(operationalProcessType.Id) >= customerSpecificTypeThreshold - 1)
                    {
                        operationalProcessType.Visible = true;
                        _operationalProcessTypeRefDataService.Update(operationalProcessType);
                    }
                }


            }

            component.Resolver.UpdatedBy = userName;
            component.Resolver.UpdatedDate = now;

            _serviceComponentService.Update(component);

            _operationalProcessTypeRefDataService.PurgeOrphans();

            return GetRedirectAction(model.EditLevel, component.Id);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpGet]
        public ActionResult Move()
        {
            var vm = new MoveResolverViewModel();
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                vm.ServiceComponents.AddRange(GetServiceComponentsWithoutDependencies(null));
            }
            return PartialView("_Move", vm);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpGet]
        public ActionResult MoveLevelZero()
        {
            var vm = new MoveResolverLevelZeroViewModel();
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var desks = _serviceDeskService.GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .Select(x => new SelectListItem { Text = x.DeskName, Value = x.Id.ToString() })
                .ToList();
                vm.Desks.AddRange(desks);
            }
            return PartialView("_MoveLevelZero", vm);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Move(MoveResolverViewModel model)
        {
            var result = GetJsonSuccessResponse();
            if (ModelState.IsValid)
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    try
                    {
                        var componentId = _serviceComponentService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .Where(w => w.Id == model.ServiceComponentId)
                            .Select(s => s.Id)
                            .SingleOrDefault();

                        var destinationComponentId = _serviceComponentService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .Where(w => w.Id == model.DestinationServiceComponentId)
                            .Select(s => s.Id)
                            .SingleOrDefault();

                        if (componentId == 0 || destinationComponentId == 0)
                        {
                            result = GetJsonErrorResponse(WebResources.ServiceComponentCannotBeFound);
                        }
                        else
                        {
                            _serviceComponentService.MoveResolver(_appUserContext.Current.CurrentCustomer.Id,
                                componentId,
                                destinationComponentId);
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

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveLevelZero(MoveResolverLevelZeroViewModel model)
        {
            var result = GetJsonSuccessResponse();
            if (ModelState.IsValid)
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    try
                    {
                        _resolverService.Move(model.Id, model.DestinationDeskId);
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

        #region Helpers

        private IEnumerable<SelectListItem> GetServiceComponentsWithoutDependencies(int? serviceComponentId)
        {
            return _serviceComponentService
                .GetByCustomerWithHierarchy(_appUserContext.Current.CurrentCustomer.Id)
                .Where(x => (!x.ServiceComponent.ChildServiceComponents.Any()) &&
                            x.ServiceComponent.Resolver == null &&
                            (serviceComponentId == null || x.ServiceComponent.Id == serviceComponentId.Value))
                .Select(x => new SelectListItem
                {
                    Text = string.Concat(x.ServiceDeskName, UnicodeCharacters.DoubleArrowRight,
                        x.ServiceDomainName, UnicodeCharacters.DoubleArrowRight,
                        x.ServiceFunctionName, UnicodeCharacters.DoubleArrowRight,
                        x.ServiceComponent.ParentServiceComponent != null ?
                        string.Concat(x.ServiceComponent.ParentServiceComponent.ComponentName, UnicodeCharacters.DoubleArrowRight, x.ServiceComponent.ComponentName)
                        : x.ServiceComponent.ComponentName),
                    Value = x.ServiceComponent.Id.ToString()
                })
                .ToList()
                .OrderBy(x => x.Text);
        }

        private void AddResolverLevelZeroReferenceData(AddResolverLevelZeroViewModel model)
        {
            model.ServiceDeliveryOrganisationTypes.AddRange(_serviceDeliveryOrganisationTypeRefDataService
                .All()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ServiceDeliveryOrganisationTypeName)
                .Select(x => new SelectListItem { Text = x.ServiceDeliveryOrganisationTypeName, Value = x.Id.ToString() })
                .ToList());

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                model.ServiceDeliveryUnitTypes.AddRange(_serviceDeliveryUnitTypeRefDataService
                    .GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ServiceDeliveryUnitTypeName)
                    .Select(x => new SelectListItem { Text = x.ServiceDeliveryUnitTypeName, Value = x.Id.ToString() })
                    .ToList());

                model.ResolverGroupTypes.AddRange(_resolverGroupTypeRefDataService
                    .GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ResolverGroupTypeName)
                    .Select(x => new SelectListItem { Text = x.ResolverGroupTypeName, Value = x.Id.ToString() })
                    .ToList());
            }
        }

        private void AddResolverReferenceData(AddResolverViewModel model)
        {
            model.ServiceDeliveryOrganisationTypes.AddRange(_serviceDeliveryOrganisationTypeRefDataService
                .All()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ServiceDeliveryOrganisationTypeName)
                .Select(x => new SelectListItem { Text = x.ServiceDeliveryOrganisationTypeName, Value = x.Id.ToString() })
                .ToList());

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                model.ServiceDeliveryUnitTypes.AddRange(_serviceDeliveryUnitTypeRefDataService
                    .GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ServiceDeliveryUnitTypeName)
                    .Select(x => new SelectListItem { Text = x.ServiceDeliveryUnitTypeName, Value = x.Id.ToString() })
                    .ToList());

                model.ResolverGroupTypes.AddRange(_resolverGroupTypeRefDataService
                    .GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ResolverGroupTypeName)
                    .Select(x => new SelectListItem { Text = x.ResolverGroupTypeName, Value = x.Id.ToString() })
                    .ToList());
            }

            // Only get components who do not have a child and resolver details.
            model.ServiceComponents.AddRange(GetServiceComponentsWithoutDependencies(model.ServiceComponentId));
        }

        protected string GetRedirect(string level, int serviceComponentId = 0)
        {
            if (serviceComponentId > 0)
            {
                return Url.Action("Edit", "ServiceComponent", new { level, Id = serviceComponentId });
            }

            return Url.Action("Index", "Resolver", new { level });
        }

        protected ActionResult GetRedirectAction(string level, int serviceComponentId = 0)
        {
            if (serviceComponentId > 0)
            {
                return RedirectToAction("Edit", "ServiceComponent", new { level, Id = serviceComponentId });
            }

            return RedirectToAction("Index", "Resolver", new { level });
        }

        #endregion

        #region Add Resolver Grid - Not used for any function, but required by Inline edit to work.

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxAddResolverGrid([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult UpdateAjaxAddResolverGrid([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DestroyAjaxAddResolverGrid([DataSourceRequest] DataSourceRequest request)
        {
            return Json(null);
        }

        #endregion
    }
}