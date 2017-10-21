namespace Fujitsu.SLM.Services.Entities
{
    public class OperationalProcessTypeRefDataListItem
    {
        public int Id { get; set; }
        public string OperationalProcessTypeName { get; set; }
        public bool Visible { get; set; }
        public bool Standard { get; set; }
        public int SortOrder { get; set; }
        public int UsageCount { get; set; }
    }
}