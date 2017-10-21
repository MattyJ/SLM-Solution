using AutoMapper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.TemplateProcessors.Interface;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Helpers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IServiceDecompositionTemplateDataImportProcessor _serviceDecompositionTemplateDataImportProcessor;
        private readonly IServiceDecompositionDesignDataImportProcessor _serviceDecompositionDesignDataImportProcessor;
        private readonly ITemplateProcessor _templateProcessor;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        private const string MsExcelMimeType = "application/vnd.ms-excel";
        private const string ModelStateErrorKey = "SpreadsheetFile";

        public TemplateController(ITemplateService templateService,
            IServiceDeskService serviceDeskService,
            IServiceDecompositionTemplateDataImportProcessor serviceDecompositionTemplateDataImportProcessor,
            IServiceDecompositionDesignDataImportProcessor serviceDecompositionDesignDataImportProcessor,
            ITemplateProcessor templateProcessor,
            IContextManager contextManager,
            IAppUserContext appUserContext)
        {
            if (templateService == null)
            {
                throw new ArgumentNullException(nameof(templateService));
            }
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }
            if (serviceDecompositionTemplateDataImportProcessor == null)
            {
                throw new ArgumentNullException(nameof(serviceDecompositionTemplateDataImportProcessor));
            }
            if (serviceDecompositionDesignDataImportProcessor == null)
            {
                throw new ArgumentNullException(nameof(serviceDecompositionDesignDataImportProcessor));
            }
            if (templateProcessor == null)
            {
                throw new ArgumentNullException(nameof(templateProcessor));
            }
            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }
            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            _templateService = templateService;
            _serviceDeskService = serviceDeskService;
            _serviceDecompositionTemplateDataImportProcessor = serviceDecompositionTemplateDataImportProcessor;
            _serviceDecompositionDesignDataImportProcessor = serviceDecompositionDesignDataImportProcessor;
            _templateProcessor = templateProcessor;
            _contextManager = contextManager;
            _appUserContext = appUserContext;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxTemplateGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;
            try
            {
                result = _templateService.AllTemplates()
                    .ToDataSourceResult(request, Mapper.Map<TemplateListItem, TemplateViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult DeleteAjaxTemplateGrid([DataSourceRequest] DataSourceRequest request, TemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var template = _templateService.GetById(model.Id);
                    if (template != null)
                    {
                        _templateService.Delete(template);
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

        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult DownloadTemplate(int id)
        {
            var template = _templateService.GetById(id);

            // Create the content disposition
            var contentDisposition = new ContentDisposition
            {
                FileName = template.Filename,
                Inline = false
            };

            // Add the header and return the stream
            Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

            return new FileStreamResult(new MemoryStream(template.TemplateData), MsExcelMimeType);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult ImportTemplateSpreadsheet(string templateType)
        {
            var model = new ImportTemplateViewModel { TemplateType = templateType };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult ImportTemplateSpreadsheet(ImportTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var folder = SettingsHelper.ImportSpreadsheetFileLocation;

                // If the folder doesn't exist then create
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var originalFullFileName = model.SpreadsheetFile.FileName;
                var newFileName = Guid.NewGuid() + Path.GetExtension(originalFullFileName);
                var newfullFileName = Path.Combine(folder, newFileName);

                try
                {
                    // Save the file to the file location, need to do it here rather than in the Process method (below) otherwise we get a file not found exception.
                    model.SpreadsheetFile.SaveAs(newfullFileName);

                    _serviceDecompositionTemplateDataImportProcessor.Execute(newfullFileName, originalFullFileName, model.TemplateType.GetEnumIntFromText<TemplateType>());

                    // Success redirect back to the Templates
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Guid errorGuid;
                    ModelState.AddModelError(key: ModelStateErrorKey, errorMessage: Guid.TryParse(ex.Message, out errorGuid) ? string.Format(WebResources.ErrorMessage, errorGuid) : ex.Message);
                }
                finally
                {
                    // Tidy up the file if it has not been deleted
                    if (System.IO.File.Exists(newfullFileName))
                    {
                        System.IO.File.Delete(newfullFileName);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult AddServiceConfiguratorTemplate(string level, int id)
        {
            var vm = new AddTemplateServiceDomainViewModel
            {
                ServiceDeskId = id,
                EditLevel = level,
                HasServiceDeskContext = id != 0,
                TemplateType = TemplateTypeNames.SORT
            };

            return Add(vm);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult AddSLMTemplate(string level, int id)
        {
            var vm = new AddTemplateServiceDomainViewModel
            {
                ServiceDeskId = id,
                EditLevel = level,
                HasServiceDeskContext = id != 0,
                TemplateType = TemplateTypeNames.SLM
            };

            return Add(vm);
        }

        private ActionResult Add(AddTemplateServiceDomainViewModel vm)
        {
            vm.ReturnUrl = vm.HasServiceDeskContext
                ? Url.Action("Edit", "ServiceDesk", new { level = vm.EditLevel, id = vm.ServiceDeskId })
                : Url.Action("Index", "ServiceDomain", new { level = vm.EditLevel, id = vm.ServiceDeskId });

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var customerId = _appUserContext.Current.CurrentCustomer.Id;
                var serviceDesks = _serviceDeskService.GetByCustomer(customerId).ToList();

                if (vm.ServiceDeskId > 0)
                {
                    vm.ServiceDeskName =
                        serviceDesks.First(x => x.Id == vm.ServiceDeskId).DeskName;
                }
                else if (serviceDesks.Count == 1)
                {
                    vm.ServiceDeskId = serviceDesks.First().Id;
                    vm.ServiceDeskName = serviceDesks.First().DeskName;
                    vm.HasServiceDeskContext = true;
                }
                else
                {
                    vm.ServiceDesks.AddRange(serviceDesks.Select(s => new SelectListItem { Text = s.DeskName, Value = s.Id.ToString(CultureInfo.InvariantCulture) }).ToList());
                }
            }

            return View("Add" + vm.EditLevel, vm);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ImportServiceDecompositionSpreadsheet(string level, int id)
        {
            var vm = new ImportServiceDecompositionSpreadsheetViewModel
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
                    vm.ServiceDeskId = serviceDesks.First().Id;
                    vm.ServiceDeskName = serviceDesks.First().DeskName;
                    vm.HasServiceDeskContext = true;
                }
                else
                {
                    vm.ServiceDesks.AddRange(serviceDesks.Select(s => new SelectListItem { Text = s.DeskName, Value = s.Id.ToString(CultureInfo.InvariantCulture) }).ToList());
                }
            }

            return View("ImportServiceDecompositionSpreadsheet" + level, vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult ImportServiceDecompositionSpreadsheet(ImportServiceDecompositionSpreadsheetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var folder = SettingsHelper.ImportSpreadsheetFileLocation;

                // If the folder doesn't exist then create
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var originalFullFileName = model.SpreadsheetFile.FileName;
                var newFileName = Guid.NewGuid() + Path.GetExtension(originalFullFileName);
                var newfullFileName = Path.Combine(folder, newFileName);

                try
                {
                    model.SpreadsheetFile.SaveAs(newfullFileName);
                    _serviceDecompositionDesignDataImportProcessor.Execute(newfullFileName, originalFullFileName, model.ServiceDeskId);
                    return Redirect(model.ReturnUrl);
                }
                catch (Exception ex)
                {
                    Guid errorGuid;
                    ModelState.AddModelError(key: ModelStateErrorKey, errorMessage: Guid.TryParse(ex.Message, out errorGuid) ? string.Format(WebResources.ErrorMessage, errorGuid) : ex.Message);
                }
                finally
                {
                    // Tidy up the file if it has not been deleted
                    if (System.IO.File.Exists(newfullFileName))
                    {
                        System.IO.File.Delete(newfullFileName);
                    }
                }
            }

            return View(model);
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxTemplateDomainGrid([DataSourceRequest] DataSourceRequest request, string templateType)
        {
            DataSourceResult result = null;

            try
            {
                result = _templateService.AllTemplateDomains(templateType)
                    .ToDataSourceResult(request, Mapper.Map<TemplateDomainListItem, TemplateDomainViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader("ErrorMessage", ex.Message);
            }

            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(Duration = 0)]
        public ActionResult UpdateAjaxTemplateDomainGrid([DataSourceRequest] DataSourceRequest request,
                                    [Bind(Prefix = "models")] List<TemplateDomainViewModel> updatedTemplateDomains,
                                    string level)
        {
            if (updatedTemplateDomains != null && ModelState.IsValid)
            {
                try
                {
                    var deskId = updatedTemplateDomains.First(x => x.Selected).ServiceDeskId;
                    if (deskId != null)
                    {
                        _templateProcessor.Execute(deskId.Value, Mapper.Map<List<TemplateDomainListItem>>(updatedTemplateDomains));
                    }
                }
                catch (Exception ex)
                {
                    _contextManager.ResponseManager.StatusCode = 500;
                    _contextManager.ResponseManager.AppendHeader("ErrorMessage", ex.Message);
                }
            }

            return Json(updatedTemplateDomains.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        public ActionResult Template(int id)
        {
            var template = _templateService.GetById(id);

            var model = new TemplateViewModel
            {
                Id = id,
                Filename = template.Filename,
                TemplateType = template.TemplateType.ToEnumText<TemplateType>()
            };

            return View(model);
        }

        [OutputCache(Duration = 0)]
        public ActionResult ReadAjaxTemplateRowGrid([DataSourceRequest] DataSourceRequest request, int templateId)
        {
            DataSourceResult result = null;

            try
            {
                result = _templateService.GetTemplateRows(templateId)
                    .ToDataSourceResult(request, Mapper.Map<TemplateRowListItem, TemplateRowViewModel>);
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader("ErrorMessage", ex.Message);
            }

            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult Delete(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var template = _templateService.GetById(id);
                _templateService.Delete(template);
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