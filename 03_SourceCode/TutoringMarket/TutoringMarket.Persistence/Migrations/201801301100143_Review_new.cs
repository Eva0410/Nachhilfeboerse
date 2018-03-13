namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Review_new : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "Author", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reviews", "Author");
        }
    }
}
