namespace Fujitsu.SLM.Web.Models
{
    public class CurrentCustomerViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public bool Baseline { get; set; }

        public CurrentCustomerViewModel()
        {
            Id = 0;
            CustomerName = "None Selected";
            Baseline = false;
        }
    }
}
