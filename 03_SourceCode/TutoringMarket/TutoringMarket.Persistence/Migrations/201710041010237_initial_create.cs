namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Books = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        Tutor_Id = c.Int(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tutors", t => t.Tutor_Id, cascadeDelete: true)
                .Index(t => t.Tutor_Id);
            
            CreateTable(
                "dbo.Tutors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.Binary(),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EMail = c.String(nullable: false, maxLength: 150),
                        PhoneNumber = c.String(maxLength: 15),
                        Description = c.String(maxLength: 500),
                        Birthday = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Time = c.String(nullable: false, maxLength: 150),
                        Price = c.Int(nullable: false),
                        Department_Id = c.Int(nullable: false),
                        Class_Id = c.Int(nullable: false),
                        IdentityName = c.String(),
                        Gender = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SchoolClasses", t => t.Class_Id, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.Department_Id, cascadeDelete: true)
                .Index(t => t.Department_Id)
                .Index(t => t.Class_Id);
            
            CreateTable(
                "dbo.SchoolClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tutor_Subject",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tutor_Id = c.Int(nullable: false),
                        Subject_Id = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subjects", t => t.Subject_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tutors", t => t.Tutor_Id, cascadeDelete: true)
                .Index(t => t.Tutor_Id)
                .Index(t => t.Subject_Id);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "Tutor_Id", "dbo.Tutors");
            DropForeignKey("dbo.Tutor_Subject", "Tutor_Id", "dbo.Tutors");
            DropForeignKey("dbo.Tutor_Subject", "Subject_Id", "dbo.Subjects");
            DropForeignKey("dbo.Tutors", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.Tutors", "Class_Id", "dbo.SchoolClasses");
            DropIndex("dbo.Tutor_Subject", new[] { "Subject_Id" });
            DropIndex("dbo.Tutor_Subject", new[] { "Tutor_Id" });
            DropIndex("dbo.Tutors", new[] { "Class_Id" });
            DropIndex("dbo.Tutors", new[] { "Department_Id" });
            DropIndex("dbo.Reviews", new[] { "Tutor_Id" });
            DropTable("dbo.Subjects");
            DropTable("dbo.Tutor_Subject");
            DropTable("dbo.SchoolClasses");
            DropTable("dbo.Tutors");
            DropTable("dbo.Reviews");
            DropTable("dbo.Departments");
        }
    }
}
