namespace Fujitsu.SLM.Web.Models
{
    public class EditServiceDeskViewModel : LevelViewModel
    {
        public ServiceDeskViewModel ServiceDesk { get; set; }
        public bool CanMoveServiceDomain { get; set; }
        public bool CanImportServiceConfiguratorTemplate { get; set; }
        public bool CanImportServiceLandscapeTemplate { get; set; }
    }
}