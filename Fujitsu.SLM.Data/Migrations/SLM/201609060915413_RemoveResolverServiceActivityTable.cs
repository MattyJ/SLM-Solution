namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveResolverServiceActivityTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceActivity", "ResolverId", "dbo.Resolver");
            DropIndex("dbo.ServiceActivity", new[] { "ResolverId" });
            DropTable("dbo.ServiceActivity");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ServiceActivity",
                c => new
                    {
                        ResolverId = c.Int(nullable: false),
                        ActivityName = c.String(nullable: false, maxLength: 1500),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.ResolverId);
            
            CreateIndex("dbo.ServiceActivity", "ResolverId");
            AddForeignKey("dbo.ServiceActivity", "ResolverId", "dbo.Resolver", "Id");
        }
    }
}
