using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace Fujitsu.SLM.Web.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Lockout End Date")]
        public DateTime? LockoutEndDateUtc { get; set; }

        [Display(Name = "Access Failed Count")]
        public int AccessFailedCount { get; set; }

        [Display(Name = "Last Login Date")]
        public DateTime LastLoginUtc { get; set; }

        public int RegionTypeId { get; set; }

        [Display(Name = "Region")]
        public string RegionName { get; set; }

        public bool IsLockedOut
        {
            get
            {
                if (!LockoutEndDateUtc.HasValue)
                {
                    return false;
                }
                if (LockoutEndDateUtc.Value > DateTime.UtcNow)
                {
                    return true;
                }
                return false;
            }
        }
    }
}