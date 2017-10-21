namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System.Data.Entity.Migrations;

    public partial class AddAuditTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Audit",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Version = c.Double(nullable: false),
                    ReasonForIssue = c.String(nullable: false, maxLength: 100),
                    Notes = c.String(maxLength: 250),
                    InsertedDate = c.DateTime(nullable: false),
                    InsertedBy = c.String(nullable: false, maxLength: 150),
                    UpdatedDate = c.DateTime(nullable: false),
                    UpdatedBy = c.String(nullable: false, maxLength: 150),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.Audit");
        }
    }
}
