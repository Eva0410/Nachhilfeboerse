namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeacherStatistics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeacherCommentStatisticEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeacherIdentityName = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TeacherCommentStatisticEntries");
        }
    }
}
