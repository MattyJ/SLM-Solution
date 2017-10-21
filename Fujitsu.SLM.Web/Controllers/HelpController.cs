using AutoMapper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Helpers;
using Fujitsu.SLM.Web.Models;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [AllowAnonymous]
    [OutputCache(Duration = 0)]
    public class HelpController : Controller
    {
        private readonly IContextHelpRefDataService _contextHelpRefDataService;
        private readonly IRepository<Asset> _assetRepository;
        private readonly IContextManager _contextManager;

        public HelpController(IContextHelpRefDataService contextHelpRefDataService,
            IRepository<Asset> assetRepository,
            IContextManager contextManager)
        {
            if (contextHelpRefDataService == null)
            {
                throw new ArgumentNullException(nameof(contextHelpRefDataService));
            }

            if (assetRepository == null)
            {
                throw new ArgumentNullException(nameof(assetRepository));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            _contextHelpRefDataService = contextHelpRefDataService;
            _assetRepository = assetRepository;
            _contextManager = contextManager;
        }

        [HttpGet]
        public ActionResult Index(string helpKey)
        {
            // Check if we have help in the database
            var dbHelp = _contextHelpRefDataService.GetByHelpKey(helpKey);

            // Create the model for the help page
            var helpModel = new ContextHelpRefDataViewModel
            {
                HelpText = "<p>There is no help available for this page</p>"
            };

            // If we have help the use the text
            if (dbHelp != null)
            {
                helpModel.HelpText = dbHelp.HelpText;
                helpModel.Key = dbHelp.Key;
                helpModel.Title = dbHelp.Title;
                if (dbHelp.Asset != null)
                {
                    helpModel.HelpVideoTitle = dbHelp.Asset.OriginalFileName;
                    helpModel.HelpVideoFileName = dbHelp.Asset.FileName;
                    helpModel.HelpVideoFileExtension = dbHelp.Asset.FileExtension.Substring(1);
                }
            }
            else
            {
                helpModel.Key = helpKey;
                helpModel.Title = string.Empty;

                // Create Help
                var help = Mapper.Map<ContextHelpRefDataViewModel, ContextHelpRefData>(helpModel);
                _contextHelpRefDataService.Create(help);

                helpModel.Id = help.Id;
            }

            return View(helpModel);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Edit(string helpKey)
        {
            // Check if we have help in the database
            var dbHelp = _contextHelpRefDataService.GetByHelpKey(helpKey);

            if (dbHelp == null)
            {
                return RedirectToAction("Index", helpKey);
            }

            var helpModel = new ContextHelpRefDataViewModel
            {
                Id = dbHelp.Id,
                HelpText = dbHelp.HelpText,
                Key = dbHelp.Key,
                Title = dbHelp.Title,
            };

            if (dbHelp.Asset != null)
            {
                helpModel.HelpVideoTitle = dbHelp.Asset.OriginalFileName;
                helpModel.HelpVideoFileName = dbHelp.Asset.FileName;
                helpModel.HelpVideoFileExtension = dbHelp.Asset.FileExtension.Substring(1);
            }

            return View(helpModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator)]
        public ActionResult Edit(ContextHelpRefDataViewModel model, string button)
        {
            if (model != null && ModelState.IsValid)
            {
                if (button.ToLower().Equals("update"))
                {
                    var help = _contextHelpRefDataService.GetById(model.Id);
                    help.Title = model.Title;
                    help.HelpText = model.HelpText;

                    // Update Help
                    if (model.HelpVideoFile != null)
                    {
                        var folder = SettingsHelper.HelpVideoFileLocation;
                        var fullPath = string.Empty;
                        try
                        {
                            // If the folder doesn't exist then create
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }

                            var originalFullFileName = model.HelpVideoFile.FileName;
                            var fileName = Guid.NewGuid();
                            var fileExtension = Path.GetExtension(originalFullFileName);
                            fullPath = Path.Combine(folder, fileName + fileExtension);

                            model.HelpVideoFile.SaveAs(fullPath);
                            var asset = new Asset
                            {
                                OriginalFileName = Path.GetFileNameWithoutExtension(originalFullFileName),
                                FileName = fileName.ToString(),
                                FileExtension = fileExtension,
                                FullPath = fullPath,
                                MimeType = MimeMapping.GetMimeMapping(fullPath),
                                ContextHelpRefDatas = new List<ContextHelpRefData>
                                {
                                    help
                                }

                            };

                            _assetRepository.Insert(asset);
                            help.Asset = asset;
                            _contextHelpRefDataService.Update(help);
                        }
                        catch (Exception)
                        {
                            // Tidy up the file if it has not been deleted
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }

                            throw;
                        }
                    }
                    else
                    {
                        _contextHelpRefDataService.Update(help);
                    }
                }
            }


            return RedirectToAction("Index", "Help", new { helpKey = model?.Key ?? "homepage" });
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult DeleteHelpVideo(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var help = _contextHelpRefDataService.GetById(id);
                if (help.Asset != null)
                {
                    var fullPath = help.Asset.FullPath;
                    var assetId = help.AssetId;
                    help.Asset = null;
                    _assetRepository.Delete(assetId);
                    _contextHelpRefDataService.Update(help);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                }
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