namespace Fujitsu.SLM.Services.Entities
{
    public class DomainTypeRefDataListItem
    {
        public int Id { get; set; }
        public string DomainName { get; set; }
        public bool Visible { get; set; }
        public int SortOrder { get; set; }
        public int UsageCount { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}