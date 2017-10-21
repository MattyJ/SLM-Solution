using AutoMapper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Enumerations;
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
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    [OutputCache(Duration = 0)]
    public class ReferenceDataController : Controller
    {
        private readonly IDomainTypeRefDataService _domainTypeRefDataService;
        private readonly IFunctionTypeRefDataService _functionTypeRefDataService;
        private readonly IInputTypeRefDataService _inputTypeRefDataService;
        private readonly IServiceDeliveryOrganisationTypeRefDataService _serviceDeliveryOrganisationTypeRefDataService;
        private readonly IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitTypeRefDataService;
        private readonly IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private readonly IOperationalProcessTypeRefDataService _operationalProcessTypeRefDataService;
        private readonly IRegionTypeRefDataService _regionTypeRefDataService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;
        private readonly ILoggingManager _loggingManager;


        public ReferenceDataController(IDomainTypeRefDataService domainTypeRefDataService,
            IFunctionTypeRefDataService functionTypeRefDataService,
            IInputTypeRefDataService inputTypeRefDataService,
            IServiceDeliveryOrganisationTypeRefDataService serviceDeliveryOrganisationTypeRefDataService,
            IServiceDeliveryUnitTypeRefDataService serviceDeliveryUnitTypeRefDataService,
            IResolverGroupTypeRefDataService resolverGroupTypeRefDataService,
            IOperationalProcessTypeRefDataService operationalProcessTypeRefDataService,
            IRegionTypeRefDataService regionTypeRefDataService,
            IContextManager contextManager,
            IAppUserContext appUserContext,
            ILoggingManager loggingManager)
        {
            if (domainTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(domainTypeRefDataService));
            }

            if (functionTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(functionTypeRefDataService));
            }

            if (inputTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(inputTypeRefDataService));
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

            if (operationalProcessTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(operationalProcessTypeRefDataService));
            }

            if (regionTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(regionTypeRefDataService));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            if (loggingManager == null)
            {
                throw new ArgumentNullException(nameof(loggingManager));
            }

            _domainTypeRefDataService = domainTypeRefDataService;
            _functionTypeRefDataService = functionTypeRefDataService;
            _inputTypeRefDataService = inputTypeRefDataService;
            _serviceDeliveryOrganisationTypeRefDataService = serviceDeliveryOrganisationTypeRefDataService;
            _serviceDeliveryUnitTypeRefDataService = serviceDeliveryUnitTypeRefDataService;
            _resolverGroupTypeRefDataService = resolverGroupTypeRefDataService;
            _operationalProcessTypeRefDataService = operationalProcessTypeRefDataService;
            _regionTypeRefDataService = regionTypeRefDataService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;
            _loggingManager = loggingManager;
        }

        // GET: ReferenceData
        public ActionResult Index()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            // Land On Input Type Reference Data
            return RedirectToAction("InputTypes");
        }

        #region Domain Type Ref Data

        public ActionResult Domains()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadAjaxDomainRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _domainTypeRefDataService.GetDomainTypeRefData(_contextManager.UserManager.IsSLMAdministrator(), _contextManager.UserManager.IsSLMArchitect() ? _contextManager.UserManager.Name : null)
                    .ToDataSourceResult(request, Mapper.Map<DomainTypeRefDataListItem, DomainTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxDomainRefDataGrid([DataSourceRequest]DataSourceRequest request, DomainTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_domainTypeRefDataService.All().Any(c => c.DomainName.ToLower() == model.DomainName.Trim().ToLower()))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.EntityNameIsNotUnique, "Domain Type"));
                    }
                    else
                    {
                        _domainTypeRefDataService.Create(Mapper.Map<DomainTypeRefDataViewModel, DomainTypeRefData>(model));
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
        public ActionResult UpdateAjaxDomainRefDataGrid([DataSourceRequest]DataSourceRequest request, DomainTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update
                    var domain = _domainTypeRefDataService.GetById(model.Id);
                    domain.DomainName = model.DomainName;
                    domain.Visible = model.Visible;
                    domain.SortOrder = model.SortOrder;
                    _domainTypeRefDataService.Update(domain);
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
        public ActionResult DeleteAjaxDomainRefDataGrid([DataSourceRequest]DataSourceRequest request, DomainTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var domain = _domainTypeRefDataService.GetById(model.Id);

                    if (domain == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Domain Type"));
                    }
                    else
                    {
                        if (_domainTypeRefDataService.GetNumberOfDomainTypeReferences(domain.Id) > 0)
                        {
                            _contextManager.ResponseManager.StatusCode = 500;
                            _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Domain Type"));

                        }
                        else
                        {
                            _domainTypeRefDataService.Delete(domain);
                        }
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

        public JsonResult GetDomainTypes()
        {
            var domains = _domainTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.DomainName).ToList();

            domains.Add(new DomainTypeRefData
            {
                Id = 0,
                DomainName = WebResources.DefaultDropDownListText,
                SortOrder = 0,
            });

            return Json(domains, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceDeliveryUnitTypes()
        {
            var r = _serviceDeliveryUnitTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.ServiceDeliveryUnitTypeName).ToList();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAndNotVisibleDomainTypesForCustomer()
        {
            var list = new List<DomainTypeRefData>();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                list.AddRange(_domainTypeRefDataService.GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateDomainType()
        {
            var vm = new DomainTypeRefDataViewModel { SortOrder = 5, Visible = false };
            return PartialView("_DomainType", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateDomainType(DomainTypeRefDataViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var domainType = _domainTypeRefDataService.All().FirstOrDefault(c => c.DomainName.ToLower() == model.DomainName.Trim().ToLower());
                    model.Id = domainType?.Id ?? _domainTypeRefDataService.Create(Mapper.Map<DomainTypeRefDataViewModel, DomainTypeRefData>(model));
                }
                else
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = ModelState.GetModelStateMesssages().FirstOrDefault() });
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                _loggingManager.Write(LoggingEventSource.WebUi, LoggingEventType.Error, ex.Message);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }

            // The model is required to set the appropiate UI field
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Function Type Ref Data

        public ActionResult Functions()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadAjaxFunctionRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _functionTypeRefDataService.GetFunctionTypeRefData(_contextManager.UserManager.IsSLMAdministrator(), _contextManager.UserManager.IsSLMArchitect() ? _contextManager.UserManager.Name : null).ToDataSourceResult(request, Mapper.Map<FunctionTypeRefDataListItem, FunctionTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxFunctionRefDataGrid([DataSourceRequest]DataSourceRequest request, FunctionTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_functionTypeRefDataService.All().Any(c => c.FunctionName.ToLower() == model.FunctionName.Trim().ToLower()))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                            string.Format(WebResources.EntityNameIsNotUnique, "Function Type"));
                    }
                    else
                    {
                        _functionTypeRefDataService.Create(Mapper.Map<FunctionTypeRefDataViewModel, FunctionTypeRefData>(model));
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
        public ActionResult UpdateAjaxFunctionRefDataGrid([DataSourceRequest]DataSourceRequest request, FunctionTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var function = _functionTypeRefDataService.GetById(model.Id);
                    function.FunctionName = model.FunctionName;
                    function.Visible = model.Visible;
                    function.SortOrder = model.SortOrder;
                    _functionTypeRefDataService.Update(function);
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
        public ActionResult DeleteAjaxFunctionRefDataGrid([DataSourceRequest]DataSourceRequest request, FunctionTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var function = _functionTypeRefDataService.GetById(model.Id);

                    if (function == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Function Type"));
                    }

                    else
                    {
                        if (_functionTypeRefDataService.GetNumberOfFunctionTypeReferences(function.Id) > 0)
                        {
                            _contextManager.ResponseManager.StatusCode = 500;
                            _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Function Type"));
                        }
                        else
                        {
                            _functionTypeRefDataService.Delete(function);
                        }
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

        public JsonResult GetFunctionTypes()
        {
            var functions = _functionTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.FunctionName).ToList();

            functions.Add(new FunctionTypeRefData
            {
                Id = 0,
                FunctionName = WebResources.DefaultDropDownListText,
                SortOrder = 0,
            });

            return Json(functions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateFunctionType()
        {
            var vm = new FunctionTypeRefDataViewModel { SortOrder = 5, Visible = false };
            return PartialView("_FunctionType", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateFunctionType(FunctionTypeRefDataViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var functionType = _functionTypeRefDataService.All().FirstOrDefault(f => f.FunctionName.ToLower() == model.FunctionName.Trim().ToLower());
                    model.Id = functionType?.Id ?? _functionTypeRefDataService.Create(Mapper.Map<FunctionTypeRefDataViewModel, FunctionTypeRefData>(model));
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                _loggingManager.Write(LoggingEventSource.WebUi, LoggingEventType.Error, ex.Message);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }

            // The model is required to set the appropiate UI field
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAndNotVisibleFunctionTypesForCustomer()
        {
            var list = new List<FunctionTypeRefData>();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                list.AddRange(_functionTypeRefDataService.GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Input Type Ref Data

        public ActionResult InputTypes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadAjaxInputTypeRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _inputTypeRefDataService.GetInputTypeRefDataWithUsageStats().ToDataSourceResult(request, Mapper.Map<InputTypeRefDataListItem, InputTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxInputTypeRefDataGrid([DataSourceRequest]DataSourceRequest request, InputTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_inputTypeRefDataService.All().Any(c => c.InputTypeName == model.InputTypeName))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                            string.Format(WebResources.EntityNameIsNotUnique, "Input Type"));
                    }
                    else
                    {
                        // Insert
                        var inputType = Mapper.Map<InputTypeRefDataViewModel, InputTypeRefData>(model);
                        _inputTypeRefDataService.Create(inputType);
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
        public ActionResult UpdateAjaxInputTypeRefDataGrid([DataSourceRequest]DataSourceRequest request, InputTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update
                    var inputType = _inputTypeRefDataService.GetById(model.Id);
                    inputType.InputTypeNumber = model.InputTypeNumber;
                    inputType.InputTypeName = model.InputTypeName;
                    inputType.SortOrder = model.SortOrder;
                    inputType.Default = model.Default;
                    _inputTypeRefDataService.Update(inputType);
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
        public ActionResult DeleteAjaxInputTypeRefDataGrid([DataSourceRequest]DataSourceRequest request, InputTypeRefDataViewModel model)
        {
            //DataSourceResult result = null;
            //result = _inputTypeRefDataService.All().ToDataSourceResult(request, Mapper.Map<InputTypeRefData, InputTypeRefDataViewModel>);

            if (ModelState.IsValid)
            {
                try
                {
                    var inputType = _inputTypeRefDataService.GetById(model.Id);

                    if (inputType == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                            string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Input Type"));
                    }
                    else
                    {
                        if (_inputTypeRefDataService.IsInputTypeReferenced(inputType.Id))
                        {
                            _contextManager.ResponseManager.StatusCode = 500;
                            _contextManager.ResponseManager.AppendHeader("HandledError",
                                string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Input Type"));

                        }
                        else
                        {
                            _inputTypeRefDataService.Delete(inputType);
                        }
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

        public JsonResult GetInputTypes()
        {
            var inputTypes = _inputTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.InputTypeNumber).ThenBy(o => o.InputTypeName).ToList();

            return Json(inputTypes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateInputType()
        {
            var model = new InputTypeRefDataViewModel
            {
                InputTypeNumber = _inputTypeRefDataService.All().Max(x => x.InputTypeNumber) + 1,
                InputTypeName = string.Empty,
                SortOrder = 5,
                Default = false
            };
            return PartialView("_InputType", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateInputType(InputTypeRefDataViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_inputTypeRefDataService.All().Any(c => c.InputTypeName == model.InputTypeName))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;  // Triggers the $.ajax error
                        ModelState.AddModelError("HandledError", string.Format(WebResources.EntityNameIsNotUnique, "Input Type"));
                        return Json(new DataSourceResult { Errors = ModelState.GetModelStateMesssages().FirstOrDefault() });
                    }

                    // Insert
                    var inputType = Mapper.Map<InputTypeRefDataViewModel, InputTypeRefData>(model);
                    model.Id = _inputTypeRefDataService.Create(inputType);
                }
                else
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = ModelState.GetModelStateMesssages().FirstOrDefault() });
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                _loggingManager.Write(LoggingEventSource.WebUi, LoggingEventType.Error, ex.Message);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }

            // The model is required to set the appropiate UI field
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Service Delivery Organisation Type Ref Data

        public ActionResult ServiceDeliveryOrganisationTypes()
        {
            return View();
        }

        [OutputCache(Duration = 0)]
        public JsonResult GetServiceDeliveryOrganisationTypes()
        {
            var r = _serviceDeliveryOrganisationTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.ServiceDeliveryOrganisationTypeName).ToList();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _serviceDeliveryOrganisationTypeRefDataService
                    .All()
                    .ToDataSourceResult(request, Mapper.Map<ServiceDeliveryOrganisationTypeRefData, ServiceDeliveryOrganisationTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ServiceDeliveryOrganisationTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_serviceDeliveryOrganisationTypeRefDataService.All().Any(c => c.ServiceDeliveryOrganisationTypeName == model.ServiceDeliveryOrganisationTypeName))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                            string.Format(WebResources.EntityNameIsNotUnique, "Service Delivery Organisation Type"));
                    }
                    else
                    {
                        // Insert
                        var entity = Mapper.Map<ServiceDeliveryOrganisationTypeRefDataViewModel, ServiceDeliveryOrganisationTypeRefData>(model);
                        entity.Id = 0;
                        _serviceDeliveryOrganisationTypeRefDataService.Create(entity);
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
        public ActionResult UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ServiceDeliveryOrganisationTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update
                    var entity = _serviceDeliveryOrganisationTypeRefDataService.GetById(model.Id);
                    Mapper.Map(model, entity);
                    _serviceDeliveryOrganisationTypeRefDataService.Update(entity);
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
        public ActionResult DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ServiceDeliveryOrganisationTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _serviceDeliveryOrganisationTypeRefDataService.GetById(model.Id);

                    if (entity == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted,
                            string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Service Delivery Organisation Type"));
                    }

                    else
                    {
                        if (_serviceDeliveryOrganisationTypeRefDataService.IsServiceDeliveryOrganisationTypeReferenced(entity.Id))
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ReferenceDataItemIsStillUtilised,
                                string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Service Delivery Organisation Type"));
                        }
                        else
                        {
                            _serviceDeliveryOrganisationTypeRefDataService.Delete(entity);
                        }
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

        #endregion

        #region Service Delivery Unit Type Ref Data

        public ActionResult ServiceDeliveryUnitTypes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadAjaxServiceDeliveryUnitTypesRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _serviceDeliveryUnitTypeRefDataService
                    .GetServiceDeliveryUnitTypeRefDataWithUsageStats()
                    .ToDataSourceResult(request, Mapper.Map<ServiceDeliveryUnitTypeRefDataListItem, ServiceDeliveryUnitTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxServiceDeliveryUnitTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ServiceDeliveryUnitTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_serviceDeliveryUnitTypeRefDataService.All().Any(c => c.ServiceDeliveryUnitTypeName == model.ServiceDeliveryUnitTypeName))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                            string.Format(WebResources.EntityNameIsNotUnique, "Service Delivery Unit Type"));
                    }
                    else
                    {
                        // Insert
                        var sdu = Mapper.Map<ServiceDeliveryUnitTypeRefDataViewModel, ServiceDeliveryUnitTypeRefData>(model);
                        sdu.Id = 0;
                        _serviceDeliveryUnitTypeRefDataService.Create(sdu);
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
        public ActionResult UpdateAjaxServiceDeliveryUnitTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ServiceDeliveryUnitTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update
                    var sdu = _serviceDeliveryUnitTypeRefDataService.GetById(model.Id);
                    Mapper.Map(model, sdu);
                    _serviceDeliveryUnitTypeRefDataService.Update(sdu);
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
        public ActionResult DeleteAjaxServiceDeliveryUnitTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ServiceDeliveryUnitTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sdu = _serviceDeliveryUnitTypeRefDataService.GetById(model.Id);

                    if (sdu == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted,
                            string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Service Delivery Unit Type"));
                    }

                    else
                    {
                        if (_serviceDeliveryUnitTypeRefDataService.IsServiceDeliveryUnitTypeReferenced(sdu.Id))
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ReferenceDataItemIsStillUtilised,
                                string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Service Delivery Unit Type"));
                        }
                        else
                        {
                            _serviceDeliveryUnitTypeRefDataService.Delete(sdu);
                        }
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

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateServiceDeliveryUnitType()
        {
            var vm = new ServiceDeliveryUnitTypeRefDataViewModel { SortOrder = 5, Visible = false };
            return PartialView("_ServiceDeliveryUnitType", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateServiceDeliveryUnitType(ServiceDeliveryUnitTypeRefDataViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceDeliveryUnitType = _serviceDeliveryUnitTypeRefDataService.All().FirstOrDefault(c => c.ServiceDeliveryUnitTypeName.ToLower() == model.ServiceDeliveryUnitTypeName.Trim().ToLower());
                    model.Id = serviceDeliveryUnitType?.Id ?? _serviceDeliveryUnitTypeRefDataService.Create(Mapper.Map<ServiceDeliveryUnitTypeRefDataViewModel, ServiceDeliveryUnitTypeRefData>(model));
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                _loggingManager.Write(LoggingEventSource.WebUi, LoggingEventType.Error, ex.Message);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }

            // The model is required to set the appropiate UI field
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer()
        {
            var list = new List<ServiceDeliveryUnitTypeRefData>();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                list.AddRange(_serviceDeliveryUnitTypeRefDataService.GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Resolver Group Type Ref Data

        public ActionResult ResolverGroupTypes()
        {
            return View();
        }

        public JsonResult GetResolverGroupTypes()
        {
            var r = _resolverGroupTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.ResolverGroupTypeName).ToList();
            return Json(r, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAndNotVisibleResolverGroupTypesForCustomer()
        {
            var list = new List<ResolverGroupTypeRefData>();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                list.AddRange(_resolverGroupTypeRefDataService.GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReadAjaxResolverGroupTypesRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _resolverGroupTypeRefDataService.GetResolverGroupTypeRefDataWithUsageStats()
                    .ToDataSourceResult(request, Mapper.Map<ResolverGroupTypeRefDataListItem, ResolverGroupTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxResolverGroupTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ResolverGroupTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_resolverGroupTypeRefDataService.All().Any(c => c.ResolverGroupTypeName.ToLower() == model.ResolverGroupTypeName.Trim().ToLower()))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError",
                          string.Format(WebResources.EntityNameIsNotUnique, "Resolver Group Type"));
                    }
                    else
                    {
                        var resolverGroup = Mapper.Map<ResolverGroupTypeRefDataViewModel, ResolverGroupTypeRefData>(model);
                        resolverGroup.Id = 0;
                        _resolverGroupTypeRefDataService.Create(resolverGroup);
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
        public ActionResult UpdateAjaxResolverGroupTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ResolverGroupTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resolverGroup = _resolverGroupTypeRefDataService.GetById(model.Id);
                    Mapper.Map(model, resolverGroup);
                    _resolverGroupTypeRefDataService.Update(resolverGroup);
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
        public ActionResult DeleteAjaxResolverGroupTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, ResolverGroupTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _resolverGroupTypeRefDataService.GetById(model.Id);

                    if (entity == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted,
                            string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Resolver Group Type"));
                    }

                    else
                    {
                        if (_resolverGroupTypeRefDataService.GetNumberOfResolverGroupTypeReferences(entity.Id) > 0)
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ReferenceDataItemIsStillUtilised,
                                string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Resolver Group Type"));
                        }
                        else
                        {

                            _resolverGroupTypeRefDataService.Delete(entity);
                        }
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

        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateResolverGroupType()
        {
            var vm = new ResolverGroupTypeRefDataViewModel { Order = 5, Visible = false };
            return PartialView("_ResolverGroupType", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateResolverGroupType(ResolverGroupTypeRefDataViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resolverGroupType = _resolverGroupTypeRefDataService.All()
                        .FirstOrDefault(r => r.ResolverGroupTypeName.ToLower() == model.ResolverGroupTypeName.Trim().ToLower());
                    model.Id = resolverGroupType?.Id ?? _resolverGroupTypeRefDataService.Create(Mapper.Map<ResolverGroupTypeRefDataViewModel, ResolverGroupTypeRefData>(model));
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                _loggingManager.Write(LoggingEventSource.WebUi, LoggingEventType.Error, ex.Message);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }

            // The model is required to set the appropiate UI field
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Operational Process Type Ref Data

        public ActionResult OperationalProcessTypes()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateOperationalProcessType()
        {
            var vm = new OperationalProcessTypeRefDataViewModel { SortOrder = 5, Visible = false, Standard = false };
            return PartialView("_OperationalProcessType", vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateOperationalProcessType(OperationalProcessTypeRefDataViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var operationalProcessType = _operationalProcessTypeRefDataService.All()
                        .FirstOrDefault(o => String.Equals(o.OperationalProcessTypeName, model.OperationalProcessTypeName.Trim(), StringComparison.CurrentCultureIgnoreCase));
                    model.Id = operationalProcessType?.Id ?? _operationalProcessTypeRefDataService.Create(Mapper.Map<OperationalProcessTypeRefDataViewModel, OperationalProcessTypeRefData>(model));
                }
                else
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = ModelState.GetModelStateMesssages().FirstOrDefault() });
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                _loggingManager.Write(LoggingEventSource.WebUi, LoggingEventType.Error, ex.Message);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAllAndNotVisibleOperationalProcessTypesForCustomer()
        {
            var result = new List<OperationalProcessTypeRefData>();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                result.AddRange(_operationalProcessTypeRefDataService.GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id).ToList());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]


        public ActionResult ReadAjaxOperationalProcessTypesRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _operationalProcessTypeRefDataService.GetOperationalProcessTypeRefDataWithUsageStats()
                    .ToDataSourceResult(request, Mapper.Map<OperationalProcessTypeRefDataListItem, OperationalProcessTypeRefDataViewModel>);
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
        public ActionResult CreateAjaxOperationalProcessTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, OperationalProcessTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_operationalProcessTypeRefDataService.All().Any(c => c.OperationalProcessTypeName.ToLower() == model.OperationalProcessTypeName.Trim().ToLower()))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.EntityNameIsNotUnique, "Operational Process Type"));
                    }
                    else
                    {
                        // Insert
                        var opt = Mapper.Map<OperationalProcessTypeRefDataViewModel, OperationalProcessTypeRefData>(model);
                        opt.Id = 0;
                        _operationalProcessTypeRefDataService.Create(opt);
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
        public ActionResult UpdateAjaxOperationalProcessTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, OperationalProcessTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var opt = _operationalProcessTypeRefDataService.GetById(model.Id);
                    Mapper.Map(model, opt);
                    _operationalProcessTypeRefDataService.Update(opt);
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
        public ActionResult DeleteAjaxOperationalProcessTypesRefDataGrid([DataSourceRequest]DataSourceRequest request, OperationalProcessTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var opt = _operationalProcessTypeRefDataService.GetById(model.Id);

                    if (opt == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted,
                            string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Operational Process Type"));
                    }

                    else
                    {
                        if (_operationalProcessTypeRefDataService.GetNumberOfOperationalProcessTypeReferences(opt.Id) > 0)
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ReferenceDataItemIsStillUtilised,
                                string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Operational Process Type"));
                        }
                        else
                        {
                            _operationalProcessTypeRefDataService.Delete(opt);
                        }
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

        #endregion

        [AllowAnonymous]
        public JsonResult GetRegionTypes()
        {
            var regions = _regionTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.RegionName).ToList();

            return Json(regions, JsonRequestBehavior.AllowGet);
        }
    }
}