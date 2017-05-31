namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tutor_Subject_Relationadded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Subjects", "Tutor_Id", "dbo.Tutors");
            DropIndex("dbo.Subjects", new[] { "Tutor_Id" });
            DropColumn("dbo.Subjects", "Tutor_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subjects", "Tutor_Id", c => c.Int());
            CreateIndex("dbo.Subjects", "Tutor_Id");
            AddForeignKey("dbo.Subjects", "Tutor_Id", "dbo.Tutors", "Id");
        }
    }
}
