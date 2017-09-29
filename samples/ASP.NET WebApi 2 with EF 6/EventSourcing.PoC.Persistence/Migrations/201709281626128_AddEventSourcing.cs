using System.Data.Entity.Migrations;

namespace EventSourcing.PoC.Persistence.Migrations
{

    public partial class AddEventSourcing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                {
                    EventId = c.Long(nullable: false, identity: true),
                    Timestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    StreamId = c.Long(nullable: false),
                    Version = c.Long(nullable: false),
                    Type = c.String(nullable: false),
                    Payload = c.Binary(nullable: false),
                })
                .PrimaryKey(t => t.EventId)
                .Index(t => t.StreamId)
                .Index(t => t.Version);
        }

        public override void Down()
        {
            DropIndex("dbo.Events", new[] { "Version" });
            DropIndex("dbo.Events", new[] { "StreamId" });
            DropTable("dbo.Events");
        }
    }
}
