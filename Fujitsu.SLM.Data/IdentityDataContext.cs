using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.SLM.Data
{
    [ExcludeFromCodeCoverage]
    public class IdentityDataContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityDataContext()
            : base("IdentityDataContext", throwIfV1Schema: false)
        {
        }

        public IdentityDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public static IdentityDataContext Create()
        {
            return new IdentityDataContext();
        }

        public static IdentityDataContext Create(string connectionString)
        {
            return new IdentityDataContext(connectionString);
        }
    }
}
