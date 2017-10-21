namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System.Data.Entity.Migrations;

    public partial class AddspDeleteStoredProcedures : DbMigration
    {
        public override void Up()
        {
            Sql(DatabaseResources.Create_spDeleteResolver);
            Sql(DatabaseResources.Create_spDeleteServiceComponent);
            Sql(DatabaseResources.Create_spDeleteServiceFunction);
            Sql(DatabaseResources.Create_spDeleteServiceDomain);
            Sql(DatabaseResources.Create_spDeleteServiceDesk);
            Sql(DatabaseResources.Create_spDeleteServiceDeskContents);
            Sql(DatabaseResources.Modify_spDeleteCustomer);
        }

        public override void Down()
        {
            Sql(DatabaseResources.Create_spDeleteCustomer);
            Sql(DatabaseResources.Drop_spDeleteCustomer);
            Sql(DatabaseResources.Drop_spDeleteServiceDeskContents);
            Sql(DatabaseResources.Drop_spDeleteServiceDesk);
            Sql(DatabaseResources.Drop_spDeleteServiceDomain);
            Sql(DatabaseResources.Drop_spDeleteServiceFunction);
            Sql(DatabaseResources.Drop_spDeleteServiceComponent);
            Sql(DatabaseResources.Drop_spDeleteResolver);
        }
    }
}
