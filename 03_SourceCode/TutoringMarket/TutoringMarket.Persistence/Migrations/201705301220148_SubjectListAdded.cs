namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubjectListAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subjects", "Tutor_Id", c => c.Int());
            CreateIndex("dbo.Subjects", "Tutor_Id");
            AddForeignKey("dbo.Subjects", "Tutor_Id", "dbo.Tutors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subjects", "Tutor_Id", "dbo.Tutors");
            DropIndex("dbo.Subjects", new[] { "Tutor_Id" });
            DropColumn("dbo.Subjects", "Tutor_Id");
        }
    }
}
