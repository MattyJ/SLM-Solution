using Fujitsu.Aspose.Spreadsheets;

namespace Fujitsu.SLM.DataImporters.Entities
{
    [WorksheetRowItem(1, 0)]
    public class ServiceDecomposition : DataImportClassBase
    {
        [WorksheetRowItemValue("Service Domain")]
        public string ServiceDomain { get; set; }

        [WorksheetRowItemValue("Service Function")]
        public string ServiceFunction { get; set; }

        [WorksheetRowItemValue("Service Component Level 1")]
        public string ServiceComponentLevel1 { get; set; }

        [WorksheetRowItemValue("Service Component Level 2")]
        public string ServiceComponentLevel2 { get; set; }

        [WorksheetRowItemValue("Service Activities")]
        public string ServiceActivities { get; set; }

        [WorksheetRowItemValue("Responsible Organisation")]
        public string ServiceDeliveryOrganisation { get; set; }

        [WorksheetRowItemValue("Service Delivery Unit")]
        public string ServiceDeliveryUnit { get; set; }

        [WorksheetRowItemValue("Resolver Group")]
        public string ResolverGroup { get; set; }
    }
}