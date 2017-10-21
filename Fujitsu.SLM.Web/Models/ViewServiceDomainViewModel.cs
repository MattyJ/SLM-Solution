namespace Fujitsu.SLM.Web.Models
{
    public class ViewServiceDomainViewModel : LevelViewModel
    {
        public bool CanMoveServiceDomain { get; set; }
        public bool CanImportServiceConfiguratorTemplate { get; set; }
        public bool CanImportServiceLandscapeTemplate { get; set; }
    }
}