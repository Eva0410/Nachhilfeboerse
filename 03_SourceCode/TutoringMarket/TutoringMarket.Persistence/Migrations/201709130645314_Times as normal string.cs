namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Timesasnormalstring : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "Time", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutors", "Time");
        }
    }
}
