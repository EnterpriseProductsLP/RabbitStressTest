using FluentMigrator;

namespace Migrations
{
    [TimestampedMigration(2016, 08, 15, 18, 26, "Eric Burcham")]
    public class Migration_20160815_1826_AddUnconsumedMessagesTable : Migration
    {
        public override void Up()
        {
            Create.Table("UnconsumedMessages").InSchema("dbo")
                .WithColumn("UnconsumedMessages").AsInt64().PrimaryKey("PK_UnconsumedMessages")
                .WithColumn("MessageName").AsString(100);

            Create.Index("UX_UnconsumedMessages")
                .OnTable("UnconsumedMessages")
                .InSchema("dbo")
                .OnColumn("MessageName")
                .Unique();
        }

        public override void Down()
        {
            Delete.Table("UnconsumedMessages").InSchema("dbo");
        }
    }
}
