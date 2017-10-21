namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddspDeleteCustomer : DbMigration
    {
        public override void Up()
        {
            Sql(DatabaseResources.Create_spDeleteCustomer);
        }

        public override void Down()
        {
            Sql(DatabaseResources.Drop_spDeleteCustomer);
        }
    }
}
