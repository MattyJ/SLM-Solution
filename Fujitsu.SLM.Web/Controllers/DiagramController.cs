using AutoMapper;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Services.Interfaces;
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
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using Diagram = Fujitsu.SLM.Constants.Diagram;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize(Roles = UserRoles.Viewer)]
    public class DiagramController : Controller
    {
        private readonly IDiagramService _diagramService;
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IAppUserContext _appUserContext;
        private readonly IContextManager _contextManager;
        private readonly IUserIdentity _userIdentity;
        private readonly IObjectBuilder _objectBuilder;

        public DiagramController(IDiagramService diagramService,
            IServiceDeskService serviceDeskService,
            IAppUserContext appUserContext,
            IContextManager contextManager,
            IUserIdentity userIdentity,
            IObjectBuilder objectBuilder)
        {

            if (diagramService == null)
            {
                throw new ArgumentNullException(nameof(diagramService));
            }

            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }

            _diagramService = diagramService;
            _serviceDeskService = serviceDeskService;
            _appUserContext = appUserContext;
            _contextManager = contextManager;
            _userIdentity = userIdentity;
            _objectBuilder = objectBuilder;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            var vm = new DiagramGridViewModel
            {
                EditLevel = level
            };
            return View(level, vm);
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult ReadAjaxDiagramGrid([DataSourceRequest] DataSourceRequest request, string level)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var l = level.GetEnumIntFromText<LevelName>();
                    result = _diagramService
                        .GetByCustomerId(_appUserContext.Current.CurrentCustomer.Id)
                        .Where(w => w.Level == l)
                        .ToDataSourceResult(request, Mapper.Map<Model.Diagram, DiagramViewModel>);
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
        public ActionResult UpdateAjaxDiagramGrid([DataSourceRequest] DataSourceRequest request, DiagramViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_appUserContext.Current?.CurrentCustomer != null &&
                        _appUserContext.Current.CurrentCustomer.Id > 0)
                    {
                        var diagram = _diagramService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id,
                            model.Id);
                        if (diagram != null)
                        {
                            diagram.DiagramNotes = model.DiagramNotes;
                            diagram.Filename = model.Filename;
                            diagram.UpdatedDate = DateTime.Now;
                            diagram.UpdatedBy = _contextManager.UserManager.Name;
                            _diagramService.Update(diagram);
                        }
                        else
                        {
                            ModelState.AddModelError(ModelStateErrorNames.DiagramCannotBeFound,
                                WebResources.DiagramCannotBeFound);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(ModelStateErrorNames.DiagramCannotBeFound,
                            WebResources.DiagramCannotBeFound);
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
        public ActionResult DeleteAjaxDiagramGrid([DataSourceRequest] DataSourceRequest request, DiagramViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var diagram = _diagramService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id,
                        model.Id);

                    if (diagram == null)
                    {
                        _contextManager.ResponseManager.StatusCode = 500;
                        _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage,
                            WebResources.DiagramCannotBeFound);
                    }
                    else
                    {
                        _diagramService.Delete(diagram);
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
        public ActionResult Delete(int id)
        {
            var result = new { Success = "True", Message = "Success" };
            try
            {
                var diagram = _diagramService.GetById(id);
                _diagramService.Delete(diagram);
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
        public ActionResult DownloadDiagram(int id)
        {
            ActionResult actionResult = null;
            try
            {
                RetryableOperation.Invoke(ExceptionPolicies.General, () =>
                {
                    var diagram = _diagramService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id, id);
                    if (diagram != null)
                    {
                        var contentDisposition = new ContentDisposition
                        {
                            FileName = diagram.Filename,
                            Inline = false,
                        };
                        _contextManager.ResponseManager.AppendHeader("Content-Disposition",
                            contentDisposition.ToString());
                        actionResult = new FileStreamResult(new MemoryStream(diagram.DiagramData), diagram.MimeType);
                    }
                });
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }
            return actionResult;
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult SaveDiagram(string dataUri, string filename, int level)
        {
            var result = new { Success = "True", Message = "Success" };

            // Data URI format
            // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
            // Data URI Example
            // data:application/pdf;base64,JVBERi0xLjQKJcLB2s.....

            const string mimeTypeMarker = "data:";
            var mimeTypeIndex = dataUri.IndexOf(mimeTypeMarker, System.StringComparison.Ordinal) + mimeTypeMarker.Length;
            var mimeType = dataUri.Substring(mimeTypeIndex,
                dataUri.IndexOf(";", System.StringComparison.Ordinal) - mimeTypeIndex);

            const string base64Marker = ";base64,";
            var base64Index = dataUri.IndexOf(base64Marker, System.StringComparison.Ordinal) + base64Marker.Length;
            var base64 = dataUri.Substring(base64Index);

            try
            {
                // RetryableOperation to Log Unexpected Error and Alert Error Instance id.
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        var userName = _contextManager.UserManager.Name;
                        var dateTime = DateTime.Now;

                        var diagram = new Model.Diagram
                        {
                            CustomerId = _appUserContext.Current.CurrentCustomer.Id,
                            Level = level,
                            MimeType = mimeType,
                            DiagramData = Convert.FromBase64String(base64),
                            Filename = filename,
                            InsertedBy = userName,
                            InsertedDate = dateTime,
                            UpdatedBy = userName,
                            UpdatedDate = dateTime,
                        };

                        _diagramService.Create(diagram);
                    });
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
                result = new { Success = "False", ex.Message };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #region Service Desk Structure Diagram

        private IEnumerable<TreeViewItemModel> GetTreeViewDomainData(int serviceDeskId)
        {
            var domainData = new List<TreeViewItemModel>
            {
                new TreeViewItemModel
                {
                    Id = "0",
                    Text = "Service Domains",
                    Checked = true,
                    Expanded = true,
                }
            };

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var serviceDesk = _serviceDeskService.GetByCustomerAndId(_appUserContext.Current.CurrentCustomer.Id,
                    serviceDeskId);
                if (serviceDesk != null)
                {
                    foreach (var domain in serviceDesk.ServiceDomains)
                    {

                        domainData[0].Items.Add(new TreeViewItemModel
                        {
                            Id = domain.Id.ToString(),
                            Text = domain.AlternativeName ?? domain.DomainType.DomainName,
                            Checked = true,
                        });
                    }
                }
            }

            return domainData;
        }

        [HttpGet]
        public ActionResult ServiceDeskStructure(int level)
        {
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var serviceDesks = _serviceDeskService.GetByCustomer(_appUserContext.Current.CurrentCustomer.Id);
                if (serviceDesks.Count() == 1)
                {
                    // Go straight to the rendering the diagram
                    return RedirectToAction("ServiceDeskStructureDiagram", new { level, id = serviceDesks.First().Id });
                }
            }

            return View(level);
        }

        [HttpGet]
        public ActionResult ServiceDeskStructureDiagram(int level, int id)
        {
            var levelName = level == 1 ? LevelNames.LevelOne : LevelNames.LevelTwo;
            var chartModel = new ChartViewModel
            {
                Title =
                    $"{_appUserContext.Current.CurrentCustomer.CustomerName} {levelName} {Diagram.ServiceDeskStructureTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{levelName} {Diagram.ServiceDeskStructureTitle}",
                Filename = $"{levelName} {Diagram.ServiceDeskStructureTitle}",
                Level = level,
                Id = id,
                ServiceDomains = true,
                ServiceFunctions = true,
                ServiceComponents = true,
                Resolvers = true,
                ServiceActivities = false,
                InlineDomainData = GetTreeViewDomainData(id),
            };

            return View(chartModel);
        }


        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadServiceDeskStructureChart(int id,
            bool svcDomains,
            string[] domainsSelected,
            bool svcFunctions,
            bool svcComponents,
            bool resolverGroups,
            bool svcActivities,
            bool opProcs)
        {
            var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.ServiceDeskStructure);
            var chartData = generator.Generate(id, svcDomains, svcFunctions, svcComponents, resolverGroups,
                svcActivities, opProcs, domainsSelected);
            var chart = Mapper.Map<List<ChartDataViewModel>>(chartData);

            return Json(chart, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Dot Matrix Diagram

        [HttpGet]
        public ActionResult DotMatrix(int level)
        {
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var serviceDesks = _serviceDeskService.GetByCustomer(_appUserContext.Current.CurrentCustomer.Id);
                if (serviceDesks.Count() == 1)
                {
                    // Go straight to the rendering the diagram
                    return RedirectToAction("DotMatrixDiagram", new { level, id = serviceDesks.First().Id });
                }
            }

            return View(level);
        }

        [HttpGet]
        public ActionResult DotMatrixDiagram(int level, int id)
        {
            var levelName = level == 1 ? LevelNames.LevelOne : LevelNames.LevelTwo;
            var chartModel = new ChartViewModel
            {
                Title =
                    $"{_appUserContext.Current.CurrentCustomer.CustomerName} {levelName} {Diagram.ServiceDeskDotMatrixTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{levelName} {Diagram.ServiceDeskDotMatrixTitle}",
                Filename = $"{levelName} {Diagram.ServiceDeskDotMatrixTitle}",
                Level = level,
                Id = id
            };

            return View(chartModel);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadServiceDeskDotMatrixChart(int id)
        {
            var chart = new List<ChartDataViewModel>();

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.ServiceDeskDotMatrix);
                chart = Mapper.Map<List<ChartDataViewModel>>(generator.Generate(id));
            }

            return Json(chart);
        }

        #endregion

        #region Fujitsu Domains Overview Diagram

        [HttpGet]
        public ActionResult FujitsuDomains(int level)
        {
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var serviceDesks = _serviceDeskService.GetByCustomer(_appUserContext.Current.CurrentCustomer.Id);
                if (serviceDesks.Count() == 1)
                {
                    // Go straight to the rendering the diagram
                    return RedirectToAction("FujitsuDomainsDiagram", new { level, id = serviceDesks.First().Id });
                }
            }

            return View(level);
        }

        [HttpGet]
        public ActionResult FujitsuDomainsDiagram(int level, int id)
        {
            var chartModel = new ChartViewModel
            {
                Title = $"{_appUserContext.Current.CurrentCustomer.CustomerName} {LevelNames.LevelZero} {Diagram.FujitsuDomainsTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{LevelNames.LevelZero} {Diagram.FujitsuDomainsTitle}",
                Filename = $"{LevelNames.LevelZero} {Diagram.FujitsuDomainsTitle}",
                Level = level,
                Id = id
            };

            return View(chartModel);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadFujitsuDomainsChart(int id)
        {
            var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.FujitsuDomains);
            var chart = Mapper.Map<List<ChartDataViewModel>>(generator.Generate(id));

            return Json(chart, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Customer Services Overview Diagram

        [HttpGet]
        public ActionResult CustomerServices(int level)
        {
            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var serviceDesks = _serviceDeskService.GetByCustomer(_appUserContext.Current.CurrentCustomer.Id);
                if (serviceDesks.Count() == 1)
                {
                    // Go straight to the rendering the diagram
                    return RedirectToAction("CustomerServicesDiagram", new { level, id = serviceDesks.First().Id });
                }
            }

            return View(level);
        }

        [HttpGet]
        public ActionResult CustomerServicesDiagram(int level, int id)
        {
            var chartModel = new ChartViewModel
            {
                Title = $"{_appUserContext.Current.CurrentCustomer.CustomerName} {LevelNames.LevelZero} {Diagram.CustomerServicesTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{LevelNames.LevelZero} {Diagram.CustomerServicesTitle}",
                Filename = $"{LevelNames.LevelZero} {Diagram.CustomerServicesTitle}",
                Level = level,
                Id = id
            };

            return View(chartModel);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadCustomerServicesChart(int id)
        {
            var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.CustomerServices);
            var chart = Mapper.Map<List<ChartDataViewModel>>(generator.Generate(id));

            return Json(chart, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Service Organisation Diagram

        [HttpGet]
        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult ServiceOrganisation(int level, string type)
        {
            var model = new ServiceOrganisationViewModel
            {
                Level = level,
                OrganisationType = type
            };

            if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
            {
                var serviceDesks = _serviceDeskService.GetByCustomer(_appUserContext.Current.CurrentCustomer.Id);
                if (serviceDesks.Count() == 1)
                {
                    // Go straight to the rendering the diagram
                    if (type.IsCaseInsensitiveEqual(Diagram.FujitsuServiceOrganisation))
                    {
                        return RedirectToAction("FujitsuServiceOrganisationDiagram", new { level, id = serviceDesks.First().Id });
                    }

                    if (type.IsCaseInsensitiveEqual(Diagram.CustomerServiceOrganisation))
                    {
                        return RedirectToAction("CustomerServiceOrganisationDiagram", new { level, id = serviceDesks.First().Id });
                    }

                    if (type.IsCaseInsensitiveEqual(Diagram.CustomerThirdPartyServiceOrganisation))
                    {
                        return RedirectToAction("CustomerThirdPartyServiceOrganisationDiagram", new { level, id = serviceDesks.First().Id });
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult FujitsuServiceOrganisationDiagram(int level, int id)
        {
            var levelName = level == 1 ? LevelNames.LevelOne : LevelNames.LevelTwo;
            var chartModel = new ChartViewModel
            {
                Title = $"{_appUserContext.Current.CurrentCustomer.CustomerName} {levelName} {Diagram.FujitsuServiceOrganisationTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{levelName} {Diagram.FujitsuServiceOrganisationTitle}",
                Filename = $"{levelName} {Diagram.FujitsuServiceOrganisationTitle}",
                Level = level,
                Id = id,
                ServiceComponents = true,
                Resolvers = true,
                ServiceActivities = true
            };

            return View(chartModel);
        }

        [HttpGet]
        public ActionResult CustomerServiceOrganisationDiagram(int level, int id)
        {
            var levelName = level == 1 ? LevelNames.LevelOne : LevelNames.LevelTwo;
            var chartModel = new ChartViewModel
            {
                Title = $"{_appUserContext.Current.CurrentCustomer.CustomerName} {levelName} {Diagram.CustomerServiceOrganisationTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{levelName} {Diagram.CustomerServiceOrganisationTitle}",
                Filename = $"{levelName} {Diagram.CustomerServiceOrganisationTitle}",
                Level = level,
                Id = id,
                ServiceComponents = true,
                Resolvers = true,
                ServiceActivities = true
            };

            return View(chartModel);
        }

        [HttpGet]
        public ActionResult CustomerThirdPartyServiceOrganisationDiagram(int level, int id)
        {
            var levelName = level == 1 ? LevelNames.LevelOne : LevelNames.LevelTwo;
            var chartModel = new ChartViewModel
            {
                Title = $"{_appUserContext.Current.CurrentCustomer.CustomerName} {levelName} {Diagram.CustomerThirdPartyServiceOrganisationTitle}",
                Author = _userIdentity.Name,
                Creator = $"{UI.ApplicationName} {SettingsHelper.VersionTitle}",
                Subject = $"{levelName} {Diagram.CustomerThirdPartyServiceOrganisationTitle}",
                Filename = $"{levelName} {Diagram.CustomerThirdPartyServiceOrganisationTitle}",
                Level = level,
                Id = id,
                ServiceComponents = true,
                Resolvers = true,
                ServiceActivities = true
            };

            return View(chartModel);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadFujitsuServiceOrganisationChart(int id,
                bool svcComponents,
                bool resolverGroups,
                bool svcActivities)
        {
            var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.FujitsuServiceOrganisation);
            var chartData = generator.Generate(id, false, false, svcComponents, resolverGroups, svcActivities);
            var chart = Mapper.Map<List<ChartDataViewModel>>(chartData);

            return Json(chart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadCustomerServiceOrganisationChart(int id,
            bool svcComponents,
            bool resolverGroups,
            bool svcActivities)
        {
            var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.CustomerServiceOrganisation);
            var chartData = generator.Generate(id, false, false, svcComponents, resolverGroups, svcActivities);
            var chart = Mapper.Map<List<ChartDataViewModel>>(chartData);

            return Json(chart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult ReadCustomerThirdPartyServiceOrganisationChart(int id,
            bool svcComponents,
            bool resolverGroups,
            bool svcActivities)
        {
            var generator = _objectBuilder.Resolve<IDiagramGenerator>(Diagram.CustomerThirdPartyServiceOrganisation);
            var chartData = generator.Generate(id, false, false, svcComponents, resolverGroups, svcActivities);
            var chart = Mapper.Map<List<ChartDataViewModel>>(chartData);

            return Json(chart, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}