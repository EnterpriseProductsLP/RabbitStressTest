using FluentMigrator;

namespace Migrations
{
    [TimestampedMigration(2016, 08, 15, 20, 26, "Eric Burcham")]
    public class Migration_20160815_2026_AddInsertAndDeleteStoredProcedures : Migration
    {
        public override void Up()
        {
            Execute.Script(@"Scripts\Create_spUnconsumedMessageInsert.sql");
            Execute.Script(@"Scripts\Create_spUnconsumedMessageDelete.sql");
        }

        public override void Down()
        {
            Execute.Script(@"Scripts\Drop_spUnconsumedMessageInsert.sql");
            Execute.Script(@"Scripts\Drop_spUnconsumedMessageDelete.sql");
        }
    }
}
