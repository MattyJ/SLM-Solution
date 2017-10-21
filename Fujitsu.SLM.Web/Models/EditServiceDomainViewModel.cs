namespace Fujitsu.SLM.Web.Models
{
    public class EditServiceDomainViewModel : LevelViewModel
    {
        public ServiceDomainViewModel ServiceDomain { get; set; }
        public bool CanMoveServiceFunction { get; set; }
    }
}