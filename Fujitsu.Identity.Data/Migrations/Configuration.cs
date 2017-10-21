using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Fujitsu.Identity.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Fujitsu.Identity.Data.FujitsuIdentity>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Fujitsu.Identity.Data.FujitsuIdentity context)
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "Administrator"
                },
                new IdentityRole()
                {
                    Name = "Architect"
                },
                new IdentityRole()
                {
                    Name = "Viewer"
                },

            };

            roles.ForEach(role => context.Roles.AddOrUpdate(role));
            context.SaveChanges();

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
