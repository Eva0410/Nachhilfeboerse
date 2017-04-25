namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EMail = c.String(nullable: false),
                        PhoneNumber = c.String(),
                        Description = c.String(maxLength: 300),
                        Birthday = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Price = c.Double(nullable: false),
                        Department_Id = c.Int(nullable: false),
                        Class_Id = c.Int(nullable: false),
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
                "dbo.Subjects",
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tutor_Subject", "Tutor_Id", "dbo.Tutors");
            DropForeignKey("dbo.Tutor_Subject", "Subject_Id", "dbo.Subjects");
            DropForeignKey("dbo.Reviews", "Tutor_Id", "dbo.Tutors");
            DropForeignKey("dbo.Tutors", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.Tutors", "Class_Id", "dbo.SchoolClasses");
            DropIndex("dbo.Tutor_Subject", new[] { "Subject_Id" });
            DropIndex("dbo.Tutor_Subject", new[] { "Tutor_Id" });
            DropIndex("dbo.Tutors", new[] { "Class_Id" });
            DropIndex("dbo.Tutors", new[] { "Department_Id" });
            DropIndex("dbo.Reviews", new[] { "Tutor_Id" });
            DropTable("dbo.Tutor_Subject");
            DropTable("dbo.Subjects");
            DropTable("dbo.SchoolClasses");
            DropTable("dbo.Tutors");
            DropTable("dbo.Reviews");
            DropTable("dbo.Departments");
        }
    }
}
