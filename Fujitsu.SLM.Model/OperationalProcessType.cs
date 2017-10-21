namespace Fujitsu.SLM.Model
{
    public class OperationalProcessType
    {
        public int Id { get; set; }

        public int OperationalProcessTypeRefDataId { get; set; }
        public virtual OperationalProcessTypeRefData OperationalProcessTypeRefData { get; set; }

        // Foreign key
        public virtual Resolver Resolver { get; set; }
    }
}