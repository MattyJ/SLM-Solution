namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerAuditRelationshipAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Audit", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Audit", "CustomerId");
            AddForeignKey("dbo.Audit", "CustomerId", "dbo.Customer", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Audit", "CustomerId", "dbo.Customer");
            DropIndex("dbo.Audit", new[] { "CustomerId" });
            DropColumn("dbo.Audit", "CustomerId");
        }
    }
}
