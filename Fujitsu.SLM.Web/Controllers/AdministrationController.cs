using AutoMapper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using Parameter = Fujitsu.SLM.Model.Parameter;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Administrator)]
    public class AdministrationController : Controller
    {
        private readonly IParameterService _parameterService;
        private readonly ICustomerService _customerService;
        private readonly IContributorService _contributorService;
        private readonly IAssetService _assetService;
        private readonly IRegionTypeRefDataService _regionTypeRefDataService;

        private readonly IContextManager _contextManager;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;

        public AdministrationController(IParameterService parameterService,
            ICustomerService customerService,
            IContributorService contributorService,
            IAssetService assetService,
            IRegionTypeRefDataService regionTypeRefDataService,
            IContextManager contextManager,
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }
            if (customerService == null)
            {
                throw new ArgumentNullException(nameof(customerService));
            }
            if (contributorService == null)
            {
                throw new ArgumentNullException(nameof(contributorService));
            }
            if (assetService == null)
            {
                throw new ArgumentNullException(nameof(assetService));
            }
            if (regionTypeRefDataService == null)
            {
                throw new ArgumentNullException(nameof(regionTypeRefDataService));
            }
            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            _parameterService = parameterService;
            _customerService = customerService;
            _contributorService = contributorService;
            _assetService = assetService;
            _regionTypeRefDataService = regionTypeRefDataService;
            _contextManager = contextManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Administration
        public ActionResult Index()
        {
            // Set Customers as Landing item for Administration Menu
            return RedirectToAction("Users", "Administration");
        }

        [HttpGet]
        public ActionResult Users()
        {
            return View();
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxUsersGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;
            try
            {
                var users = Mapper.Map<List<UserViewModel>>(_userManager.Users);
                var regions = _regionTypeRefDataService.All().ToList();
                foreach (var user in users)
                {
                    var regionTypeRefData = regions.FirstOrDefault(x => x.Id == user.RegionTypeId);
                    if (regionTypeRefData != null) user.RegionName = regionTypeRefData.RegionName;
                }
                result = users.ToDataSourceResult(request);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
            }

            return Json(result);
        }

        [OutputCache(Duration = 0)]
        public async Task<ActionResult> DeleteAjaxUsersGrid([DataSourceRequest] DataSourceRequest request, UserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return Json(new DataSourceResult { Errors = WebResources.UserCannotFindUser });
                }

                // Check whether Architect is still an owner of a Customer
                if (_customerService.IsArchitectACustomerOwner(user.Email))
                {
                    ModelState.AddModelError(ModelStateErrorNames.ArchitectIsStillCustomerOwner,
                        WebResources.ArchitectIsStillCustomerOwner);
                }
                else
                {
                    // Delete user logins.
                    var logins = user.Logins
                        .ToList();
                    foreach (var login in logins)
                    {
                        var removeLoginIdentityResult = await _userManager.RemoveLoginAsync(login.UserId,
                            new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                        if (!removeLoginIdentityResult.Succeeded)
                        {
                            return Json(new DataSourceResult { Errors = removeLoginIdentityResult.Errors });
                        }
                    }

                    // Delete user roles
                    var rolesForUser = await _userManager.GetRolesAsync(user.Id);
                    foreach (var roleForUser in rolesForUser)
                    {
                        var removeRoleIdentityResult = await _userManager.RemoveFromRoleAsync(user.Id, roleForUser);
                        if (!removeRoleIdentityResult.Succeeded)
                        {
                            return Json(new DataSourceResult { Errors = removeRoleIdentityResult.Errors });
                        }
                    }

                    // Delete user.
                    var deleteUserIdentityResult = await _userManager.DeleteAsync(user);
                    if (!deleteUserIdentityResult.Succeeded)
                    {
                        return Json(new DataSourceResult { Errors = deleteUserIdentityResult.Errors });
                    }

                    // Delete contributors
                    _contributorService.DeleteUserContributors(user.Id);

                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                return Json(new DataSourceResult { Errors = new[] { ex.Message } });
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new DataSourceResult { Errors = WebResources.UserCannotFindUser });
                }

                // Check whether Architect is still an owner of a Customer
                if (_customerService.IsArchitectACustomerOwner(user.Email))
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = WebResources.ArchitectIsStillCustomerOwner });
                }

                // Delete user logins.
                var logins = user.Logins.ToList();
                foreach (var login in logins)
                {
                    var removeLoginIdentityResult = await _userManager.RemoveLoginAsync(login.UserId,
                        new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    if (!removeLoginIdentityResult.Succeeded)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        return Json(new DataSourceResult { Errors = removeLoginIdentityResult.Errors });
                    }
                }

                // Delete user roles
                var rolesForUser = await _userManager.GetRolesAsync(user.Id);
                foreach (var roleForUser in rolesForUser)
                {
                    var removeRoleIdentityResult = await _userManager.RemoveFromRoleAsync(user.Id, roleForUser);
                    if (!removeRoleIdentityResult.Succeeded)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        return Json(new DataSourceResult { Errors = removeRoleIdentityResult.Errors });
                    }
                }

                // Delete user
                var deleteUserIdentityResult = await _userManager.DeleteAsync(user);
                if (!deleteUserIdentityResult.Succeeded)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = deleteUserIdentityResult.Errors });
                }

                // Delete contributors
                _contributorService.DeleteUserContributors(user.Id);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", Message = ex.Message };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Roles(string role)
        {
            // Note this will return an IQueryable list of Roles.
            var model = new RoleViewModel
            {
                RoleName = role,
            };

            return View(model);
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxUserRolesGrid([DataSourceRequest] DataSourceRequest request, string role)
        {
            DataSourceResult result = null;
            try
            {
                var applicationRoleId = _roleManager.FindByName(role).Id;
                result = _userManager.Users.Where(u => u.Roles.Any(x => x.RoleId == applicationRoleId)).ToDataSourceResult(request, Mapper.Map<ApplicationUser, UserViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteUserRolesGrid([DataSourceRequest] DataSourceRequest request, UserViewModel model, string role)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    // Maintain Role Hierarchy
                    switch (role)
                    {
                        case UserRoles.Architect:
                            RemoveRole(model.Id, UserRoles.Administrator);
                            break;
                        case UserRoles.Viewer:
                            RemoveRole(model.Id, UserRoles.Administrator);
                            RemoveRole(model.Id, UserRoles.Architect);
                            break;
                    }

                    // Remove the role
                    _userManager.RemoveFromRole(model.Id, role);

                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, ExceptionPolicies.General);
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        private void RemoveRole(string userId, string role)
        {
            if (_userManager.IsInRole(userId, role))
            {
                _userManager.RemoveFromRole(userId, role);
            }
        }

        [HttpGet]
        public ActionResult AddUserToRole(string role)
        {
            var model = new RoleViewModel
            {
                RoleName = role,
            };

            return PartialView("_AddUserToRole", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 0)]
        public JsonResult AddUserToRole(RoleViewModel model)
        {
            var result = new { Success = "True", Message = "Success" };

            if (ModelState.IsValid)
            {
                try
                {
                    // Maintain Role Hierarchy
                    switch (model.RoleName)
                    {
                        case UserRoles.Administrator:
                            AddRole(model.UserId, UserRoles.Architect);
                            AddRole(model.UserId, UserRoles.Viewer);
                            break;
                        case UserRoles.Architect:
                            AddRole(model.UserId, UserRoles.Viewer);
                            break;
                    }

                    // Add the role
                    _userManager.AddToRole(model.UserId, model.RoleName);
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


        private void AddRole(string userId, string role)
        {
            if (!_userManager.IsInRole(userId, role))
            {
                _userManager.AddToRole(userId, role);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public JsonResult GetUsers(string role)
        {
            var applicationRoleId = _roleManager.FindByName(role).Id;
            var users =
                _userManager.Users.Where(u => u.Roles.All(x => x.RoleId != applicationRoleId))
                    .Select(x => new SelectListItem()
                    {
                        Value = x.Id,
                        Text = x.UserName,
                    }).ToList();

            users.Add(new SelectListItem()
            {
                Value = "0",
                Text = WebResources.DefaultDropDownListText,
            });

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public JsonResult GetUsersInRole(string role)
        {
            var applicationRoleId = _roleManager.FindByName(role).Id;
            var users = _userManager.Users
                .Where(u => string.IsNullOrEmpty(role) ||
                    u.Roles.Any(y => y.RoleId == applicationRoleId))
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.UserName
                })
                .ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        // GET: /Administration/Parameters
        public ViewResult Parameters()
        {
            var model = new ParameterViewModel();
            return View(model);
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxParametersGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;
            try
            {
                var parameters = _parameterService
                    .All()
                    .ToList();

                var parameterViewModels = Mapper.Map<List<Parameter>, List<ParameterViewModel>>(parameters);

                result = parameterViewModels.ToDataSourceResult(request);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        public ActionResult UpdateAjaxParametersGrid([DataSourceRequest]DataSourceRequest request, ParameterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var parameter = _parameterService.Find(viewModel.ParameterName);

                    // Before updating, check that the parameter is not a System type. This cannot be updated from the UI.
                    if (parameter.Type == ParameterType.System)
                    {
                        ModelState.AddModelError(ModelStateErrorNames.ParameterIsSystem, WebResources.ParametersParameterIsSystem);
                    }
                    else
                    {
                        _parameterService.SaveParameter(viewModel.ParameterName, viewModel.ParameterValue, parameter.Type);
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                }
            }

            // Return the updated parameter. Also return any validation errors.
            return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        public PartialViewResult SelectUser(string roleName)
        {
            return PartialView("_SelectUser", new SelectUserViewModel { RoleName = roleName });
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public async Task<JsonResult> LockUser(LockUserViewModel model)
        {
            var result = new { Success = true, Message = "Success" };

            if (ModelState.IsValid)
            {
                try
                {
                    // Get current user.
                    var user = await _userManager.FindByIdAsync(model.UserId);

                    user.LockoutEndDateUtc = model.Lock ? DateTime.MaxValue : DateTime.UtcNow;
                    user.LastLoginUtc = DateTime.UtcNow;

                    var identityResult = await _userManager.UpdateAsync(user);

                    if (!identityResult.Succeeded)
                    {
                        result = new { Success = false, Message = identityResult.Errors.First() };
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                    result = new { Success = false, ex.Message };
                }
            }
            else
            {
                var errors = ModelState.GetModelStateMesssages();
                result = new { Success = false, Message = errors.First() };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public async Task<JsonResult> ResetUserPassword(ResetUserPasswordViewModel model)
        {
            var result = new { Success = true, Message = "Success" };

            if (ModelState.IsValid)
            {
                try
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(model.UserId);
                    var defaultPassword = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.UserResetPassword);
                    var identityResult = await _userManager.ResetPasswordAsync(model.UserId, resetToken, defaultPassword);
                    if (!identityResult.Succeeded)
                    {
                        result = new { Success = false, Message = identityResult.Errors.First() };
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, result.Message);
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                    result = new { Success = false, ex.Message };
                }
            }
            else
            {
                var errors = ModelState.GetModelStateMesssages();
                result = new { Success = false, Message = errors.First() };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Videos()
        {
            return View();
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxVideosGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;
            try
            {
                result = _assetService.All().ToDataSourceResult(request, Mapper.Map<Asset, AssetViewModel>);

            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult DownloadAsset(int id)
        {
            var asset = _assetService.GetById(id);
            var contentDisposition = new ContentDisposition
            {
                FileName = $"{asset.OriginalFileName}{asset.FileExtension}",
                Inline = false
            };
            _contextManager.ResponseManager.AppendHeader("Content-Disposition", contentDisposition.ToString());


            return new FilePathResult(asset.FullPath, asset.MimeType);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteAsset(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                if (_assetService.GetNumberOfAssetReferences(id) > 0)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Video") });
                }

                var asset = _assetService.GetById(id);
                _assetService.Delete(asset);
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
        public ActionResult Regions()
        {
            return View();
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxRegionsRefDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;
            try
            {
                var regions = _regionTypeRefDataService.All().OrderBy(o => o.SortOrder).ThenBy(o => o.RegionName).Select(Mapper.Map<RegionTypeRefDataListItem>).ToList();
                foreach (var region in regions)
                {
                    region.UsageCount = _userManager.Users.Count(x => x.RegionType != null && x.RegionType.Id == region.Id);
                }

                result = regions.ToDataSourceResult(request, Mapper.Map<RegionTypeRefDataListItem, RegionTypeRefDataViewModel>);

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
        public ActionResult CreateAjaxRegionsRefDataGrid([DataSourceRequest]DataSourceRequest request, RegionTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_regionTypeRefDataService.All().Any(c => string.Equals(c.RegionName, model.RegionTypeName.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.EntityNameIsNotUnique, "Region"));
                    }
                    else
                    {
                        _regionTypeRefDataService.Create(Mapper.Map<RegionTypeRefDataViewModel, RegionTypeRefData>(model));
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
        public ActionResult UpdateAjaxRegionsRefDataGrid([DataSourceRequest]DataSourceRequest request, RegionTypeRefDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update
                    var region = _regionTypeRefDataService.GetById(model.Id);
                    if (!string.Equals(region.RegionName, model.RegionTypeName.Trim(), StringComparison.CurrentCultureIgnoreCase) && _regionTypeRefDataService.All().Any(c => string.Equals(c.RegionName, model.RegionTypeName.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader("HandledError", string.Format(WebResources.EntityNameIsNotUnique, "Region"));
                    }
                    else
                    {
                        region.RegionName = model.RegionTypeName;
                        region.SortOrder = model.SortOrder;
                        _regionTypeRefDataService.Update(region);
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
        public ActionResult DeleteRegion(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                if (_userManager.Users.Any(x => x.RegionType != null && x.RegionType.Id == id))
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    return Json(new DataSourceResult { Errors = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Region") });
                }

                var region = _regionTypeRefDataService.GetById(id);
                _regionTypeRefDataService.Delete(region);
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