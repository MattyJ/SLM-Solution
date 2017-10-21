using AutoMapper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using EnumExtensions = Fujitsu.SLM.Enumerations.EnumExtensions;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    public class ServiceDecompositionController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerPackService _customerPackService;
        private readonly IDiagramService _diagramService;
        private readonly IContributorService _contributorService;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public ServiceDecompositionController(ICustomerService customerService,
            ICustomerPackService customerPackService,
            IDiagramService diagramService,
            IContributorService contributorService,
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            IContextManager contextManager,
            IAppUserContext appUserContext) : base(contextManager)
        {
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

            if (customerPackService == null)
            {
                throw new ArgumentNullException(nameof(customerPackService));
            }

            if (diagramService == null)
            {
                throw new ArgumentNullException(nameof(diagramService));
            }

            if (contributorService == null)
            {
                throw new ArgumentNullException(nameof(contributorService));
            }

            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            _customerService = customerService;
            _customerPackService = customerPackService;
            _diagramService = diagramService;
            _contributorService = contributorService;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextManager = contextManager;
            _appUserContext = appUserContext;

        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult EditCustomer(int id)
        {
            var customer = _customerService.GetById(id);
            if (customer != null)
            {
                var userName = _contextManager.UserManager.Name;
                if (_contextManager.UserManager.IsSLMAdministrator()
                    || _contextManager.UserManager.IsSLMArchitect() && (customer.AssignedArchitect.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                    || _contributorService.IsContributor(customer.Id, userName)))
                {
                    var currentCustomer = new CurrentCustomerViewModel
                    {
                        Id = customer.Id,
                        CustomerName = customer.CustomerName,
                        Baseline = customer.Baseline
                    };

                    _appUserContext.Current.CurrentCustomer = currentCustomer;
                }
                else
                {
                    throw new UnauthorizedAccessException("This user does not have access to the selected Customer.");
                }

                var model = Mapper.Map<Customer, CustomerViewModel>(customer);
                if (string.Equals(_contextManager.UserManager.Name, model.AssignedArchitect, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.Owner = true;
                }
                model.ReturnUrl = Url.Action("MyCustomers", "Customer");

                return View("EditCustomer", model);
            }

            return RedirectToAction("MyCustomers", "Customer");
        }


        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(CustomerViewModel model, string action, string level)

        {

            if (action.Equals("Cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToAction("MyCustomers", "Customer");
            }

            if (action.Equals("Save", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ModelState.IsValid)
                {
                    if (!_customerService.All().Any(c => c.CustomerName == model.CustomerName && c.Id != model.Id))
                    {
                        var userName = _contextManager.UserManager.Name;
                        var dateTimeNow = DateTime.Now;

                        // Update Customer
                        var customer = _customerService.GetById(model.Id);
                        customer.CustomerName = model.CustomerName;
                        customer.CustomerNotes = model.CustomerNotes;
                        customer.Baseline = model.Baseline;
                        customer.UpdatedDate = dateTimeNow;
                        customer.UpdatedBy = userName;

                        _customerService.Update(customer);
                        return RedirectToAction("MyCustomers", "Customer");
                    }

                    ModelState.AddModelError("CustomerName", string.Format(WebResources.EntityNameIsNotUnique, "Customer Name"));
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult UploadCustomerFile(string level)
        {
            var vm = new UploadCustomerFileViewModel
            {
                EditLevel = level,
                Levels = EnumExtensions.AsSelectListItems<LevelName>(),
                DisplayLevel = string.IsNullOrEmpty(level)
            };
            return PartialView("_UploadCustomerFile", vm);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCustomerFile(UploadCustomerFileViewModel model)
        {
            var result = GetJsonSuccessResponse();
            if (ModelState.IsValid)
            {
                try
                {
                    if (_appUserContext.Current.CurrentCustomer != null)
                    {
                        var userName = _contextManager.UserManager.Name;
                        var dateTimeNow = DateTime.Now;

                        var diagram = new Model.Diagram
                        {
                            Level = model.EditLevel.GetEnumIntFromText<LevelName>(),
                            CustomerId = _appUserContext.Current.CurrentCustomer.Id,
                            Filename = Path.GetFileName(model.CustomerFile.FileName),
                            DiagramNotes = model.Notes,
                            MimeType = model.CustomerFile.ContentType,
                            UpdatedDate = dateTimeNow,
                            UpdatedBy = userName,
                            InsertedDate = dateTimeNow,
                            InsertedBy = userName
                        };

                        using (var reader = new BinaryReader(model.CustomerFile.InputStream))
                        {
                            diagram.DiagramData = reader.ReadBytes(model.CustomerFile.ContentLength);
                        }

                        _diagramService.Create(diagram);
                    }
                }
                catch (Exception ex)
                {
                    result = GetJsonErrorResponse(ex.Message);
                    ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
                }
            }
            else
            {
                _contextManager.ResponseManager.StatusCode = 500;
                return Json(new DataSourceResult { Errors = ModelState.GetModelStateMesssages().FirstOrDefault() });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult AddCustomerPack(string level)
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = level,
                Levels = EnumExtensions.AsSelectListItems<LevelName>(),
                DisplayLevel = string.IsNullOrEmpty(level),
                Filename = "CustomerPack.zip"
            };
            return PartialView("_AddCustomerPack", vm);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomerPack(AddCustomerPackViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_appUserContext.Current.CurrentCustomer != null)
                    {
                        // Get the level.
                        var level = model.EditLevel.GetEnumIntFromText<LevelName>();

                        // Get all the diagrams for this level.
                        var diagrams = _diagramService
                            .Diagrams(_appUserContext.Current.CurrentCustomer.Id)
                            .Where(x => x.Level == level)
                            .ToList();

                        using (var zipMemoryStream = new MemoryStream())
                        {
                            // Build the archive
                            using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                            {
                                var i = 1;
                                foreach (var diagram in diagrams)
                                {
                                    var zipArchiveEntry = zipArchive.CreateEntry(string.Format("{0:000}", i++) + diagram.Filename);
                                    using (var zipArchiveEntryStream = zipArchiveEntry.Open())
                                    {
                                        zipArchiveEntryStream.Write(diagram.DiagramData, 0 /* offset */,
                                            diagram.DiagramData.Length);
                                    }
                                }
                            }

                            //Rewind the stream for reading to output.
                            zipMemoryStream.Seek(0, SeekOrigin.Begin);

                            // Create a new Customer Pack entity.
                            var now = DateTime.Now;
                            var customerPack = new CustomerPack
                            {
                                CustomerId = _appUserContext.Current.CurrentCustomer.Id,
                                Filename = model.Filename,
                                Level = level,
                                MimeType = MimeTypeNames.Zip,
                                PackData = zipMemoryStream.ToArray(),
                                PackNotes = model.PackNotes,
                                InsertedDate = now,
                                InsertedBy = _contextManager.UserManager.Name,
                                UpdatedDate = now,
                                UpdatedBy = _contextManager.UserManager.Name
                            };

                            _customerPackService.Create(customerPack);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                    ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
                }
            }
            return CustomerPack();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult CustomerPack()
        {
            var vm = new CustomerPackGridViewModel();
            return View("CustomerPack", vm);
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult ReadAjaxCustomerPackGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current.CurrentCustomer != null)
                {
                    result = _customerPackService
                        .CustomerPacks()
                        .Where(w => w.CustomerId == _appUserContext.Current.CurrentCustomer.Id)
                        .ToDataSourceResult(request, Mapper.Map<CustomerPack, CustomerPackViewModel>);
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
        public ActionResult UpdateAjaxCustomerPackGrid([DataSourceRequest] DataSourceRequest request, CustomerPackViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_appUserContext.Current.CurrentCustomer != null)
                    {
                        var customerPack = _customerPackService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id, model.Id);
                        if (customerPack != null)
                        {
                            customerPack.PackNotes = model.PackNotes;
                            customerPack.Filename = model.Filename;
                            customerPack.UpdatedDate = DateTime.Now;
                            customerPack.UpdatedBy = _contextManager.UserManager.Name;
                            _customerPackService.Update(customerPack);
                        }
                        else
                        {
                            ModelState.AddModelError(ModelStateErrorNames.CustomerPackCannotBeFound, WebResources.CustomerPackCannotBeFound);
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
        public ActionResult DeleteAjaxCustomerPackGrid([DataSourceRequest] DataSourceRequest request, CustomerPackViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customerPack = _customerPackService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id, model.Id);

                    if (customerPack == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.CustomerPackCannotBeFound);
                    }
                    else
                    {
                        _customerPackService.Delete(customerPack);
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
        public ActionResult DeleteCustomerPack(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var customerPack = _customerPackService.GetById(id);
                _customerPackService.Delete(customerPack);
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

        [HttpGet]
        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult DownloadCustomerPack(int id)
        {
            ActionResult actionResult = null;
            try
            {
                var customerPack = _customerPackService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id, id);
                if (customerPack != null)
                {
                    var contentDisposition = new ContentDisposition
                    {
                        FileName = customerPack.Filename,
                        Inline = false
                    };
                    _contextManager.ResponseManager.AppendHeader("Content-Disposition", contentDisposition.ToString());
                    actionResult = new FileStreamResult(new MemoryStream(customerPack.PackData), customerPack.MimeType);
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }
            return actionResult;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult AddContributor()
        {
            var vm = new AddCustomerContributorViewModel();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                vm.CustomerId = _appUserContext.Current.CurrentCustomer.Id;
                vm.CustomerName = _appUserContext.Current.CurrentCustomer.CustomerName;

                vm.ReturnUrl = Url.Action("EditCustomer", "ServiceDecomposition", new { id = vm.CustomerId });

                var users = GetUsersInRole(UserRoles.Architect, true).ToList();

                if (users.Any())
                {
                    var customerContributors = _contributorService.All()
                        .Where(c => c.CustomerId == vm.CustomerId)
                        .Select(c => c.UserId)
                        .ToList();

                    if (customerContributors.Any())
                    {
                        foreach (var contributor in customerContributors)
                        {
                            // Remove any users already contributing
                            users.RemoveAll(user => string.Equals(user.Id, contributor, StringComparison.InvariantCultureIgnoreCase));
                        }
                    }
                }

                vm.Users.AddRange(users.Select(s => new SelectListItem { Text = s.UserName, Value = s.Id.ToString(CultureInfo.InvariantCulture) }).ToList());

            }

            return View("AddCustomerContributor", vm);
        }

        [HttpGet]
        private IEnumerable<ApplicationUser> GetUsersInRole(string role, bool excludeCurrentUser)
        {
            var applicationRoleId = _roleManager.FindByName(role).Id;
            var currentUser = _contextManager.UserManager.Name;

            if (excludeCurrentUser)
            {
                return _userManager.Users
                    .Where(u => u.UserName != currentUser && (string.IsNullOrEmpty(role) ||
                                                              u.Roles.Any(y => y.RoleId == applicationRoleId)))
                    .OrderBy(u => u.UserName)
                    .ToList();
            }

            return _userManager.Users
                    .Where(u => string.IsNullOrEmpty(role) || u.Roles.Any(y => y.RoleId == applicationRoleId))
                    .OrderBy(u => u.UserName)
                    .ToList();

        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult CreateAjaxAddCustomerContributorGrid([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<BulkCustomerContributorViewModel> customerContributors)
        {
            var outputContributors = new List<BulkCustomerContributorViewModel>();

            if (ModelState.IsValid)
            {
                var inputContributors = customerContributors as IList<BulkCustomerContributorViewModel> ?? customerContributors.ToList();
                try
                {
                    if (customerContributors != null && inputContributors.Any())
                    {
                        var userName = _contextManager.UserManager.Name;
                        var now = DateTime.Now;
                        var customerId = _appUserContext.Current.CurrentCustomer.Id;
                        var contributors = _contributorService.GetCustomersContributors(customerId).Select(c => c.UserId).ToList();

                        foreach (var contributor in inputContributors)
                        {

                            if (string.IsNullOrEmpty(contributor.UserId) ||
                                contributors.Contains(contributor.UserId))
                            {
                                continue;
                            }

                            var user = _userManager.Users.FirstOrDefault(u => u.Id == contributor.UserId);

                            if (user != null)
                            {
                                var newContributor = new Contributor
                                {
                                    UserId = contributor.UserId,
                                    EmailAddress = user.Email,
                                    CustomerId = customerId,
                                    InsertedBy = userName,
                                    InsertedDate = now,
                                    UpdatedBy = userName,
                                    UpdatedDate = now
                                };

                                newContributor.Id = _contributorService.Create(newContributor);
                                outputContributors.Add(Mapper.Map<BulkCustomerContributorViewModel>(newContributor));
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

            return Json(outputContributors.ToDataSourceResult(request, ModelState));
        }

        #region Add Customer Contributor Grid - Not used for any function, but required by Inline edit to work.

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ReadAjaxAddCustomerContributorGrid([DataSourceRequest] DataSourceRequest request)
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
        public ActionResult UpdateAjaxAddCustomerContributoGrid([DataSourceRequest] DataSourceRequest request)
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
        public ActionResult DestroyAjaxAddCustomerContributoGrid([DataSourceRequest] DataSourceRequest request)
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