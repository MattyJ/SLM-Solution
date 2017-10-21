namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceFunctionDiagramOrderColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceFunction", "DiagramOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceFunction", "DiagramOrder");
        }
    }
}
