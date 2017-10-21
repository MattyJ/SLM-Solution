namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServiceDeliveryUnitTypeVisibleColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceDeliveryUnitTypeRefData", "Visible", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceDeliveryUnitTypeRefData", "Visible");
        }
    }
}
