namespace Fujitsu.SLM.Web.Models
{
    public class ServiceComponentGridViewModel : LevelViewModel
    {
        public int? ServiceFunctionId { get; set; }
        public int? ParentComponentId { get; set; }
        public bool HasServiceFunctionContext { get; set; }
        public bool CanMoveServiceComponent { get; set; }
    }
}