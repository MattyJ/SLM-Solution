using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddCustomerContributorViewModel : LevelViewModel
    {
        public AddCustomerContributorViewModel()
        {
            Users = new List<SelectListItem>();
        }

        public Guid UserId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<SelectListItem> Users { get; private set; }
    }
}