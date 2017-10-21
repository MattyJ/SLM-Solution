namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceDomainDiagramOrderColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceDomain", "DiagramOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceDomain", "DiagramOrder");
        }
    }
}
