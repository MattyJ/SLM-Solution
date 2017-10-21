namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTemplateRows : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemplateRow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDomain = c.String(),
                        ServiceFunction = c.String(),
                        ServiceComponentLevel1 = c.String(),
                        ServiceComponentLevel2 = c.String(),
                        ServiceActivities = c.String(),
                        ServiceDeliveryOrganisation = c.String(),
                        ServiceDeliveryUnit = c.String(),
                        ResolverGroup = c.String(),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Template", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateRow", "TemplateId", "dbo.Template");
            DropIndex("dbo.TemplateRow", new[] { "TemplateId" });
            DropTable("dbo.TemplateRow");
        }
    }
}
