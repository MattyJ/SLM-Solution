namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceComponentDiagramOrderColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceComponent", "DiagramOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceComponent", "DiagramOrder");
        }
    }
}
