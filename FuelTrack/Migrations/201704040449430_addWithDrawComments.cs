namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addWithDrawComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequests", "Notes", c => c.String());
            AddColumn("dbo.PaymentRequests", "BankAccountName", c => c.String());
            AddColumn("dbo.PaymentRequests", "BankAccountNumber", c => c.String());
            AddColumn("dbo.PaymentRequests", "BankBranch", c => c.String());
            AddColumn("dbo.PaymentRequests", "WithdrawedComments", c => c.String());
            AddColumn("dbo.PaymentRequests", "WithdrawedTimestamp", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequests", "WithdrawedTimestamp");
            DropColumn("dbo.PaymentRequests", "WithdrawedComments");
            DropColumn("dbo.PaymentRequests", "BankBranch");
            DropColumn("dbo.PaymentRequests", "BankAccountNumber");
            DropColumn("dbo.PaymentRequests", "BankAccountName");
            DropColumn("dbo.PaymentRequests", "Notes");
        }
    }
}
