namespace Fujitsu.SLM.Web.Models
{
    public class ServiceFunctionGridViewModel : LevelViewModel
    {
        public int ServiceDomainId { get; set; }
        public bool HasServiceDomainContext { get; set; }
        public bool CanMoveServiceFunction { get; set; }
    }
}