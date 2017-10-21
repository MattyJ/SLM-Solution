namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System.Data.Entity.Migrations;

    public partial class UpdateServiceComponentActivities : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE sc SET sc.ServiceActivities = sa.ActivityName FROM ServiceComponent AS sc INNER JOIN Resolver AS r ON r.ServiceComponent_Id = sc.Id  INNER JOIN ServiceActivity AS sa ON sa.ResolverId = r.Id WHERE r.ServiceComponent_Id IS NOT NULL");
        }

        public override void Down()
        {
        }
    }
}
