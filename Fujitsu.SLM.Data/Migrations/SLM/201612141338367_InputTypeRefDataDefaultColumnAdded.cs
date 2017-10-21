namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InputTypeRefDataDefaultColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InputTypeRefData", "Default", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InputTypeRefData", "Default");
        }
    }
}
