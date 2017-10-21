using AutoMapper;
using Fujitsu.SLM.Constants;
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
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Architect)]
    public class AuditController : Controller
    {
        private readonly IAuditService _auditService;
        private readonly ICustomerService _customerService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public AuditController(
            IAuditService auditService,
            ICustomerService customerService,
            IContextManager contextManager,
            IAppUserContext appUserContext)
        {
            if (auditService == null)
            {
                throw new ArgumentNullException(nameof(auditService));
            }

            if (customerService == null)
            {
                throw new ArgumentNullException(nameof(customerService));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            _auditService = auditService;
            _customerService = customerService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;

        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadAjaxAuditGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    result = _auditService.CustomerAudits(_appUserContext.Current.CurrentCustomer.Id)
                            .ToDataSourceResult(request, Mapper.Map<Audit, AuditViewModel>);
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
        [OutputCache(Duration = 0)]
        public ActionResult CreateAjaxAuditGrid([DataSourceRequest]DataSourceRequest request, AuditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dateTimeNow = DateTime.Now;

                    var maximumVersionNumer = _auditService.All().Where(x => x.CustomerId == _appUserContext.Current.CurrentCustomer.Id).OrderByDescending(y => y.Version).Select(y => y.Version).FirstOrDefault();

                    if (model.Version > maximumVersionNumer)
                    {
                        model.InsertedDate = dateTimeNow;
                        model.UpdatedDate = dateTimeNow;
                        var audit = Mapper.Map<AuditViewModel, Audit>(model);
                        _auditService.Create(audit);
                    }
                    else
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", WebResources.CustomerAuditVersionNumber);
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
        public JsonResult CreateCustomerBaseline(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                // Baseline Customer
                var customer = _customerService.GetById(id);
                _auditService.CreateAuditBaseline(customer);
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
        public ActionResult GetNextVersionNumber()
        {
            var maximumVersionNumer = _auditService.All().Where(x => x.CustomerId == _appUserContext.Current.CurrentCustomer.Id).OrderByDescending(y => y.Version).Select(y => y.Version).FirstOrDefault();

            var result = new
            {
                VersionNumber = maximumVersionNumer + 0.1
            };

            return Json(result);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var audit = _auditService.GetById(id);
            var auditViewModel = Mapper.Map<Audit, AuditViewModel>(audit);

            return View("Edit", auditViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var now = DateTime.Now;
                var audit = _auditService.GetById(model.Id);
                audit.ReasonForIssue = model.ReasonForIssue;
                audit.Notes = model.Notes;
                audit.UpdatedBy = _contextManager.UserManager.Name;
                audit.UpdatedDate = now;

                _auditService.Update(audit);

                return RedirectToAction("Index", "Audit");
            }

            return View("Edit", model);
        }
    }
}