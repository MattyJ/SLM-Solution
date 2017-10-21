namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColumnLengthSizeChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Resolver", "ServiceDeliveryUnitNotes", c => c.String(maxLength: 1500));
            AlterColumn("dbo.ServiceFunction", "AlternativeName", c => c.String(maxLength: 100));
            AlterColumn("dbo.ServiceDomain", "AlternativeName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceDomain", "AlternativeName", c => c.String(maxLength: 150));
            AlterColumn("dbo.ServiceFunction", "AlternativeName", c => c.String(maxLength: 150));
            AlterColumn("dbo.Resolver", "ServiceDeliveryUnitNotes", c => c.String(maxLength: 250));
        }
    }
}
