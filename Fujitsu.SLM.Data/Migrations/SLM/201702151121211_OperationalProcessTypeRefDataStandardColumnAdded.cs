namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperationalProcessTypeRefDataStandardColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperationalProcessTypeRefData", "Standard", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperationalProcessTypeRefData", "Standard");
        }
    }
}
