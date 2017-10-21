namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System.Data.Entity.Migrations;

    public partial class AddspModifyDeleteServiceDeskContents : DbMigration
    {
        public override void Up()
        {
            Sql(DatabaseResources.Modify_spDeleteServiceDeskContents);
        }

        public override void Down()
        {
            Sql(DatabaseResources.Drop_spDeleteServiceDeskContents);
            Sql(DatabaseResources.Create_spDeleteServiceDeskContents);
        }
    }
}
