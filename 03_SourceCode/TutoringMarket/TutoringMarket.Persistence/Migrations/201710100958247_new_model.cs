namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new_model : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tutor_Subject", "Subject_Id", "dbo.Subjects");
            DropForeignKey("dbo.Tutor_Subject", "Tutor_Id", "dbo.Tutors");
            DropIndex("dbo.Tutor_Subject", new[] { "Tutor_Id" });
            DropIndex("dbo.Tutor_Subject", new[] { "Subject_Id" });
            CreateTable(
                "dbo.SubjectTutors",
                c => new
                    {
                        Subject_Id = c.Int(nullable: false),
                        Tutor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Subject_Id, t.Tutor_Id })
                .ForeignKey("dbo.Subjects", t => t.Subject_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tutors", t => t.Tutor_Id, cascadeDelete: true)
                .Index(t => t.Subject_Id)
                .Index(t => t.Tutor_Id);
            
            DropTable("dbo.Tutor_Subject");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Tutor_Subject",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tutor_Id = c.Int(nullable: false),
                        Subject_Id = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.SubjectTutors", "Tutor_Id", "dbo.Tutors");
            DropForeignKey("dbo.SubjectTutors", "Subject_Id", "dbo.Subjects");
            DropIndex("dbo.SubjectTutors", new[] { "Tutor_Id" });
            DropIndex("dbo.SubjectTutors", new[] { "Subject_Id" });
            DropTable("dbo.SubjectTutors");
            CreateIndex("dbo.Tutor_Subject", "Subject_Id");
            CreateIndex("dbo.Tutor_Subject", "Tutor_Id");
            AddForeignKey("dbo.Tutor_Subject", "Tutor_Id", "dbo.Tutors", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tutor_Subject", "Subject_Id", "dbo.Subjects", "Id", cascadeDelete: true);
        }
    }
}
