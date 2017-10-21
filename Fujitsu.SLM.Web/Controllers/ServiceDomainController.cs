using AutoMapper;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
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
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    public class ServiceDomainController : Controller
    {
        private readonly IServiceDomainService _serviceDomainService;
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IDomainTypeRefDataService _domainTypeRefDataService;
        private readonly IParameterService _parameterService;
        private readonly ITemplateService _templateService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public ServiceDomainController(IServiceDomainService serviceDomainService,
            IServiceDeskService serviceDeskService,
            IDomainTypeRefDataService domainTypeRefDataService,
            IParameterService parameterService,
            ITemplateService templateService,
            IContextManager contextManager,
            IAppUserContext appUserContext)
        {
            if (serviceDomainService == null)
            {
                throw new ArgumentNullException(nameof(serviceDomainService));
            }

            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }

            if (domainTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(domainTypeRefDataService));
            }

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            if (templateService == null)
            {
                throw new ArgumentNullException(nameof(templateService));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            _serviceDomainService = serviceDomainService;
            _serviceDeskService = serviceDeskService;
            _domainTypeRefDataService = domainTypeRefDataService;
            _parameterService = parameterService;
            _templateService = templateService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            var serviceDeskCount = _serviceDeskService
                .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .Select(s => s.Id)
                .Count();

            var templates = _templateService.AllTemplates().ToList();
            var vm = new ViewServiceDomainViewModel
            {
                EditLevel = level,
                CanMoveServiceDomain = serviceDeskCount > 1,
                CanImportServiceConfiguratorTemplate = templates.Any(x => x.TemplateType == TemplateTypeNames.SORT),
                CanImportServiceLandscapeTemplate = templates.Any(x => x.TemplateType == TemplateTypeNames.SLM)
            };

            return View(level, vm);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Add(string level, int id)
        {
            var vm = new AddServiceDomainViewModel
            {
                ServiceDeskId = id,
                EditLevel = level,
                HasServiceDeskContext = id != 0
            };

            vm.ReturnUrl = vm.HasServiceDeskContext
                ? Url.Action("Edit", "ServiceDesk", new { level = vm.EditLevel, id = vm.ServiceDeskId })
                : Url.Action("Index", "ServiceDomain", new { level = vm.EditLevel, id = vm.ServiceDeskId });

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var customerId = _appUserContext.Current.CurrentCustomer.Id;
                var serviceDesks = _serviceDeskService.GetByCustomer(customerId).ToList();

                if (id > 0)
                {
                    vm.ServiceDeskName =
                        serviceDesks.First(x => x.Id == id).DeskName;
                }
                else if (serviceDesks.Count == 1)
                {
                    // We only have one service desk no need for the user to select it
                    vm.ServiceDeskId = serviceDesks.First().Id;
                    vm.ServiceDeskName = serviceDesks.First().DeskName;
                    vm.HasServiceDeskContext = true;
                }
                else
                {
                    vm.ServiceDesks.AddRange(serviceDesks.Select(s => new SelectListItem { Text = s.DeskName, Value = s.Id.ToString(CultureInfo.InvariantCulture) }).ToList());
                }

                var serviceDomains = _domainTypeRefDataService.GetAllAndNotVisibleForCustomer(customerId);
                vm.DomainTypes.AddRange(serviceDomains.Select(s => new SelectListItem { Text = s.DomainName, Value = s.Id.ToString(CultureInfo.InvariantCulture) }).ToList());

            }

            return View("Add" + level, vm);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Edit(string level, int id)
        {
            var customerId = _appUserContext.Current.CurrentCustomer.Id;

            var customerDomainCount = _serviceDomainService
                .CustomerServiceDomains(customerId).Count();

            var serviceDomain = _serviceDomainService.GetById(id);

            var vm = new EditServiceDomainViewModel
            {
                ServiceDomain = Mapper.Map<ServiceDomainViewModel>(serviceDomain),
                EditLevel = level,
                CanMoveServiceFunction = customerDomainCount > 1,
            };

            return View("Edit" + level, vm);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditServiceDomainViewModel model)
        {
            if (_appUserContext.Current.CurrentCustomer == null ||
                _appUserContext.Current.CurrentCustomer.Id == 0)
            {
                return GetRedirect(model.EditLevel);
            }

            if (_contextManager.RequestManager.Form[FormValuesNames.Return] != null)
            {
                return GetRedirect(model.EditLevel, model.ServiceDomain.ServiceDeskId);
            }

            if (ModelState.IsValid)
            {
                var serviceDomain = _serviceDomainService.GetById(model.ServiceDomain.Id);
                if (serviceDomain == null)
                {
                    return GetRedirect(model.EditLevel, model.ServiceDomain.ServiceDeskId);
                }

                var now = DateTime.Now;

                serviceDomain.DomainTypeId = model.ServiceDomain.DomainTypeId;
                serviceDomain.AlternativeName = model.ServiceDomain.AlternativeName;
                serviceDomain.DiagramOrder = model.ServiceDomain.DiagramOrder ?? 5;
                serviceDomain.UpdatedBy = _contextManager.UserManager.Name;
                serviceDomain.UpdatedDate = now;

                _serviceDomainService.Update(Mapper.Map<ServiceDomain>(serviceDomain));

                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                var domainType = _domainTypeRefDataService.GetById(serviceDomain.DomainTypeId);
                if (!domainType.Visible && _domainTypeRefDataService.GetNumberOfDomainTypeReferences(serviceDomain.DomainTypeId) >= customerSpecificTypeThreshold)
                {
                    domainType.Visible = true;
                    _domainTypeRefDataService.Update(domainType);
                }


                return GetRedirect(model.EditLevel, model.ServiceDomain.ServiceDeskId);
            }

            return View("Edit" + model.EditLevel, model);
        }

        private ActionResult GetRedirect(string level, int serviceDeskId = 0)
        {
            return serviceDeskId > 0 ? RedirectToAction("Edit", "ServiceDesk", new { Level = level, Id = serviceDeskId }) : RedirectToAction("Index", "ServiceDomain", new { Level = level });
        }

        [HttpPost]
        public ActionResult ReadAjaxServiceDomainGrid([DataSourceRequest] DataSourceRequest request, int? serviceDeskId)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current.CurrentCustomer != null &&
                    _appUserContext.Current.CurrentCustomer.Id > 0 &&
                    serviceDeskId != null &&
                    serviceDeskId > 0)
                {
                    result = _serviceDomainService
                        .ServiceDeskDomains(serviceDeskId.Value)
                        .ToDataSourceResult(request, Mapper.Map<ServiceDomainListItem, ServiceDomainViewModel>);
                }
                else
                {
                    result = _serviceDomainService
                        .CustomerServiceDomains(_appUserContext.Current.CurrentCustomer.Id)
                        .ToDataSourceResult(request, Mapper.Map<ServiceDomainListItem, ServiceDomainViewModel>);
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
        public ActionResult UpdateAjaxServiceDomainGrid([DataSourceRequest]DataSourceRequest request, ServiceDomainViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = _contextManager.UserManager.Name;
                    var dateTimeNow = DateTime.Now;

                    // Update
                    var serviceDomain = _serviceDomainService.GetById(model.Id);

                    // Check that the domain is from the current customer.
                    if (serviceDomain == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", WebResources.ServiceDomainCannotBeFound);
                    }
                    else
                    {
                        serviceDomain.AlternativeName = model.AlternativeName;
                        serviceDomain.DomainTypeId = model.DomainTypeId;
                        serviceDomain.DiagramOrder = model.DiagramOrder ?? 5;
                        serviceDomain.UpdatedDate = dateTimeNow;
                        serviceDomain.UpdatedBy = userName;
                        _serviceDomainService.Update(serviceDomain);
                    }
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
        public ActionResult DeleteAjaxServiceDomainGrid([DataSourceRequest]DataSourceRequest request, ServiceDomainViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDomain = _serviceDomainService.GetById(model.Id);

                    if (serviceDomain == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ServiceDomainDeleteEntityDoesNotExist, WebResources.ServiceDomainCannotFindDomainToBeDeleted);
                    }
                    else if (serviceDomain.ServiceFunctions != null && serviceDomain.ServiceFunctions.Any())
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ErrorMessage,
                            WebResources.ServiceDomainCannotBeDeletedDueToFunctionsExisting);
                    }
                    else
                    {
                        _serviceDomainService.Delete(serviceDomain);
                    }
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
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteServiceDomain(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // RetryableOperation to Log any Unexpected Errors and Alert Error Instance id.
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        var serviceDomain = _serviceDomainService.GetById(id);
                        _serviceDomainService.Delete(serviceDomain);
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

        [HttpGet]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Move()
        {
            var vm = new MoveServiceDomainViewModel();
            if (_appUserContext.Current.CurrentCustomer != null)
            {
                var serviceDesks = _serviceDeskService
                    .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .ToList();
                vm.ServiceDesks.AddRange(serviceDesks
                    .Select(s => new KeyValuePair<int, string>(s.Id, s.DeskName)));
            }
            return PartialView("_Move", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public JsonResult Move(MoveServiceDomainViewModel model)
        {
            var result = new { Success = "True", Message = "Success" };

            if (ModelState.IsValid)
            {
                try
                {
                    var userName = _contextManager.UserManager.Name;
                    var dateTimeNow = DateTime.Now;
                    var customerId = _appUserContext.Current.CurrentCustomer.Id;

                    var serviceDomain = _serviceDomainService.GetByCustomerAndId(customerId, model.ServiceDomainId);
                    if (serviceDomain != null)
                    {
                        serviceDomain.UpdatedBy = userName;
                        serviceDomain.UpdatedDate = dateTimeNow;
                        serviceDomain.ServiceDeskId = model.ServiceDeskId;

                        _serviceDomainService.Update(serviceDomain);
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                    result = new { Success = "False", ex.Message };
                }
            }
            else
            {
                var errors = ModelState.GetModelStateMesssages();
                result = new { Success = "False", Message = errors.First() };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxServiceAddDomainGrid([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<BulkServiceDomainViewModel> serviceDomains)
        {
            var inputDomains = serviceDomains as IList<BulkServiceDomainViewModel> ?? serviceDomains.ToList();
            var outputDomains = new List<BulkServiceDomainViewModel>();
            if (ModelState.IsValid)
            {
                try
                {
                    var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);

                    if (serviceDomains != null && inputDomains.Any())
                    {
                        var userName = _contextManager.UserManager.Name;
                        var now = DateTime.Now;
                        var customerId = _appUserContext.Current.CurrentCustomer.Id;

                        // Make sure all the of the specified service desks exist.
                        var checkServiceDeskIds = inputDomains
                            .Where(w => w.ServiceDeskId.HasValue)
                            .Select(s => s.ServiceDeskId.Value)
                            .Distinct()
                            .ToList();
                        var serviceDeskIds = _serviceDeskService.GetByCustomer(customerId)
                            .Where(w => checkServiceDeskIds.Contains(w.Id))
                            .Select(s => s.Id)
                            .ToList();

                        foreach (var inputDomain in inputDomains)
                        {
                            if (!inputDomain.ServiceDeskId.HasValue ||
                                inputDomain.ServiceDeskId == 0 ||
                                inputDomain.DomainTypeId == 0 ||
                                !serviceDeskIds.Contains(inputDomain.ServiceDeskId.Value))
                            {
                                continue;
                            }

                            var diagramDisplayOrder = inputDomain.DiagramOrder ?? 5;
                            if (diagramDisplayOrder < 1)
                            {
                                diagramDisplayOrder = 5;
                            }
                            var newServiceDomain = new ServiceDomain
                            {
                                AlternativeName = inputDomain.AlternativeName,
                                DiagramOrder = diagramDisplayOrder,
                                DomainTypeId = inputDomain.DomainTypeId,
                                ServiceDeskId = inputDomain.ServiceDeskId.Value,
                                InsertedBy = userName,
                                InsertedDate = now,
                                UpdatedBy = userName,
                                UpdatedDate = now
                            };
                            newServiceDomain.Id = _serviceDomainService.Create(newServiceDomain);
                            outputDomains.Add(Mapper.Map<BulkServiceDomainViewModel>(newServiceDomain));

                            var domainType = _domainTypeRefDataService.GetById(inputDomain.DomainTypeId);
                            if (!domainType.Visible && _domainTypeRefDataService.GetNumberOfDomainTypeReferences(inputDomain.DomainTypeId) >= customerSpecificTypeThreshold)
                            {
                                domainType.Visible = true;
                                _domainTypeRefDataService.Update(domainType);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                }
            }

            return Json(outputDomains.ToDataSourceResult(request, ModelState));
        }

        #region Add Service Domain Grid - Not used for any function, but required by Inline edit to work.

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxServiceAddDomainGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
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
        public ActionResult UpdateAjaxServiceAddDomainGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
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
        public ActionResult DestroyAjaxServiceAddDomainGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        #endregion


    }
}