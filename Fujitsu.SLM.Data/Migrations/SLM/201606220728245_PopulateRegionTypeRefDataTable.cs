namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System.Data.Entity.Migrations;

    public partial class PopulateRegionTypeRefDataTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[RegionTypeRefData]([RegionName] ,[SortOrder]) VALUES ('Japan', 5)");
            Sql("INSERT INTO [dbo].[RegionTypeRefData]([RegionName] ,[SortOrder]) VALUES ('Americas', 5)");
            Sql("INSERT INTO [dbo].[RegionTypeRefData]([RegionName] ,[SortOrder]) VALUES ('EMEIA', 5)");
            Sql("INSERT INTO [dbo].[RegionTypeRefData]([RegionName] ,[SortOrder]) VALUES ('Asia-Pac', 5)");
            Sql("INSERT INTO [dbo].[RegionTypeRefData]([RegionName] ,[SortOrder]) VALUES ('Oceania', 5)");
            Sql("INSERT INTO [dbo].[RegionTypeRefData]([RegionName] ,[SortOrder]) VALUES ('Global Delivery', 5)");
        }

        public override void Down()
        {
            Sql("DELETE FROM [dbo].[RegionTypeRefData]");
        }
    }
}
