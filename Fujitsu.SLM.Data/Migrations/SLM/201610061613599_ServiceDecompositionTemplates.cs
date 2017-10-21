namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceDecompositionTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemplateComponent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComponentLevel = c.Int(nullable: false),
                        ParentTemplateComponentId = c.Int(),
                        ComponentName = c.String(nullable: false, maxLength: 100),
                        ServiceActivities = c.String(maxLength: 1500),
                        TemplateFunctionId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateComponent", t => t.ParentTemplateComponentId)
                .ForeignKey("dbo.TemplateFunction", t => t.TemplateFunctionId, cascadeDelete: true)
                .Index(t => t.ParentTemplateComponentId)
                .Index(t => t.TemplateFunctionId);
            
            CreateTable(
                "dbo.TemplateFunction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FunctionName = c.String(nullable: false, maxLength: 100),
                        TemplateDomainId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateDomain", t => t.TemplateDomainId, cascadeDelete: true)
                .Index(t => t.TemplateDomainId);
            
            CreateTable(
                "dbo.TemplateDomain",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DomainName = c.String(nullable: false, maxLength: 100),
                        TemplateId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Template", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Filename = c.String(nullable: false, maxLength: 100),
                        TemplateType = c.Int(nullable: false),
                        TemplateData = c.Binary(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TemplateResolver",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDeliveryOrganisationName = c.String(nullable: false, maxLength: 100),
                        ServiceDeliveryUnitName = c.String(nullable: false, maxLength: 100),
                        ResolverGroupName = c.String(nullable: false, maxLength: 100),
                        TemplateComponentId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateComponent", t => t.TemplateComponentId, cascadeDelete: true)
                .Index(t => t.TemplateComponentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateResolver", "TemplateComponentId", "dbo.TemplateComponent");
            DropForeignKey("dbo.TemplateFunction", "TemplateDomainId", "dbo.TemplateDomain");
            DropForeignKey("dbo.TemplateDomain", "TemplateId", "dbo.Template");
            DropForeignKey("dbo.TemplateComponent", "TemplateFunctionId", "dbo.TemplateFunction");
            DropForeignKey("dbo.TemplateComponent", "ParentTemplateComponentId", "dbo.TemplateComponent");
            DropIndex("dbo.TemplateResolver", new[] { "TemplateComponentId" });
            DropIndex("dbo.TemplateDomain", new[] { "TemplateId" });
            DropIndex("dbo.TemplateFunction", new[] { "TemplateDomainId" });
            DropIndex("dbo.TemplateComponent", new[] { "TemplateFunctionId" });
            DropIndex("dbo.TemplateComponent", new[] { "ParentTemplateComponentId" });
            DropTable("dbo.TemplateResolver");
            DropTable("dbo.Template");
            DropTable("dbo.TemplateDomain");
            DropTable("dbo.TemplateFunction");
            DropTable("dbo.TemplateComponent");
        }
    }
}
