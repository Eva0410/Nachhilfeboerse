namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tmp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "AvatarImage", c => c.Binary());
            DropColumn("dbo.Tutors", "Image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tutors", "Image", c => c.String());
            DropColumn("dbo.Tutors", "AvatarImage");
        }
    }
}
