namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContextHelpRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 100),
                        Title = c.String(maxLength: 100),
                        HelpText = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key, unique: true);
            
            CreateTable(
                "dbo.Contributor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        EmailAddress = c.String(nullable: false, maxLength: 150),
                        CustomerId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false, maxLength: 100),
                        CustomerNotes = c.String(maxLength: 250),
                        AssignedArchitect = c.String(nullable: false, maxLength: 150),
                        Active = c.Boolean(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CustomerName, unique: true);
            
            CreateTable(
                "dbo.ServiceDesk",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeskName = c.String(nullable: false, maxLength: 100),
                        CustomerId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.DeskInputType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDeskId = c.Int(nullable: false),
                        InputTypeRefData_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InputTypeRefData", t => t.InputTypeRefData_Id)
                .ForeignKey("dbo.ServiceDesk", t => t.ServiceDeskId, cascadeDelete: true)
                .Index(t => t.ServiceDeskId)
                .Index(t => t.InputTypeRefData_Id);
            
            CreateTable(
                "dbo.InputTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InputTypeNumber = c.Int(nullable: false),
                        InputTypeName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.InputTypeName, unique: true);
            
            CreateTable(
                "dbo.Resolver",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDeskId = c.Int(nullable: false),
                        ServiceDeliveryOrganisationTypeId = c.Int(nullable: false),
                        ServiceDeliveryOrganisationNotes = c.String(maxLength: 250),
                        ServiceDeliveryUnitNotes = c.String(maxLength: 250),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                        ResolverGroupType_Id = c.Int(),
                        ServiceComponent_Id = c.Int(),
                        ServiceDeliveryUnitType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ResolverGroupTypeRefData", t => t.ResolverGroupType_Id)
                .ForeignKey("dbo.ServiceComponent", t => t.ServiceComponent_Id)
                .ForeignKey("dbo.ServiceDeliveryOrganisationTypeRefData", t => t.ServiceDeliveryOrganisationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ServiceDeliveryUnitTypeRefData", t => t.ServiceDeliveryUnitType_Id)
                .ForeignKey("dbo.ServiceDesk", t => t.ServiceDeskId, cascadeDelete: true)
                .Index(t => t.ServiceDeskId)
                .Index(t => t.ServiceDeliveryOrganisationTypeId)
                .Index(t => t.ResolverGroupType_Id)
                .Index(t => t.ServiceComponent_Id)
                .Index(t => t.ServiceDeliveryUnitType_Id);
            
            CreateTable(
                "dbo.OperationalProcessType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperationalProcessTypeRefDataId = c.Int(nullable: false),
                        Resolver_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OperationalProcessTypeRefData", t => t.OperationalProcessTypeRefDataId, cascadeDelete: true)
                .ForeignKey("dbo.Resolver", t => t.Resolver_Id)
                .Index(t => t.OperationalProcessTypeRefDataId)
                .Index(t => t.Resolver_Id);
            
            CreateTable(
                "dbo.OperationalProcessTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperationalProcessTypeName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.OperationalProcessTypeName, unique: true);
            
            CreateTable(
                "dbo.ResolverGroupTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResolverGroupTypeName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ResolverGroupTypeName, unique: true);
            
            CreateTable(
                "dbo.ServiceActivity",
                c => new
                    {
                        ResolverId = c.Int(nullable: false),
                        ActivityName = c.String(nullable: false, maxLength: 500),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.ResolverId)
                .ForeignKey("dbo.Resolver", t => t.ResolverId)
                .Index(t => t.ResolverId);
            
            CreateTable(
                "dbo.ServiceComponent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComponentLevel = c.Int(nullable: false),
                        ParentServiceComponentId = c.Int(),
                        ComponentName = c.String(nullable: false, maxLength: 100),
                        ServiceFunctionId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                        ResolverId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceComponent", t => t.ParentServiceComponentId)
                .ForeignKey("dbo.ServiceFunction", t => t.ServiceFunctionId, cascadeDelete: true)
                .ForeignKey("dbo.Resolver", t => t.ResolverId)
                .Index(t => t.ParentServiceComponentId)
                .Index(t => t.ServiceFunctionId)
                .Index(t => t.ResolverId);
            
            CreateTable(
                "dbo.ServiceFunction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FunctionTypeId = c.Int(nullable: false),
                        AlternativeName = c.String(maxLength: 150),
                        ServiceDomainId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FunctionTypeRefData", t => t.FunctionTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ServiceDomain", t => t.ServiceDomainId, cascadeDelete: true)
                .Index(t => t.FunctionTypeId)
                .Index(t => t.ServiceDomainId);
            
            CreateTable(
                "dbo.FunctionTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FunctionName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FunctionName, unique: true);
            
            CreateTable(
                "dbo.ServiceDomain",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DomainTypeId = c.Int(nullable: false),
                        AlternativeName = c.String(maxLength: 150),
                        ServiceDeskId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainTypeRefData", t => t.DomainTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ServiceDesk", t => t.ServiceDeskId, cascadeDelete: true)
                .Index(t => t.DomainTypeId)
                .Index(t => t.ServiceDeskId);
            
            CreateTable(
                "dbo.DomainTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DomainName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.DomainName, unique: true);
            
            CreateTable(
                "dbo.ServiceDeliveryOrganisationTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDeliveryOrganisationTypeName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ServiceDeliveryOrganisationTypeName, unique: true);
            
            CreateTable(
                "dbo.ServiceDeliveryUnitTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDeliveryUnitTypeName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ServiceDeliveryUnitTypeName, unique: true);
            
            CreateTable(
                "dbo.CustomerPack",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Filename = c.String(nullable: false, maxLength: 250),
                        MimeType = c.String(nullable: false, maxLength: 100),
                        PackData = c.Binary(nullable: false),
                        PackNotes = c.String(maxLength: 250),
                        CustomerId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Diagram",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Filename = c.String(nullable: false, maxLength: 250),
                        MimeType = c.String(nullable: false, maxLength: 100),
                        DiagramData = c.Binary(nullable: false),
                        DiagramNotes = c.String(maxLength: 250),
                        CustomerId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParameterName = c.String(nullable: false, maxLength: 50),
                        ParameterValue = c.String(nullable: false, maxLength: 200),
                        Type = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ParameterName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Diagram", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.CustomerPack", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Resolver", "ServiceDeskId", "dbo.ServiceDesk");
            DropForeignKey("dbo.Resolver", "ServiceDeliveryUnitType_Id", "dbo.ServiceDeliveryUnitTypeRefData");
            DropForeignKey("dbo.Resolver", "ServiceDeliveryOrganisationTypeId", "dbo.ServiceDeliveryOrganisationTypeRefData");
            DropForeignKey("dbo.ServiceComponent", "ResolverId", "dbo.Resolver");
            DropForeignKey("dbo.ServiceFunction", "ServiceDomainId", "dbo.ServiceDomain");
            DropForeignKey("dbo.ServiceDomain", "ServiceDeskId", "dbo.ServiceDesk");
            DropForeignKey("dbo.ServiceDomain", "DomainTypeId", "dbo.DomainTypeRefData");
            DropForeignKey("dbo.ServiceComponent", "ServiceFunctionId", "dbo.ServiceFunction");
            DropForeignKey("dbo.ServiceFunction", "FunctionTypeId", "dbo.FunctionTypeRefData");
            DropForeignKey("dbo.Resolver", "ServiceComponent_Id", "dbo.ServiceComponent");
            DropForeignKey("dbo.ServiceComponent", "ParentServiceComponentId", "dbo.ServiceComponent");
            DropForeignKey("dbo.ServiceActivity", "ResolverId", "dbo.Resolver");
            DropForeignKey("dbo.Resolver", "ResolverGroupType_Id", "dbo.ResolverGroupTypeRefData");
            DropForeignKey("dbo.OperationalProcessType", "Resolver_Id", "dbo.Resolver");
            DropForeignKey("dbo.OperationalProcessType", "OperationalProcessTypeRefDataId", "dbo.OperationalProcessTypeRefData");
            DropForeignKey("dbo.DeskInputType", "ServiceDeskId", "dbo.ServiceDesk");
            DropForeignKey("dbo.DeskInputType", "InputTypeRefData_Id", "dbo.InputTypeRefData");
            DropForeignKey("dbo.ServiceDesk", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Contributor", "CustomerId", "dbo.Customer");
            DropIndex("dbo.Parameter", new[] { "ParameterName" });
            DropIndex("dbo.Diagram", new[] { "CustomerId" });
            DropIndex("dbo.CustomerPack", new[] { "CustomerId" });
            DropIndex("dbo.ServiceDeliveryUnitTypeRefData", new[] { "ServiceDeliveryUnitTypeName" });
            DropIndex("dbo.ServiceDeliveryOrganisationTypeRefData", new[] { "ServiceDeliveryOrganisationTypeName" });
            DropIndex("dbo.DomainTypeRefData", new[] { "DomainName" });
            DropIndex("dbo.ServiceDomain", new[] { "ServiceDeskId" });
            DropIndex("dbo.ServiceDomain", new[] { "DomainTypeId" });
            DropIndex("dbo.FunctionTypeRefData", new[] { "FunctionName" });
            DropIndex("dbo.ServiceFunction", new[] { "ServiceDomainId" });
            DropIndex("dbo.ServiceFunction", new[] { "FunctionTypeId" });
            DropIndex("dbo.ServiceComponent", new[] { "ResolverId" });
            DropIndex("dbo.ServiceComponent", new[] { "ServiceFunctionId" });
            DropIndex("dbo.ServiceComponent", new[] { "ParentServiceComponentId" });
            DropIndex("dbo.ServiceActivity", new[] { "ResolverId" });
            DropIndex("dbo.ResolverGroupTypeRefData", new[] { "ResolverGroupTypeName" });
            DropIndex("dbo.OperationalProcessTypeRefData", new[] { "OperationalProcessTypeName" });
            DropIndex("dbo.OperationalProcessType", new[] { "Resolver_Id" });
            DropIndex("dbo.OperationalProcessType", new[] { "OperationalProcessTypeRefDataId" });
            DropIndex("dbo.Resolver", new[] { "ServiceDeliveryUnitType_Id" });
            DropIndex("dbo.Resolver", new[] { "ServiceComponent_Id" });
            DropIndex("dbo.Resolver", new[] { "ResolverGroupType_Id" });
            DropIndex("dbo.Resolver", new[] { "ServiceDeliveryOrganisationTypeId" });
            DropIndex("dbo.Resolver", new[] { "ServiceDeskId" });
            DropIndex("dbo.InputTypeRefData", new[] { "InputTypeName" });
            DropIndex("dbo.DeskInputType", new[] { "InputTypeRefData_Id" });
            DropIndex("dbo.DeskInputType", new[] { "ServiceDeskId" });
            DropIndex("dbo.ServiceDesk", new[] { "CustomerId" });
            DropIndex("dbo.Customer", new[] { "CustomerName" });
            DropIndex("dbo.Contributor", new[] { "CustomerId" });
            DropIndex("dbo.ContextHelpRefData", new[] { "Key" });
            DropTable("dbo.Parameter");
            DropTable("dbo.Diagram");
            DropTable("dbo.CustomerPack");
            DropTable("dbo.ServiceDeliveryUnitTypeRefData");
            DropTable("dbo.ServiceDeliveryOrganisationTypeRefData");
            DropTable("dbo.DomainTypeRefData");
            DropTable("dbo.ServiceDomain");
            DropTable("dbo.FunctionTypeRefData");
            DropTable("dbo.ServiceFunction");
            DropTable("dbo.ServiceComponent");
            DropTable("dbo.ServiceActivity");
            DropTable("dbo.ResolverGroupTypeRefData");
            DropTable("dbo.OperationalProcessTypeRefData");
            DropTable("dbo.OperationalProcessType");
            DropTable("dbo.Resolver");
            DropTable("dbo.InputTypeRefData");
            DropTable("dbo.DeskInputType");
            DropTable("dbo.ServiceDesk");
            DropTable("dbo.Customer");
            DropTable("dbo.Contributor");
            DropTable("dbo.ContextHelpRefData");
        }
    }
}
