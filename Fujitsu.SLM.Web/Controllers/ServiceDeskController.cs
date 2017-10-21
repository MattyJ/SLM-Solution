using AutoMapper;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Attributes;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    public class ServiceDeskController : Controller
    {
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IInputTypeRefDataService _inputTypeRefDataService;
        private readonly ITemplateService _templateService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public ServiceDeskController(IServiceDeskService serviceDeskService,
            IInputTypeRefDataService inputTypeRefDataService,
            ITemplateService templateService,
            IContextManager contextManager,
            IAppUserContext appUserContext)
        {
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }
            if (inputTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(inputTypeRefDataService));
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

            _serviceDeskService = serviceDeskService;
            _inputTypeRefDataService = inputTypeRefDataService;
            _templateService = templateService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;

        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            var customerName = string.Empty;
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                customerName = _appUserContext.Current.CurrentCustomer.CustomerName;
            }
            return View(level, model: customerName);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpGet]
        public ActionResult Add(string level)
        {
            var model = new AddServiceDeskViewModel();
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                model.EditLevel = level;
                model.ReturnUrl = Url.Action("Index", "ServiceDesk", new { level = level });

                model.ServiceDesk = new ServiceDeskViewModel
                {
                    CustomerId = _appUserContext.Current.CurrentCustomer.Id,
                    CustomerName = _appUserContext.Current.CurrentCustomer.CustomerName,
                    DeskName = string.Empty,
                    DeskInputTypes = new List<InputTypeRefData>()
                };

                foreach (var input in _inputTypeRefDataService.All().Where(x => x.Default).ToList())
                {
                    model.ServiceDesk.DeskInputTypes.Add(input);
                }

                return View("Add" + level, model);
            }

            return RedirectToAction("Index", "ServiceDesk", new { level = level });
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string level, string deskName, int[] deskInputTypes)
        {
            if (_appUserContext.Current.CurrentCustomer == null || _appUserContext.Current.CurrentCustomer.Id <= 0)
            {
                // No context
                return RedirectToAction("Index", "ServiceDesk", new { level = level });
            }

            // Create Service Desk
            var serviceDesk = new ServiceDesk();
            var now = DateTime.Now;
            var user = _contextManager.UserManager.Name;
            serviceDesk.CustomerId = _appUserContext.Current.CurrentCustomer.Id;
            serviceDesk.DeskName = deskName;
            serviceDesk.DeskInputTypes = new List<DeskInputType>();

            if (deskInputTypes != null && deskInputTypes.Length > 0)
            {
                // Add Desk Input Types
                foreach (var deskInputTypeId in deskInputTypes)
                {
                    serviceDesk.DeskInputTypes.Add(new DeskInputType
                    {
                        InputTypeRefData = _inputTypeRefDataService.GetById(deskInputTypeId)
                    });
                }
            }

            serviceDesk.InsertedBy = user;
            serviceDesk.InsertedDate = now;
            serviceDesk.UpdatedBy = user;
            serviceDesk.UpdatedDate = now;

            _serviceDeskService.Create(serviceDesk);

            return RedirectToAction("Index", "ServiceDesk", new { level = level });
        }


        [Authorize(Roles = UserRoles.Architect)]
        [HttpGet]
        public ActionResult Edit(string level, int id)
        {
            if (_appUserContext.Current.CurrentCustomer == null || _appUserContext.Current.CurrentCustomer.Id == 0)
            {
                return GetRedirect(level, null);
            }

            var serviceDeskCount = _serviceDeskService
                .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .Select(s => s.Id)
                .Count();

            var serviceDesk = _serviceDeskService
                .GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id, id);
            if (serviceDesk == null)
            {
                return GetRedirect(level, _appUserContext.Current.CurrentCustomer.Id);
            }

            var templates = _templateService.AllTemplates().ToList();
            var vm = new EditServiceDeskViewModel
            {
                ServiceDesk = Mapper.Map<ServiceDeskViewModel>(serviceDesk),
                EditLevel = level,
                CanMoveServiceDomain = serviceDeskCount > 1,
                CanImportServiceConfiguratorTemplate = templates.Any(x => x.TemplateType == TemplateTypeNames.SORT),
                CanImportServiceLandscapeTemplate = templates.Any(x => x.TemplateType == TemplateTypeNames.SLM)
            };

            return View("Edit" + level, vm);
        }

        [Authorize(Roles = UserRoles.Architect)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string level, int id, string deskName, int[] deskInputTypes)
        {
            if (_appUserContext.Current.CurrentCustomer == null ||
                _appUserContext.Current.CurrentCustomer.Id == 0)
            {
                return GetRedirect(level, null);
            }
            var customerId = _appUserContext.Current.CurrentCustomer.Id;
            if (_contextManager.RequestManager.Form[FormValuesNames.Return] != null)
            {
                return GetRedirect(level, customerId);
            }

            var serviceDesk = _serviceDeskService.GetByCustomerAndId(customerId, id);
            if (serviceDesk == null)
            {
                return GetRedirect(level, _appUserContext.Current.CurrentCustomer.Id);
            }

            if (ModelState.IsValid)
            {
                var now = DateTime.Now;

                serviceDesk.DeskName = deskName;
                serviceDesk.UpdatedBy = _contextManager.UserManager.Name;
                serviceDesk.UpdatedDate = now;

                // Add Desk Input Types
                if (deskInputTypes != null && deskInputTypes.Length > 0)
                {
                    // Remove Desk Input Types
                    var deletedDeskInputTypes = serviceDesk
                        .DeskInputTypes
                        .RemoveAll(x => !deskInputTypes.Any(y => y == x.InputTypeRefData.Id))
                        .Select(s => s.Id)
                        .ToList();

                    var newDeskInputTypeIds =
                        deskInputTypes.Where(x => serviceDesk.DeskInputTypes.All(y => y.InputTypeRefData.Id != x));

                    foreach (var deskInputTypeId in newDeskInputTypeIds)
                    {
                        serviceDesk.DeskInputTypes.Add(new DeskInputType
                        {
                            InputTypeRefData = _inputTypeRefDataService.GetById(deskInputTypeId)
                        });
                    }

                    _serviceDeskService.Update(serviceDesk, deletedDeskInputTypes);
                }
                else
                {
                    // Remove all of the Desk Input Types
                    if (serviceDesk.DeskInputTypes.Any())
                    {
                        var deletedDeskInputTypes = serviceDesk
                            .DeskInputTypes
                            .Select(s => s.Id)
                            .ToList();

                        _serviceDeskService.Update(serviceDesk, deletedDeskInputTypes);
                    }
                    else
                    {
                        _serviceDeskService.Update(serviceDesk);
                    }
                }

                return GetRedirect(level, _appUserContext.Current.CurrentCustomer.Id);
            }

            var templates = _templateService.AllTemplates().ToList();

            var vm = new EditServiceDeskViewModel
            {
                EditLevel = level,
                ServiceDesk = Mapper.Map<ServiceDeskViewModel>(serviceDesk),
                CanImportServiceConfiguratorTemplate = templates.Any(x => x.TemplateType == TemplateTypeNames.SORT),
                CanImportServiceLandscapeTemplate = templates.Any(x => x.TemplateType == TemplateTypeNames.SLM)
            };

            return View("Edit" + level, vm);
        }

        [HttpPost]
        [AjaxModelErrorStatusModifier]
        public ActionResult ReadAjaxServiceDeskGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _serviceDeskService.All().Where(x => x.CustomerId == _appUserContext.Current.CurrentCustomer.Id)
                        .ToDataSourceResult(request, Mapper.Map<ServiceDesk, ServiceDeskViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }

            return Json(result);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxServiceDeskGrid([DataSourceRequest]DataSourceRequest request, ServiceDeskViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = _contextManager.UserManager.Name;
                    var dateTimeNow = DateTime.Now;

                    // Insert
                    var serviceDesk = Mapper.Map<ServiceDeskViewModel, ServiceDesk>(model);
                    serviceDesk.CustomerId = model.CustomerId;
                    serviceDesk.InsertedBy = userName;
                    serviceDesk.InsertedDate = dateTimeNow;
                    serviceDesk.UpdatedBy = userName;
                    serviceDesk.UpdatedDate = dateTimeNow;
                    serviceDesk.DeskInputTypes = new List<DeskInputType>();
                    foreach (var comm in model.DeskInputTypes)
                    {
                        serviceDesk.DeskInputTypes.Add(new DeskInputType()
                        {
                            InputTypeRefData = _inputTypeRefDataService.GetById(comm.Id)
                        });
                    }
                    _serviceDeskService.Create(serviceDesk);

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
        public ActionResult DeleteAjaxServiceDeskGrid([DataSourceRequest]DataSourceRequest request, ServiceDeskViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDesk = _serviceDeskService.GetById(model.Id);

                    if (serviceDesk == null)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ServiceDeskDeleteEntityDoesNotExist,
                            WebResources.ServiceDeskCannotFindServiceDeskToBeDeleted);
                    }
                    else if (serviceDesk.ServiceDomains.Any())
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ErrorMessage,
                           WebResources.ServiceDesksCannotBeDeletedDueToServiceDomainsExisting);
                    }
                    else
                    {
                        _serviceDeskService.Delete(serviceDesk);
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
        public ActionResult DeleteServiceDesk(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // RetryableOperation to Log any Unexpected Errors and Alert Error Instance id.
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        var serviceDesk = _serviceDeskService.GetById(id);
                        _serviceDeskService.Delete(serviceDesk);
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
        public JsonResult GetServiceDesks()
        {
            List<SelectListItem> selectListItems;
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                selectListItems = _serviceDeskService.All().Where(x => x.CustomerId == _appUserContext.Current.CurrentCustomer.Id).Select(x => new SelectListItem()
                {
                    Value = x.Id.ToString(CultureInfo.InvariantCulture),
                    Text = x.DeskName,
                }).ToList();
            }
            else
            {
                selectListItems = _serviceDeskService.All().Select(x => new SelectListItem()
                {
                    Value = x.Id.ToString(CultureInfo.InvariantCulture),
                    Text = x.DeskName,
                }).ToList();
            }

            selectListItems.Add(new SelectListItem()
            {
                Value = "0",
                Text = WebResources.DefaultDropDownListText,
            });

            return Json(selectListItems, JsonRequestBehavior.AllowGet);
        }


        private ActionResult GetRedirect(string level, int? customerId)
        {
            if (customerId == null)
            {
                return RedirectToAction("MyCustomers", "Customer");
            }
            return string.IsNullOrEmpty(level) ? RedirectToAction("EditCustomer", "ServiceDecomposition", new { Id = customerId }) : RedirectToAction("Index", "ServiceDesk", new { Level = level });
        }

        #region Diagrams

        [HttpGet]
        public PartialViewResult ServiceDeskStructure(int level)
        {
            return PartialView("_ServiceDeskStructure", level);
        }

        [HttpGet]
        public PartialViewResult ServiceDeskDotMatrix(int level)
        {
            return PartialView("_ServiceDeskDotMatrix", level);
        }

        [HttpGet]
        public PartialViewResult FujitsuDomains(int level)
        {
            return PartialView("_FujitsuDomains", level);
        }

        [HttpGet]
        public PartialViewResult CustomerServices(int level)
        {
            return PartialView("_CustomerServices", level);
        }

        [HttpGet]
        public PartialViewResult ServiceOrganisation(int level, string type)
        {
            if (type.SafeEquals(Constants.Diagram.FujitsuServiceOrganisation))
            {
                return PartialView("_FujitsuServiceOrganisation", level);
            }

            if (type.SafeEquals(Constants.Diagram.CustomerServiceOrganisation))
            {
                return PartialView("_CustomerServiceOrganisation", level);
            }

            return PartialView("_CustomerThirdPartyServiceOrganisation", level);
        }

        #endregion

    }
}