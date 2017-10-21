using AutoMapper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IContributorService _contributorService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public CustomerController(ICustomerService customerService,
            IContributorService contributorService,
            IContextManager contextManager,
            IAppUserContext appUserContext)
        {
            if (customerService == null)
            {
                throw new ArgumentNullException(nameof(customerService));
            }

            if (contributorService == null)
            {
                throw new ArgumentNullException(nameof(contributorService));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            _customerService = customerService;
            _contributorService = contributorService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;

        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult MyCustomers()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult MyArchives()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Customers()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Archives()
        {
            return View();
        }

        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxMyCustomersGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                var userName = _contextManager.UserManager.Name;
                result = _customerService.MyCustomers(userName).ToDataSourceResult(request, Mapper.Map<Customer, CustomerViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxCustomersGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _customerService.Customers().ToDataSourceResult(request, Mapper.Map<Customer, CustomerViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxMyArchivesGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                var userName = _contextManager.UserManager.Name;
                result = _customerService.MyArchives(userName).ToDataSourceResult(request, Mapper.Map<Customer, CustomerViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        public ActionResult ReadAjaxArchivesGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                result = _customerService.Archives().ToDataSourceResult(request, Mapper.Map<Customer, CustomerViewModel>);
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
        public ActionResult CreateAjaxCustomerGrid([DataSourceRequest]DataSourceRequest request, CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_customerService.All().Any(c => c.CustomerName == model.CustomerName))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.EntityNameIsNotUnique, "Customer Name"));
                    }
                    else
                    {
                        var userName = _contextManager.UserManager.Name;
                        var dateTimeNow = DateTime.Now;

                        // Insert
                        var customer = Mapper.Map<CustomerViewModel, Customer>(model);
                        customer.AssignedArchitect = userName;
                        customer.Active = true;
                        customer.Baseline = false;
                        customer.InsertedBy = userName;
                        customer.InsertedDate = dateTimeNow;
                        customer.UpdatedBy = userName;
                        customer.UpdatedDate = dateTimeNow;
                        _customerService.Create(customer);
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
        public ActionResult UpdateAjaxCustomerGrid([DataSourceRequest]DataSourceRequest request, CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = _contextManager.UserManager.Name;
                    var dateTimeNow = DateTime.Now;

                    // Update
                    var customer = _customerService.GetById(model.Id);

                    if (customer == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", WebResources.CustomerCannotFindCustomerToBeUpdated);
                    }
                    else
                    {
                        customer.CustomerName = model.CustomerName;
                        customer.CustomerNotes = model.CustomerNotes;
                        customer.UpdatedDate = dateTimeNow;
                        customer.UpdatedBy = userName;

                        _customerService.Update(customer);
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
        public ActionResult DeleteAjaxCustomerGrid([DataSourceRequest]DataSourceRequest request, CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = _customerService.GetById(model.Id);

                    if (customer == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", WebResources.CustomerCannotFindCustomerToBeDeleted);
                    }
                    else if (customer.ServiceDesks != null && customer.ServiceDesks.Any())
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", WebResources.CustomerCannotBeDeletedDueToServiceDesksExisting);
                    }
                    else
                    {
                        _customerService.Delete(customer);
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
        public ActionResult DeleteArchivedCustomer(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var customer = _customerService.GetById(id);
                _customerService.Delete(customer.Id);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", Message = ex.Message };
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public JsonResult ArchiveCustomer(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // Archive Customer
                var customer = _customerService.GetById(id);
                customer.Active = false;
                customer.UpdatedDate = DateTime.Now;
                customer.UpdatedBy = _contextManager.UserManager.Name;

                _customerService.Update(customer);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", ex.Message };
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public JsonResult ActivateCustomer(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // Activate Customer
                var customer = _customerService.GetById(id);

                customer.Active = true;
                customer.UpdatedDate = DateTime.Now;
                customer.UpdatedBy = _contextManager.UserManager.Name;

                _customerService.Update(customer);

            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", ex.Message };
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public JsonResult GetCustomers()
        {
            var customers = _customerService.All().Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(CultureInfo.InvariantCulture),
                Text = x.CustomerName,
            }).ToList();

            customers.Add(new SelectListItem
            {
                Value = "0",
                Text = WebResources.DefaultDropDownListText
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult ChangeOwner(ChangeCustomerOwnerViewModel model)
        {
            var result = new { Success = "True", Message = "Success" };
            if (ModelState.IsValid)
            {
                try
                {
                    // Change Owner
                    var customer = _customerService.GetById(model.CustomerId);

                    customer.AssignedArchitect = model.Email;
                    customer.UpdatedDate = DateTime.Now;
                    customer.UpdatedBy = _contextManager.UserManager.Name;

                    _customerService.Update(customer);
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                    result = new { Success = "False", ex.Message };
                    ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
                }
            }
            else
            {
                result = new { Success = "False", Message = ModelState.GetModelStateMesssages().FirstOrDefault() };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxContributorGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    result =
                        _contributorService.GetCustomersContributors(_appUserContext.Current.CurrentCustomer.Id)
                            .ToDataSourceResult(request, Mapper.Map<Contributor, ContributorViewModel>);
                }

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
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteAjaxContributorGrid([DataSourceRequest] DataSourceRequest request, ContributorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contributor = _contributorService.GetById(model.Id);

                    if (contributor == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.ContributorCannotBeFound);
                    }
                    else
                    {
                        _contributorService.Delete(contributor);
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
        public ActionResult DeleteContributor(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var contributor = _contributorService.GetById(id);
                _contributorService.Delete(contributor);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", Message = ex.Message };
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}