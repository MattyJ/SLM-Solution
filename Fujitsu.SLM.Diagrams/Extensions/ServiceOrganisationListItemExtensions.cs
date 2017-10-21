using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Diagrams.Entities;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Diagrams.Extensions
{
    public static class ServiceOrganisationListItemExtensions
    {
        private const string NoneSpecified = "None Specified";

        public static void ProcessResolvers(this List<ServiceOrganisationListItem> resolvers, bool svcComponents,
            bool svcActivities, string organisationType, string customerName, ChartDataListItem chartData)
        {
            if (!svcComponents && !svcActivities)
            {
                // Resolvers only
                ProcessResolverServiceActivities(resolvers, true, false, ServiceDeliveryOrganisationNames.Fujitsu, chartData);
            }
            if (svcComponents)
            {
                // Resolvers, Components and/or Activities
                var currentServiceDeliveryUnit = string.Empty;
                var currentResolverGroup = string.Empty;

                foreach (var resolver in resolvers)
                {
                    if (!resolver.ServiceDeliveryUnitTypeName.SafeEquals(currentServiceDeliveryUnit) ||
                        !resolver.ResolverGroupTypeName.SafeEquals(currentResolverGroup))
                    {
                        var sdu = resolver.ServiceDeliveryUnitTypeName;
                        var rg = resolver.ResolverGroupTypeName;

                        // Add Resolver
                        var resolverChartListItem = AddResolver(resolver.Resolver.Id, organisationType, customerName, sdu, rg);

                        currentServiceDeliveryUnit = resolver.ServiceDeliveryUnitTypeName;
                        currentResolverGroup = resolver.ResolverGroupTypeName;

                        // List of Level One Components being resolved
                        var levelOneComponents = resolvers.Where(r =>
                            r.ServiceDeliveryUnitTypeName == sdu
                            && r.ResolverGroupTypeName == rg
                            && r.ServiceComponent.ComponentLevel == 1)
                            .Select(c => c.ServiceComponent).ToList();

                        // List of the Level Two Parent Components being resolved
                        //TODO: r.ServiceComponent.ParentServiceComponent != null is a bug where the root cause needs to be identified
                        var levelTwoParentComponents = resolvers.Where(r =>
                            r.ServiceDeliveryUnitTypeName == sdu
                            && r.ResolverGroupTypeName == rg
                            && r.ServiceComponent.ComponentLevel == 2
                            && r.ServiceComponent.ParentServiceComponent != null)
                            .Select(c => c.ServiceComponent.ParentServiceComponent).ToList();

                        // Union of both Level One Components and Level Two Parent Components
                        var allLevelOneComponents = levelOneComponents.Union(levelTwoParentComponents).OrderBy(c => c.DiagramOrder).ThenBy(c => c.ComponentName);

                        // Distinct list of components
                        var distinctServiceComponents = allLevelOneComponents.GroupBy(c => c.Id).Select(c => c.First());

                        foreach (var component in distinctServiceComponents)
                        {
                            // Add the Parent Component
                            var parentComponentListItem = AddServiceComponent(component);

                            if (component.ChildServiceComponents == null || !component.ChildServiceComponents.Any())
                            {
                                AddResolverComponentNoChildrenComponent(svcActivities, component, parentComponentListItem);
                            }
                            else
                            {
                                AddResolverChildComponents(svcActivities, organisationType, sdu, rg, component, parentComponentListItem);
                            }

                            resolverChartListItem.Units.Add(parentComponentListItem);
                        }

                        chartData.Units.Add(resolverChartListItem);
                    }
                }
            }
            else if (svcActivities)
            {
                // Resolvers and Activities
                ProcessResolverServiceActivities(resolvers, true, true, ServiceDeliveryOrganisationNames.Fujitsu,
                    chartData);
            }
        }

        public static void ProcessResolverServiceComponents(this List<ServiceOrganisationListItem> resolvers,
            bool svcActivities, string organisationType, ChartDataListItem chartData)
        {
            // Components and/or Activities

            // List of Level One Components being resolved
            var levelOneComponents = resolvers.Where(r => r.ServiceComponent.ComponentLevel == 1)
                .Select(c => c.ServiceComponent).ToList();

            // List of the Level Two Parent Components being resolved
            var levelTwoParentComponents = resolvers.Where(
                r => r.ServiceComponent.ComponentLevel == 2)
                .Select(c => c.ServiceComponent.ParentServiceComponent).ToList();

            // Union of both Level One Components and Level Two Parent Components
            var allLevelOneComponents = levelOneComponents.Union(levelTwoParentComponents).OrderBy(r => r.DiagramOrder).ThenBy(r => r.ComponentName);

            // Distinct list of components
            var distinctServiceComponents = allLevelOneComponents.GroupBy(c => c.Id).Select(c => c.First());

            foreach (var component in distinctServiceComponents)
            {
                // Add the Parent Component
                var parentComponentListItem = AddServiceComponent(component);

                if (component.ChildServiceComponents == null || !component.ChildServiceComponents.Any())
                {
                    AddResolverComponentNoChildrenComponent(svcActivities, component, parentComponentListItem);
                }
                else
                {
                    AddComponentWithChildren(svcActivities, organisationType, parentComponentListItem, component);
                }

                chartData.Units.Add(parentComponentListItem);
            }
        }

        public static void ProcessResolverServiceActivities(this List<ServiceOrganisationListItem> resolvers,
            bool addResolver, bool addServiceActivity, string sdoType, ChartDataListItem chart)
        {
            var currentServiceDeliveryUnit = string.Empty;
            var currentResolverGroup = string.Empty;

            foreach (var resolver in resolvers)
            {
                if (addResolver)
                {

                    if (!resolver.ServiceDeliveryUnitTypeName.SafeEquals(currentServiceDeliveryUnit) ||
                        !resolver.ResolverGroupTypeName.SafeEquals(currentResolverGroup))
                    {
                        var sdu = resolver.ServiceDeliveryUnitTypeName;
                        var rg = resolver.ResolverGroupTypeName;

                        // Add Resolver
                        var resolverChartListItem = new ChartDataListItem
                        {
                            Id = resolver.Resolver.Id,
                            Title = resolver.ServiceDeliveryOrganisationTypeName,
                            TitleTwo = sdu,
                            TitleThree = rg,
                            Type = DecompositionType.Resolver.ToString(),
                            CenteredTitle = string.Empty,
                            Units = new List<ChartDataListItem>()
                        };

                        if (addServiceActivity)
                        {
                            var serviceActivities =
                                resolvers.Where(
                                    r =>
                                        r.ServiceDeliveryUnitTypeName == sdu && r.ResolverGroupTypeName == rg &&
                                        r.ServiceActivities != NoneSpecified)
                                    .Aggregate(string.Empty,
                                        (current, activity) => current + $"{activity.ServiceActivities}\r\n");

                            AddResolverServiceActivity(resolver.Resolver.Id,
                                string.IsNullOrEmpty(serviceActivities) ? NoneSpecified : serviceActivities,
                                resolverChartListItem);
                        }

                        currentServiceDeliveryUnit = resolver.ServiceDeliveryUnitTypeName;
                        currentResolverGroup = resolver.ResolverGroupTypeName;

                        chart.Units.Add(resolverChartListItem);
                    }
                }
                else if (addServiceActivity)
                {
                    AddResolverServiceActivity(resolver.Resolver.Id, resolver.ServiceActivities, chart);
                }
            }
        }

        #region Private methods

        private static void AddResolverComponentNoChildrenComponent(bool svcActivities, ServiceComponent component,
            ChartDataListItem chartData)
        {
            // Create A Dummy Child Component
            var childComponentListItem = AddDummyChildComponent(component.Id);

            if (svcActivities)
            {
                AddServiceActivity(component, childComponentListItem);
            }

            chartData.Units.Add(childComponentListItem);
        }

        private static void AddResolverChildComponents(bool svcActivities, string organisationType, string sdu,
            string rg, ServiceComponent component, ChartDataListItem parentComponentListItem)
        {
            foreach (var childComponent in component.ChildServiceComponents.Where(c => c.Resolver != null &&
                                                                                       c.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName == organisationType &&
                                                                                       (sdu == NoneSpecified && c.Resolver.ServiceDeliveryUnitType == null ||
                                                                                        c.Resolver.ServiceDeliveryUnitType != null && sdu.SafeEquals(c.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName)) &&
                                                                                       (rg == NoneSpecified && c.Resolver.ResolverGroupType == null ||
                                                                                        c.Resolver.ResolverGroupType != null && rg.SafeEquals(c.Resolver.ResolverGroupType.ResolverGroupTypeName))).OrderBy(c => c.DiagramOrder).ThenBy(c => c.ComponentName))
            {
                var childComponentListItem = AddServiceComponent(childComponent);

                if (svcActivities)
                {
                    AddServiceActivity(childComponent, childComponentListItem);
                }

                parentComponentListItem.Units.Add(childComponentListItem);
            }
        }

        private static void AddComponentWithChildren(bool svcActivities, string organisationType,
            ChartDataListItem chartData, ServiceComponent component)
        {
            foreach (
                var childComponent in component.ChildServiceComponents.Where(c => c.Resolver != null && c.Resolver.ServiceDeliveryOrganisationType
                            .ServiceDeliveryOrganisationTypeName == organisationType).OrderBy(c => c.DiagramOrder).ThenBy(c => c.ComponentName))
            {
                var childComponentListItem = AddServiceComponent(childComponent);

                if (svcActivities)
                {
                    AddServiceActivity(childComponent, childComponentListItem);
                }

                chartData.Units.Add(childComponentListItem);
            }

        }

        private static ChartDataListItem AddResolver(int id, string sdo, string customerName, string sdu, string rg)
        {
            var serviceDeliveryOrganisation = sdo.SafeEquals(ServiceDeliveryOrganisationNames.Fujitsu)
                ? sdo
                : sdo.SafeEquals(ServiceDeliveryOrganisationNames.Customer)
                    ? customerName
                    : $"{customerName} Third Party";

            var resolverChartListItem = new ChartDataListItem
            {
                Id = id,
                Title = serviceDeliveryOrganisation,
                TitleTwo = sdu,
                TitleThree = rg,
                Type = DecompositionType.Resolver.ToString(),
                CenteredTitle = string.Empty,
                Units = new List<ChartDataListItem>()
            };
            return resolverChartListItem;
        }

        private static ChartDataListItem AddServiceComponent(ServiceComponent component)
        {
            // Add Child Component
            var componentListItem =
                new ChartDataListItem
                {
                    Id = component.Id,
                    Title = component.ComponentName,
                    Type = DecompositionType.Component.ToString(),
                    CenteredTitle = string.Empty,
                    Units = new List<ChartDataListItem>()
                };

            return componentListItem;
        }

        private static ChartDataListItem AddDummyChildComponent(int id)
        {
            var childComponentListItem = new ChartDataListItem
            {
                Id = id,
                Title = string.Empty,
                CenteredTitle = string.Empty,
                Type = DecompositionType.LineForDummyChildComponent.ToString(),
                Units = new List<ChartDataListItem>()
            };
            return childComponentListItem;
        }

        private static void AddServiceActivity(ServiceComponent component, ChartDataListItem chartData)
        {
            // Add Service Activity
            if (component.Resolver != null)
            {
                if (!string.IsNullOrEmpty(component.ServiceActivities))
                {
                    AddResolverServiceActivity(component.Id, component.ServiceActivities, chartData);
                    return;
                }
                if (component.ComponentLevel == (int)ServiceComponentLevel.Level2 && !string.IsNullOrEmpty(component.ParentServiceComponent.ServiceActivities))
                {
                    AddResolverServiceActivity(component.Id, component.ParentServiceComponent.ServiceActivities, chartData);
                    return;
                }
            }

            AddResolverServiceActivity(component.Id, NoneSpecified, chartData);
        }

        private static void AddResolverServiceActivity(int id, string serviceActivitoes, ChartDataListItem chartData)
        {
            var svcActivities = new ChartDataListItem
            {
                Id = id,
                Title = serviceActivitoes,
                Type = DecompositionType.Activity.ToString(),
                CenteredTitle = string.Empty,
                Units = new List<ChartDataListItem>()
            };

            chartData.Units.Add(svcActivities);

        }
    }

    #endregion
}
