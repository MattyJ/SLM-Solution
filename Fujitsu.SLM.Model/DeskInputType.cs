namespace Fujitsu.SLM.Model
{
    public class DeskInputType
    {
        public int Id { get; set; }

        public virtual InputTypeRefData InputTypeRefData { get; set; }

        // Foreign key 
        public int ServiceDeskId { get; set; }
        public virtual ServiceDesk ServiceDesk { get; set; }
    }
}