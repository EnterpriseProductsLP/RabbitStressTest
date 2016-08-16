using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace Migrations
{
    [TimestampedMigration(2016, 08, 15, 18, 26, "Eric Burcham")]
    public class Migration_20160815_1826_AddUnconsumedMessageTable : Migration
    {
        public override void Up()
        {
            Create.Table("UnconsumedMessage").InSchema("dbo")
                .WithColumn("UnconsumedMessageId").AsInt64().PrimaryKey("PK_UnconsumedMessage").Identity(1, 1)
                .WithColumn("ClientName").AsString(100)
                .WithColumn("MessageId").AsGuid();

            Create.Index("UX_UnconsumedMessage")
                .OnTable("UnconsumedMessage")
                .InSchema("dbo")
                .OnColumn("ClientName").Ascending()
                .OnColumn("MessageId")
                .Unique();
        }

        public override void Down()
        {
            Delete.Table("UnconsumedMessage").InSchema("dbo");
        }
    }
}
