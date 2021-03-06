namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageBackToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tutors", "Image", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tutors", "Image", c => c.Binary());
        }
    }
}
