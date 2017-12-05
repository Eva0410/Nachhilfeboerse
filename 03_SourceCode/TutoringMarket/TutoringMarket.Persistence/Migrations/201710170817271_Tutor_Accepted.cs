namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tutor_Accepted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "Accepted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutors", "Accepted");
        }
    }
}
