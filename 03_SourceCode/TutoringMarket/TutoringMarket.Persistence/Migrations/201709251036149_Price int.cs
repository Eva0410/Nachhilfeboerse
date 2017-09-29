namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Priceint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tutors", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tutors", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
