using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Transformers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Transformers
{
    public class TransformSpreadsheetToTemplate : ITransformSpreadsheetToTemplate
    {
        private readonly IUserIdentity _userIdentity;

        public TransformSpreadsheetToTemplate(IUserIdentity userIdentity)
        {
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }
            _userIdentity = userIdentity;
        }

        public Template Transform(Template template, List<TemplateRow> templateRows)
        {
            foreach (var templateRow in templateRows)
            {
                if (!string.IsNullOrEmpty(templateRow.ServiceDomain))
                {
                    TransformServiceDomain(template, templateRow);
                    template.TemplateRows.Add(templateRow);
                }
            }

            return template;
        }


        public void TransformServiceDomain(Template template, TemplateRow row)
        {
            var templateDomain = template.TemplateDomains.FirstOrDefault(d => d.DomainName == row.ServiceDomain);

            if (templateDomain == null)
            {
                var dateTimeNow = DateTime.Now;
                var domain = new TemplateDomain
                {
                    DomainName = row.ServiceDomain,
                    TemplateFunctions = new List<TemplateFunction>(),
                    InsertedDate = dateTimeNow,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name
                };

                template.TemplateDomains.Add(domain);
                templateDomain = domain;
            }

            if (!string.IsNullOrEmpty(row.ServiceFunction))
            {
                TransformServiceFunction(templateDomain, row);
            }
        }
        private void TransformServiceFunction(TemplateDomain templateDomain, TemplateRow row)
        {
            var templateFunction = templateDomain.TemplateFunctions.FirstOrDefault(d => d.FunctionName == row.ServiceFunction);

            if (templateFunction == null)
            {
                var dateTimeNow = DateTime.Now;
                var function = new TemplateFunction
                {
                    FunctionName = row.ServiceFunction,
                    TemplateComponents = new List<TemplateComponent>(),
                    InsertedDate = dateTimeNow,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name
                };

                templateDomain.TemplateFunctions.Add(function);
                templateFunction = function;
            }

            if (!string.IsNullOrEmpty(row.ServiceComponentLevel1))
            {
                TransformServiceComponent(templateFunction, row);
            }
        }
        private void TransformServiceComponent(TemplateFunction templateFunction, TemplateRow row)
        {
            var templateComponent = templateFunction.TemplateComponents.FirstOrDefault(c => c.ComponentName == row.ServiceComponentLevel1 && c.ComponentLevel == 1);

            if (templateComponent == null)
            {
                var dateTimeNow = DateTime.Now;
                var levelOneComponent = new TemplateComponent
                {
                    ComponentName = row.ServiceComponentLevel1,
                    ComponentLevel = 1,
                    ServiceActivities =
                         string.IsNullOrEmpty(row.ServiceComponentLevel2) &&
                        !string.IsNullOrEmpty(row.ServiceActivities)
                            ? row.ServiceActivities
                            : string.Empty,
                    ChildTemplateComponents = new List<TemplateComponent>(),
                    TemplateResolvers = new List<TemplateResolver>(),
                    TemplateFunction = templateFunction,
                    InsertedDate = dateTimeNow,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name
                };

                templateFunction.TemplateComponents.Add(levelOneComponent);
                templateComponent = levelOneComponent;
            }

            if (!string.IsNullOrEmpty(row.ServiceComponentLevel2))
            {
                if (templateComponent.TemplateResolvers.Any())
                {
                    throw new DataImportException(
                         $"Error reading Service Decomposition Design spreadsheet. Worksheet, Resolvers and Childs Components detected on Component [{row.ServiceComponentLevel1}].");
                }
                TransformServiceComponentLevelTwo(templateComponent, row);
            }
            else if (!string.IsNullOrEmpty(row.ServiceDeliveryOrganisation))
            {
                if (templateComponent.TemplateResolvers.Any())
                {
                    throw new DataImportException(
                        $"Error reading Service Decomposition Template spreadsheet. Worksheet, Multiple Resolvers per Component detected on Component [{row.ServiceComponentLevel1}].");
                }
                TransformResolver(templateComponent, row);
            }
        }
        private void TransformServiceComponentLevelTwo(TemplateComponent templateComponent, TemplateRow row)
        {
            var templateComponentLevelTwo = templateComponent.ChildTemplateComponents.FirstOrDefault(c => c.ComponentName == row.ServiceComponentLevel2 && c.ComponentLevel == 2);
            if (templateComponentLevelTwo == null)
            {
                var dateTimeNow = DateTime.Now;
                var levelTwoComponent = new TemplateComponent
                {
                    ComponentName = row.ServiceComponentLevel2,
                    ComponentLevel = 2,
                    ServiceActivities = !string.IsNullOrEmpty(row.ServiceActivities) ? row.ServiceActivities : string.Empty,
                    TemplateResolvers = new List<TemplateResolver>(),
                    ParentTemplateComponent = templateComponent,
                    TemplateFunction = templateComponent.TemplateFunction,
                    InsertedDate = dateTimeNow,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name
                };

                templateComponent.ChildTemplateComponents.Add(levelTwoComponent);
                templateComponentLevelTwo = levelTwoComponent;
            }

            if (!string.IsNullOrEmpty(row.ServiceDeliveryOrganisation))
            {
                if (templateComponentLevelTwo.TemplateResolvers.Any())
                {
                    throw new DataImportException(
                        $"Error reading Service Decomposition Template spreadsheet. Multiple Resolvers per Component detected on Component [{row.ServiceComponentLevel1}>{row.ServiceComponentLevel2}].");
                }
                TransformResolver(templateComponentLevelTwo, row);
            }
        }
        private void TransformResolver(TemplateComponent templateComponent, TemplateRow row)
        {

            if (!ServiceDeliveryOrganisationNames.Descriptions.Contains(row.ServiceDeliveryOrganisation))
            {
                throw new DataImportException($"Error reading Service Decomposition Template spreadsheet. Invalid Responsible Organisation Value - {row.ServiceDeliveryOrganisation}.");
            }

            var templateResolver =
                templateComponent.TemplateResolvers.FirstOrDefault(
                    r => r.ServiceDeliveryOrganisationName == row.ServiceDeliveryOrganisation &&
                         r.ServiceDeliveryUnitName == row.ServiceDeliveryUnit &&
                         r.ResolverGroupName == row.ResolverGroup);

            if (templateResolver == null)
            {
                var dateTimeNow = DateTime.Now;
                var resolver = new TemplateResolver
                {
                    ServiceDeliveryOrganisationName = row.ServiceDeliveryOrganisation,
                    ServiceDeliveryUnitName = row.ServiceDeliveryUnit,
                    ResolverGroupName = row.ResolverGroup,
                    TemplateComponent = templateComponent,
                    InsertedDate = dateTimeNow,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = dateTimeNow,
                    UpdatedBy = _userIdentity.Name
                };

                templateComponent.TemplateResolvers.Add(resolver);
            }
        }


    }
}
