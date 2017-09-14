namespace TutoringMarket.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Genderedited : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tutors", "Description", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tutors", "Description", c => c.String(maxLength: 300));
        }
    }
}
