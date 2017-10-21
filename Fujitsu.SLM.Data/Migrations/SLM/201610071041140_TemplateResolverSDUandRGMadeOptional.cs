namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateResolverSDUandRGMadeOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TemplateResolver", "ServiceDeliveryUnitName", c => c.String(maxLength: 100));
            AlterColumn("dbo.TemplateResolver", "ResolverGroupName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TemplateResolver", "ResolverGroupName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.TemplateResolver", "ServiceDeliveryUnitName", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
