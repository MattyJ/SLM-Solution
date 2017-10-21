namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceActivitiesColumnAddedToServiceComponent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceComponent", "ServiceActivities", c => c.String(maxLength: 1500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceComponent", "ServiceActivities");
        }
    }
}
