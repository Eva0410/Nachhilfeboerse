namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Review_new2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reviews", "Author", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "Author", c => c.String(nullable: false));
        }
    }
}
