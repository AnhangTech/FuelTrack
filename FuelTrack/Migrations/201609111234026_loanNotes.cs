namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loanNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "Notes", c => c.String());
            AddColumn("dbo.LoanHistories", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanHistories", "Notes");
            DropColumn("dbo.Loans", "Notes");
        }
    }
}
