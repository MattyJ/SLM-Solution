namespace Fujitsu.SLM.Data.Migrations.SLM
{
    using System.Data.Entity.Migrations;

    public partial class AddCustomerSpecificTypesParameter : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[Parameter] ([ParameterName],[ParameterValue],[Type],[InsertedDate],[InsertedBy],[UpdatedDate],[UpdatedBy]) VALUES " +
                "('CustomerSpecificTypeThreshold', '3' ,0, GETDATE(), SYSTEM_USER, GETDATE(), SYSTEM_USER)");
        }

        public override void Down()
        {
            Sql("DELETE FROM [dbo].[Parameter] WHERE [ParameterName] = 'CustomerSpecificTypeThreshold')");
        }
    }
}
