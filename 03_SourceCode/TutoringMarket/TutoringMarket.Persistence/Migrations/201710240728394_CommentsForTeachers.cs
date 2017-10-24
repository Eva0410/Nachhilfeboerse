namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentsForTeachers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeacherComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(nullable: false),
                        TeacherIdentityName = c.String(nullable: false),
                        Tutor_Id = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tutors", t => t.Tutor_Id, cascadeDelete: true)
                .Index(t => t.Tutor_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeacherComments", "Tutor_Id", "dbo.Tutors");
            DropIndex("dbo.TeacherComments", new[] { "Tutor_Id" });
            DropTable("dbo.TeacherComments");
        }
    }
}
