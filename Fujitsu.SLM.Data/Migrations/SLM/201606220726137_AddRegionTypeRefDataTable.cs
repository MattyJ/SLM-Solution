namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegionTypeRefDataTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegionTypeRefData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RegionName = c.String(nullable: false, maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.RegionName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.RegionTypeRefData", new[] { "RegionName" });
            DropTable("dbo.RegionTypeRefData");
        }
    }
}
