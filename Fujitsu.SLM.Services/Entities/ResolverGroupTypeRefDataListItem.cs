namespace Fujitsu.SLM.Services.Entities
{
    public class ResolverGroupTypeRefDataListItem
    {
        public int Id { get; set; }
        public string ResolverGroupTypeName { get; set; }
        public bool Visible { get; set; }
        public int Order { get; set; }
        public int UsageCount { get; set; }
    }
}