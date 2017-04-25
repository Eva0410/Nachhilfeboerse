namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bild : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutors", "Image");
        }
    }
}
