namespace Fujitsu.SLM.Web.Models
{
    public class ResolverGridViewModel : LevelViewModel
    {
        public int ServiceDeskId { get; set; }
        public int? ServiceComponentId { get; set; }
        public bool HasServiceComponentContext { get; set; }
        public bool CanMoveResolver { get; set; }
    }
}