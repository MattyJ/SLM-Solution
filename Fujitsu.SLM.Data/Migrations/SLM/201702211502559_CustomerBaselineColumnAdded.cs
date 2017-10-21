namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerBaselineColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customer", "Baseline", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customer", "Baseline");
        }
    }
}
