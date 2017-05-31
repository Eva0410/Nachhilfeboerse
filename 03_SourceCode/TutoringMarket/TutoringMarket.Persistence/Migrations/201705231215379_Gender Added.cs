namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenderAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "Gender", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutors", "Gender");
        }
    }
}
