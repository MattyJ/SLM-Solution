namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddResolverGroupTypeVisibleColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResolverGroupTypeRefData", "Visible", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ResolverGroupTypeRefData", "Visible");
        }
    }
}
