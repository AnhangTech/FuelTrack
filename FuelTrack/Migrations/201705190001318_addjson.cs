namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addjson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StationAccounts", "BankAccountName", c => c.String());
            AddColumn("dbo.StationAccounts", "BankAccountNumber", c => c.String());
            AddColumn("dbo.StationAccounts", "BankBranch", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StationAccounts", "BankBranch");
            DropColumn("dbo.StationAccounts", "BankAccountNumber");
            DropColumn("dbo.StationAccounts", "BankAccountName");
        }
    }
}
