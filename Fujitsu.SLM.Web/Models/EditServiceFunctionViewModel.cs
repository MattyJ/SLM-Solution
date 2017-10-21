namespace Fujitsu.SLM.Web.Models
{
    public class EditServiceFunctionViewModel : LevelViewModel
    {
        public ServiceFunctionViewModel ServiceFunction { get; set; }
        public bool CanMoveServiceComponent { get; set; }
    }
}