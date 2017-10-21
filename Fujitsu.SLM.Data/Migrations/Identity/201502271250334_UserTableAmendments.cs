namespace Fujitsu.SLM.Data.Migrations.Identity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTableAmendments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastLoginUtc", c => c.DateTime(nullable: false));
            Sql("UPDATE dbo.AspNetUsers SET LastLoginUtc = GETUTCDATE();");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastLoginUtc");
        }
    }
}
