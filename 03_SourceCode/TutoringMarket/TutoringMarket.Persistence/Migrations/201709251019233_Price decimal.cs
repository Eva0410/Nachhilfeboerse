namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pricedecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tutors", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tutors", "Price", c => c.Double(nullable: false));
        }
    }
}
