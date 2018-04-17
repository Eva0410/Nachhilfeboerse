namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TutorRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TutorRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Tutor_Id = c.Int(nullable: false),
                        Requestor = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tutors", t => t.Tutor_Id, cascadeDelete: true)
                .Index(t => t.Tutor_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TutorRequests", "Tutor_Id", "dbo.Tutors");
            DropIndex("dbo.TutorRequests", new[] { "Tutor_Id" });
            DropTable("dbo.TutorRequests");
        }
    }
}
