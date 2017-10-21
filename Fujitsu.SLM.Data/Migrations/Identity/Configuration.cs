using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Fujitsu.SLM.Data.Migrations.Identity
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Fujitsu.SLM.Data.IdentityDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\Identity";
        }

        protected override void Seed(Fujitsu.SLM.Data.IdentityDataContext context)
        {
            var rolesArray = new[] { "Administrator", "Architect", "Viewer" };

            var administrators = new List<string>
            {
                "matthew.jordan@uk.fujitsu.com",
                "patrick.williams@uk.fujitsu.com",
                "vanda.almeida@ts.fujitsu.com",
            };

            const string defaultPassword = "Pa$$w0rd";

            var passwordHash = new PasswordHasher();
            var password = passwordHash.HashPassword(defaultPassword);

            var existingRolesCount = context.Roles.Count();
            if (existingRolesCount > 0)
            {
                return;
            }

            var roles = rolesArray.Select(role => new IdentityRole(role)).ToList();

            roles.ForEach(role => context.Roles.AddOrUpdate(role));
            context.SaveChanges();

            foreach (var user in administrators.Select(admin => new ApplicationUser
            {
                UserName = admin,
                Email = admin,
                PasswordHash = password,
                LockoutEnabled = true,
                LastLoginUtc = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString()
            }))
            {
                context.Users.AddOrUpdate(u => u.UserName, user);
            }

            context.SaveChanges();

            foreach (var user in context.Users.ToList())
            {
                foreach (var role in context.Roles.ToList())
                {
                    var userRole = new IdentityUserRole
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    };
                    user.Roles.Add(userRole);
                }
            }

            context.SaveChanges();
        }
    }
}
