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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Fujitsu.SLM.Web.Controllers
{
    public class OperationalProcessesController : BaseController
    {
        private readonly IOperationalProcessTypeRefDataService _operationalProcessTypeRefDataService;
        private readonly IResolverService _resolverService;
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;

        public OperationalProcessesController(IOperationalProcessTypeRefDataService operationalProcessTypeRefDataService,
            IResolverService resolverService,
            IContextManager contextManager,
            IAppUserContext appUserContext) : base(contextManager)
        {
            if (operationalProcessTypeRefDataService == null)
            {
                throw new ArgumentNullException("operationalProcessTypeRefDataService");
            }

            if (resolverService == null)
            {
                throw new ArgumentNullException("resolverService");
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException("contextManager");
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException("appUserContext");
            }

            _operationalProcessTypeRefDataService = operationalProcessTypeRefDataService;
            _resolverService = resolverService;
            _contextManager = contextManager;
            _appUserContext = appUserContext;
        }

        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult Index(string level)
        {
            _operationalProcessTypeRefDataService.PurgeOrphans();
            //TODO: This needs to change now to show only the active resolver groups, any operational processes changes will be applied
            // to the resolvers containing the group
            var model = new ViewProcessDotMatrixViewModel
            {
                EditLevel = level
            };

            // Add the column for holding the ResolverId, which is actually the ServiceComponentId.
            model.Columns.Add(new DynamicGridColumn
            {
                Name = DotMatrixNames.ResolverId,
                Title = DotMatrixNames.ResolverId,
                Type = typeof(int),
                Visible = false,
                Editable = false
            });

            // Add the column for holding the Resolver Group name and Service Component Name
            model.Columns.Add(new DynamicGridColumn
            {
                Name = DotMatrixNames.ResolverName,
                Title = "Resolver Group",
                Type = typeof(string),
                Editable = false
            });

            model.Columns.Add(new DynamicGridColumn
            {
                Name = DotMatrixNames.ComponentName,
                Title = "Service Component",
                Type = typeof(string),
                Editable = false
            });

            // Now is the dynamic part. Need to have a column for all of the Operational Processes.
            var processes = _operationalProcessTypeRefDataService
                .GetAllAndNotVisibleForCustomer(_appUserContext.Current.CurrentCustomer.Id)
                .OrderBy(o => o.SortOrder);

            processes.ForEach(f => model.Columns.Add(new DynamicGridColumn
            {
                Name = string.Concat(DotMatrixNames.OpIdPrefix, f.Id),
                Title = f.OperationalProcessTypeName,
                Type = typeof(bool)
            }));

            return View("DotMatrix" + level, model);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Viewer)]
        public ActionResult ReadAjaxDotMatrixGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    result = _resolverService
                        .GetDotMatrix(_appUserContext.Current.CurrentCustomer.Id, false)
                        .ToDataSourceResult(request, Mapper.Map<Dictionary<string, object>>);
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
        [Authorize(Roles = UserRoles.Architect)]
        public ActionResult UpdateAjaxDotMatrixGrid([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<Dictionary<string, string>> model)
        {
            DataSourceResult result = null;

            try
            {
                if (_appUserContext.Current?.CurrentCustomer != null && _appUserContext.Current.CurrentCustomer.Id > 0)
                {
                    var updateResolverGroups = new List<Resolver>();

                    List<OperationalProcessTypeRefData> opProcRefs = _operationalProcessTypeRefDataService
                        .All()
                        .ToList();

                    foreach (var resolverGroup in model)
                    {
                        var resolverGroupId = int.Parse(resolverGroup[DotMatrixNames.ResolverId]);
                        var updateResolverGroup = _resolverService
                            .GetByCustomer(_appUserContext.Current.CurrentCustomer.Id)
                            .SingleOrDefault(x => x.Id == resolverGroupId);
                        if (updateResolverGroup == null)
                        {
                            ModelState.AddModelError(ModelStateErrorNames.ResolverGroupCannotBeFound,
                                WebResources.ResolverGroupCannotBeFound);
                            break;
                        }

                        if (updateResolverGroup.OperationalProcessTypes == null)
                        {
                            updateResolverGroup.OperationalProcessTypes = new List<OperationalProcessType>();
                        }
                        else
                        {
                            updateResolverGroup.OperationalProcessTypes.Clear();
                        }
                        var opProcIds = resolverGroup
                            .Where(x => x.Key.StartsWith(DotMatrixNames.OpIdPrefix) &&
                                bool.Parse(x.Value))
                            .Select(s => int.Parse(s.Key.Replace(DotMatrixNames.OpIdPrefix, string.Empty)))
                            .ToList();
                        foreach (var opProcId in opProcIds)
                        {
                            if (opProcRefs.Any(x => x.Id == opProcId))
                            {
                                var opProcRef = opProcRefs
                                    .Single(x => x.Id == opProcId);
                                updateResolverGroup.OperationalProcessTypes.Add(new OperationalProcessType
                                {
                                    OperationalProcessTypeRefData = opProcRef,
                                    Resolver = updateResolverGroup
                                });
                            }
                        }

                        updateResolverGroup.UpdatedBy = _contextManager.UserManager.Name;
                        updateResolverGroup.UpdatedDate = DateTime.Now;

                        updateResolverGroups.Add(updateResolverGroup);
                    }

                    if (updateResolverGroups.Any())
                    {
                        _resolverService.Update(updateResolverGroups);
                    }

                    result = _resolverService
                            .GetDotMatrix(_appUserContext.Current.CurrentCustomer.Id, false)
                            .ToDataSourceResult(request, ModelState, Mapper.Map<Dictionary<string, object>>);
                }
            }
            catch (Exception ex)
            {
                _contextManager.ResponseManager.StatusCode = 500;
                _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, ex.Message);
            }

            return Json(result);
        }
    }
}