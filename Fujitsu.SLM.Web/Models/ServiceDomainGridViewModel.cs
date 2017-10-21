namespace Fujitsu.SLM.Web.Models
{
    public class ServiceDomainGridViewModel : LevelViewModel
    {
        public int ServiceDeskId { get; set; }
        public bool HasServiceDeskContext { get; set; }
        public bool CanMoveServiceDomain { get; set; }
        public bool CanImportServiceConfiguratorTemplate { get; set; }
        public bool CanImportServiceLandscapeTemplate { get; set; }
    }
}