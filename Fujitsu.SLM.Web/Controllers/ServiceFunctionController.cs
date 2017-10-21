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
    public class ServiceFunctionController : Controller
    {
        private readonly IServiceDomainService _serviceDomainService;
        private readonly IServiceFunctionService _serviceFunctionService;
        private readonly IFunctionTypeRefDataService _functionTypeRefDataService;
        private readonly IParameterService _parameterService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public ServiceFunctionController(IServiceDomainService serviceDomainService,
            IServiceFunctionService serviceFunctionService,
            IFunctionTypeRefDataService functionTypeRefDataService,
            IParameterService parameterService,
            IContextManager contextManager,
            IAppUserContext appUserContext)
        {
            if (serviceDomainService == null)
            {
                throw new ArgumentNullException(nameof(serviceDomainService));
            }

            if (serviceFunctionService == null)
            {
                throw new ArgumentNullException(nameof(serviceFunctionService));
            }

            if (functionTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(functionTypeRefDataService));
            }

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
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
            _serviceFunctionService = serviceFunctionService;
            _functionTypeRefDataService = functionTypeRefDataService;
            _parameterService = parameterService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;

        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            var serviceDomainCount = _serviceDomainService
                .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .Select(s => s.Id)
                .Count();

            var vm = new ViewServiceFunctionViewModel()
            {
                EditLevel = level,
                CanMoveServiceFunction = serviceDomainCount > 1
            };

            return View(level, vm);
        }

        [HttpPost]
        public ActionResult ReadAjaxServiceFunctionGrid([DataSourceRequest] DataSourceRequest request,
            int? serviceDomainId)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current.CurrentCustomer != null &&
                    _appUserContext.Current.CurrentCustomer.Id > 0 &&
                    serviceDomainId != null &&
                    serviceDomainId > 0)
                {
                    result = _serviceFunctionService
                        .ServiceDomainFunctions(serviceDomainId.Value)
                        .ToDataSourceResult(request, Mapper.Map<ServiceFunctionListItem, ServiceFunctionViewModel>);
                }
                else
                {
                    result = _serviceFunctionService
                        .CustomerServiceFunctions(_appUserContext.Current.CurrentCustomer.Id)
                        .ToDataSourceResult(request, Mapper.Map<ServiceFunctionListItem, ServiceFunctionViewModel>);
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
        public ActionResult UpdateAjaxServiceFunctionGrid([DataSourceRequest] DataSourceRequest request, ServiceFunctionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = _contextManager.UserManager.Name;
                    var dateTimeNow = DateTime.Now;

                    // Update
                    var serviceFunction = _serviceFunctionService.GetById(model.Id);

                    // Check that the domain is from the current customer.
                    if (serviceFunction == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                            WebResources.ServiceFunctionCannotBeFound);
                    }
                    else
                    {
                        serviceFunction.AlternativeName = model.AlternativeName;
                        serviceFunction.DiagramOrder = model.DiagramOrder ?? 5;
                        serviceFunction.FunctionTypeId = model.FunctionTypeId;
                        serviceFunction.UpdatedDate = dateTimeNow;
                        serviceFunction.UpdatedBy = userName;
                        _serviceFunctionService.Update(serviceFunction);
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
        public ActionResult DeleteAjaxServiceFunctionGrid([DataSourceRequest] DataSourceRequest request,
            ServiceFunctionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceFunction = _serviceFunctionService.GetById(model.Id);

                    if (serviceFunction == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ServiceFunctionDeleteEntityDoesNotExist,
                            WebResources.ServiceFunctionCannotFindFunctionToBeDeleted);
                    }
                    else if (serviceFunction.ServiceComponents.Any())
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ErrorMessage,
                            WebResources.ServiceFunctionCannotBeDeletedDueToComponentsExisitng);
                    }
                    else
                    {
                        _serviceFunctionService.Delete(serviceFunction);
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
        public ActionResult DeleteServiceFunction(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // RetryableOperation to Log any Unexpected Errors and Alert Error Instance id.
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        var serviceFunction = _serviceFunctionService.GetById(id);
                        _serviceFunctionService.Delete(serviceFunction);
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
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Edit(string level, int id)
        {
            var customerId = _appUserContext.Current.CurrentCustomer.Id;

            var customerFunctionCount = _serviceFunctionService
                .CustomerServiceFunctions(customerId).Count();

            var serviceFunction = _serviceFunctionService.GetById(id);

            var vm = new EditServiceFunctionViewModel
            {
                ServiceFunction = Mapper.Map<ServiceFunctionViewModel>(serviceFunction),
                EditLevel = level,
                CanMoveServiceComponent = customerFunctionCount > 1,
            };

            return View("Edit" + level, vm);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditServiceFunctionViewModel model)
        {
            if (_appUserContext.Current.CurrentCustomer == null ||
                _appUserContext.Current.CurrentCustomer.Id == 0)
            {
                return GetRedirect(model.EditLevel);
            }

            if (_contextManager.RequestManager.Form[FormValuesNames.Return] != null)
            {
                return GetRedirect(model.EditLevel, model.ServiceFunction.ServiceDomainId);
            }

            if (ModelState.IsValid)
            {
                var serviceFunction = _serviceFunctionService.GetById(model.ServiceFunction.Id);
                if (serviceFunction == null)
                {
                    return GetRedirect(model.EditLevel, model.ServiceFunction.ServiceDomainId);
                }

                var now = DateTime.Now;

                serviceFunction.FunctionTypeId = model.ServiceFunction.FunctionTypeId;
                serviceFunction.AlternativeName = model.ServiceFunction.AlternativeName;
                serviceFunction.DiagramOrder = model.ServiceFunction.DiagramOrder ?? 5;
                serviceFunction.UpdatedBy = _contextManager.UserManager.Name;
                serviceFunction.UpdatedDate = now;

                _serviceFunctionService.Update(Mapper.Map<ServiceFunction>(serviceFunction));

                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                var functionType = _functionTypeRefDataService.GetById(serviceFunction.FunctionTypeId);
                if (!functionType.Visible && _functionTypeRefDataService.GetNumberOfFunctionTypeReferences(serviceFunction.FunctionTypeId) >= customerSpecificTypeThreshold)
                {
                    functionType.Visible = true;
                    _functionTypeRefDataService.Update(functionType);
                }


                return GetRedirect(model.EditLevel, serviceFunction.ServiceDomainId);
            }

            return View("Edit" + model.EditLevel, model);
        }

        private ActionResult GetRedirect(string level, int serviceDomainId = 0)
        {
            if (string.IsNullOrEmpty(level))
            {
                return RedirectToAction("Edit", "ServiceDomain", new { Level = level, Id = serviceDomainId });
            }

            return serviceDomainId > 0 ? RedirectToAction("Edit", "ServiceDomain", new { Level = level, Id = serviceDomainId }) : RedirectToAction("Index", "ServiceFunction", new { Level = level });
        }


        [HttpGet]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Move()
        {
            var vm = new MoveServiceFunctionViewModel();
            if (_appUserContext.Current.CurrentCustomer != null)
            {
                var serviceDomains = _serviceDomainService
                    .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                    .ToList();

                vm.ServiceDomains.AddRange(serviceDomains
                    .Select(s => new KeyValuePair<int, string>(s.Id, s.ServiceDesk.DeskName + UnicodeCharacters.DoubleArrowRight + (string.IsNullOrEmpty(s.AlternativeName) ? s.DomainType.DomainName : s.AlternativeName))));
            }
            return PartialView("_Move", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public JsonResult Move(MoveServiceFunctionViewModel model)
        {
            var result = new { Success = "True", Message = "Success" };

            if (ModelState.IsValid)
            {
                try
                {
                    var userName = _contextManager.UserManager.Name;
                    var dateTimeNow = DateTime.Now;

                    var serviceFunction = _serviceFunctionService.GetById(model.ServiceFunctionId);
                    if (serviceFunction != null)
                    {
                        serviceFunction.UpdatedBy = userName;
                        serviceFunction.UpdatedDate = dateTimeNow;
                        serviceFunction.ServiceDomainId = model.ServiceDomainId;

                        _serviceFunctionService.Update(serviceFunction);
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

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Add(string level, int id)
        {
            var vm = new AddServiceFunctionViewModel
            {
                ServiceDomainId = id,
                EditLevel = level,
                HasServiceDomainContext = id != 0
            };

            vm.ReturnUrl = vm.HasServiceDomainContext
                ? Url.Action("Edit", "ServiceDomain", new { level = vm.EditLevel, id = vm.ServiceDomainId })
                : Url.Action("Index", "ServiceFunction", new { level = vm.EditLevel, id = vm.ServiceDomainId });

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var customerId = _appUserContext.Current.CurrentCustomer.Id;
                var serviceDomains = _serviceDomainService.GetByCustomer(customerId).ToList();

                if (id > 0)
                {
                    vm.ServiceDomainName =
                        serviceDomains.First(x => x.Id == id).DomainType.DomainName;
                }
                else if (serviceDomains.Count == 1)
                {
                    vm.ServiceDomainId = serviceDomains.First().Id;
                    vm.ServiceDomainName = serviceDomains.First().DomainType.DomainName;
                    vm.HasServiceDomainContext = true;
                }
                else
                {
                    vm.ServiceDomains.AddRange(
                            serviceDomains.Select(
                                s =>
                                    new SelectListItem
                                    {
                                        Text =
                                            s.ServiceDesk.DeskName + UnicodeCharacters.DoubleArrowRight +
                                            (string.IsNullOrEmpty(s.AlternativeName)
                                                ? s.DomainType.DomainName
                                                : s.AlternativeName),
                                        Value = s.Id.ToString(CultureInfo.InvariantCulture)
                                    }).ToList());
                }
                var functionTypes = _functionTypeRefDataService.GetAllAndNotVisibleForCustomer(customerId);
                vm.FunctionTypes.AddRange(functionTypes.Select(s => new SelectListItem { Text = s.FunctionName, Value = s.Id.ToString(CultureInfo.InvariantCulture) }).ToList());
            }

            return View("Add" + level, vm);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxServiceAddFunctionGrid([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<BulkServiceFunctionViewModel> serviceFunctions)
        {
            var inputFunctions = serviceFunctions as IList<BulkServiceFunctionViewModel> ?? serviceFunctions.ToList();
            var outputFunctions = new List<BulkServiceFunctionViewModel>();
            if (ModelState.IsValid)
            {
                try
                {
                    var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);

                    if (serviceFunctions != null && inputFunctions.Any())
                    {
                        var userName = _contextManager.UserManager.Name;
                        var now = DateTime.Now;
                        var customerId = _appUserContext.Current.CurrentCustomer.Id;

                        // Make sure all the of the specified Service Domains exist.
                        var checkServiceDomainIds = inputFunctions
                            .Where(w => w.ServiceDomainId.HasValue)
                            .Select(s => s.ServiceDomainId.Value)
                            .Distinct()
                            .ToList();
                        var serviceDomainIds = _serviceDomainService.GetByCustomer(customerId)
                            .Where(w => checkServiceDomainIds.Contains(w.Id))
                            .Select(s => s.Id)
                            .ToList();

                        foreach (var inputFunction in inputFunctions)
                        {
                            if (!inputFunction.ServiceDomainId.HasValue ||
                                inputFunction.ServiceDomainId == 0 ||
                                inputFunction.FunctionTypeId == 0 ||
                                !serviceDomainIds.Contains(inputFunction.ServiceDomainId.Value))
                            {
                                continue;
                            }

                            var diagramDisplayOrder = inputFunction.DiagramOrder ?? 5;
                            if (diagramDisplayOrder < 1)
                            {
                                diagramDisplayOrder = 5;
                            }

                            var newServiceFunction = new ServiceFunction
                            {
                                AlternativeName = inputFunction.AlternativeName,
                                DiagramOrder = diagramDisplayOrder,
                                FunctionTypeId = inputFunction.FunctionTypeId,
                                ServiceDomainId = inputFunction.ServiceDomainId.Value,
                                InsertedBy = userName,
                                InsertedDate = now,
                                UpdatedBy = userName,
                                UpdatedDate = now
                            };

                            newServiceFunction.Id = _serviceFunctionService.Create(newServiceFunction);
                            outputFunctions.Add(Mapper.Map<BulkServiceFunctionViewModel>(newServiceFunction));

                            var functionType = _functionTypeRefDataService.GetById(inputFunction.FunctionTypeId);
                            if (!functionType.Visible && _functionTypeRefDataService.GetNumberOfFunctionTypeReferences(inputFunction.FunctionTypeId) >= customerSpecificTypeThreshold)
                            {
                                functionType.Visible = true;
                                _functionTypeRefDataService.Update(functionType);
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

            return Json(outputFunctions.ToDataSourceResult(request, ModelState));
        }


        #region Add Service Function Grid - Not used for any function, but required by Inline edit to work.

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxServiceAddFunctionGrid([DataSourceRequest] DataSourceRequest request)
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
        public ActionResult UpdateAjaxServiceAddFunctionGrid([DataSourceRequest] DataSourceRequest request)
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
        public ActionResult DestroyAjaxServiceAddFunctionGrid([DataSourceRequest] DataSourceRequest request)
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