using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Fujitsu.SLM.Web.Models
{
    public class MyIdentityViewModel
    {

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public List<String> UserRoles { get; set; }

        public List<Claim> Claims { get; set; }

        public string SupportEmailAddress { get; set; }
    }
}
