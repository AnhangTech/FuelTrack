namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoanLimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientAccounts", "LoanLimit", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientAccounts", "LoanLimit");
        }
    }
}
