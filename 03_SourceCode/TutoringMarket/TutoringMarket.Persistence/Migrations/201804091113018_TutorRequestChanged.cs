namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TutorRequestChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TutorRequests", "SchoolClass", c => c.String());
            DropColumn("dbo.TutorRequests", "Requestor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TutorRequests", "Requestor", c => c.String());
            DropColumn("dbo.TutorRequests", "SchoolClass");
        }
    }
}
