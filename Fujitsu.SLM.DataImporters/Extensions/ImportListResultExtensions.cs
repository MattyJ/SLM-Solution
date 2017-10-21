using Fujitsu.Aspose.Spreadsheets;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.DataImporters.Entities;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.DataImporters.Extensions
{
    public static class ImportListResultExtensions
    {
        public static List<TemplateRow> AsTemplateRows(this ImportListResult<ServiceDecomposition> importResults)
        {
            var templateRows = (from row in importResults.Results.OrderBy(x => x.ServiceDomain)
                    .ThenBy(x => x.ServiceFunction)
                    .ThenBy(x => x.ServiceComponentLevel1)
                    .ThenBy(x => x.ServiceComponentLevel2)
                    .ThenBy(x => x.ServiceDeliveryOrganisation)
                    .ThenBy(x => x.ServiceDeliveryUnit)
                    .ThenBy(x => x.ResolverGroup)
                                where !string.IsNullOrEmpty(row.ServiceDomain)
                                select new TemplateRow
                                {
                                    ServiceDomain = row.ServiceDomain.SafeTrim(Validation.MaxLength.DomainName),
                                    ServiceFunction = row.ServiceFunction.SafeTrim(Validation.MaxLength.FunctionName),
                                    ServiceActivities = row.ServiceActivities.SafeTrim(Validation.MaxLength.ServiceActivities),
                                    ServiceComponentLevel1 = row.ServiceComponentLevel1.SafeTrim(Validation.MaxLength.ServiceComponentName),
                                    ServiceComponentLevel2 = row.ServiceComponentLevel2.SafeTrim(Validation.MaxLength.ServiceComponentName),
                                    ServiceDeliveryOrganisation = row.ServiceDeliveryOrganisation.SafeTrim(Validation.MaxLength.ServiceDeliveryOrganisationName),
                                    ServiceDeliveryUnit = row.ServiceDeliveryUnit.SafeTrim(Validation.MaxLength.ServiceDeliveryUnitName),
                                    ResolverGroup = row.ResolverGroup.SafeTrim(Validation.MaxLength.ResolverGroupName)
                                }).ToList();
            return templateRows;
        }
    }
}
