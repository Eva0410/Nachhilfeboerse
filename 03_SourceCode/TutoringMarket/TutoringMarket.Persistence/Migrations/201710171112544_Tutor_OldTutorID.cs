namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tutor_OldTutorID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "OldTutorId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutors", "OldTutorId");
        }
    }
}
