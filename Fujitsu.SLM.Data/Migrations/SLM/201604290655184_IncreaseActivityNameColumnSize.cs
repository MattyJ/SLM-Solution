namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseActivityNameColumnSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceActivity", "ActivityName", c => c.String(nullable: false, maxLength: 1500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceActivity", "ActivityName", c => c.String(nullable: false, maxLength: 500));
        }
    }
}
