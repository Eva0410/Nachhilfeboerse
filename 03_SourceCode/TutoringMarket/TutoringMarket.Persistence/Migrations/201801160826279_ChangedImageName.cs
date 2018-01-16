namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedImageName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "Image", c => c.Binary());
            DropColumn("dbo.Tutors", "AvatarImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tutors", "AvatarImage", c => c.Binary());
            DropColumn("dbo.Tutors", "Image");
        }
    }
}
