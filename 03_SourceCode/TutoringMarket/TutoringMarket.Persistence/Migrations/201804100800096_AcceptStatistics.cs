namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AcceptStatistics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcceptStatistics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TutorAccepted = c.Boolean(),
                        ReviewAccepted = c.Boolean(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AcceptStatistics");
        }
    }
}
