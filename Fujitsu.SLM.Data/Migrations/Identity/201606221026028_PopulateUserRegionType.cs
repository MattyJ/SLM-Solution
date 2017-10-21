namespace Fujitsu.SLM.Data.Migrations.Identity
{
    using System.Data.Entity.Migrations;

    public partial class PopulateUserRegionType : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [dbo].[AspNetUsers] SET [dbo].[AspNetUsers].[RegionTypeId] = [dbo].[RegionTypeRefData].[Id] FROM  [dbo].[AspNetUsers] INNER JOIN [dbo].[RegionTypeRefData] ON [dbo].[RegionTypeRefData].RegionName = 'Global Delivery'");
        }

        public override void Down()
        {
            Sql("UPDATE [dbo].[AspNetUsers] SET [RegionTypeId] = NULL");
        }
    }
}
