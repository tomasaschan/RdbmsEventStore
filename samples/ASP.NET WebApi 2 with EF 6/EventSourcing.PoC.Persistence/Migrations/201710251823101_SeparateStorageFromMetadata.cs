namespace EventSourcing.PoC.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeparateStorageFromMetadata : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Events", new[] { "StreamId" });
            DropIndex("dbo.Events", new[] { "Version" });
            CreateIndex("dbo.Events", "StreamId");
            CreateIndex("dbo.Events", "Version");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Events", new[] { "Version" });
            DropIndex("dbo.Events", new[] { "StreamId" });
            CreateIndex("dbo.Events", "Version");
            CreateIndex("dbo.Events", "StreamId");
        }
    }
}
