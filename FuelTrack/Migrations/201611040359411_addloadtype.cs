namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addloadtype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanHistories", "ChangeType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanHistories", "ChangeType");
        }
    }
}
