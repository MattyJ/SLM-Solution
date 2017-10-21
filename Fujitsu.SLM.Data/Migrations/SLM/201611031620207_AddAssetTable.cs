namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Asset",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileExtension = c.String(),
                        OriginalFileName = c.String(),
                        FullPath = c.String(),
                        MimeType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ContextHelpRefData", "AssetId", c => c.Int());
            CreateIndex("dbo.ContextHelpRefData", "AssetId");
            AddForeignKey("dbo.ContextHelpRefData", "AssetId", "dbo.Asset", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContextHelpRefData", "AssetId", "dbo.Asset");
            DropIndex("dbo.ContextHelpRefData", new[] { "AssetId" });
            DropColumn("dbo.ContextHelpRefData", "AssetId");
            DropTable("dbo.Asset");
        }
    }
}
