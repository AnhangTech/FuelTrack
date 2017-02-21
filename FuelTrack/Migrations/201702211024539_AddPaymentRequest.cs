namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaymentRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentRequests",
                c => new
                    {
                        PaymentRequestId = c.Long(nullable: false, identity: true),
                        StationAccountId = c.Long(nullable: false),
                        Amount = c.Double(nullable: false),
                        State = c.Int(nullable: false),
                        Reason = c.String(),
                        EmployeeId = c.String(),
                        StartTimestamp = c.DateTime(nullable: false),
                        FinanceManagerId = c.String(),
                        FinanceManagerComments = c.String(),
                        FinanceManagerCommentsTimestamp = c.DateTime(),
                        BusinessManagerId = c.String(),
                        BusinessManagerComments = c.String(),
                        BusinessManagerCommentsTimestamp = c.DateTime(),
                        Notes = c.String(),
                        BankAccountName = c.String(),
                        BankAccountNumber = c.String(),
                        BankBranch = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestId)
                .ForeignKey("dbo.StationAccounts", t => t.StationAccountId, cascadeDelete: true)
                .Index(t => t.StationAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentRequests", "StationAccountId", "dbo.StationAccounts");
            DropIndex("dbo.PaymentRequests", new[] { "StationAccountId" });
            DropTable("dbo.PaymentRequests");
        }
    }
}
