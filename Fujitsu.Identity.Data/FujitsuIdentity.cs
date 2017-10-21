using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Fujitsu.Identity.Data
{

    public class FujitsuIdentity : IdentityDbContext<ApplicationUser>
    {
        public FujitsuIdentity()
            : base("FujitsuIdentity", throwIfV1Schema: false)
        {
        }

        public FujitsuIdentity(string connectionString)
            : base(connectionString)
        {
        }

        public static FujitsuIdentity Create()
        {
            return new FujitsuIdentity();
        }

        public static FujitsuIdentity Create(string connectionString)
        {
            return new FujitsuIdentity(connectionString);
        }
    }
}
