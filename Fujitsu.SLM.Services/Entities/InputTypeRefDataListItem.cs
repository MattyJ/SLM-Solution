namespace Fujitsu.SLM.Services.Entities
{
    public class InputTypeRefDataListItem
    {
        public int Id { get; set; }

        public int InputTypeNumber { get; set; }

        public string InputTypeName { get; set; }

        public bool Default { get; set; }

        public int SortOrder { get; set; }

        public int UsageCount { get; set; }
    }
}