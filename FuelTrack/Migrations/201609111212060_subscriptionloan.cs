namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionloan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        LoanId = c.Long(nullable: false, identity: true),
                        ClientAccountId = c.Long(nullable: false),
                        StartAmount = c.Double(nullable: false),
                        CurrentAmount = c.Double(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        FreeDays = c.Int(nullable: false),
                        InterestRate = c.Double(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LoanId)
                .ForeignKey("dbo.ClientAccounts", t => t.ClientAccountId, cascadeDelete: true)
                .Index(t => t.ClientAccountId);
            
            CreateTable(
                "dbo.LoanHistories",
                c => new
                    {
                        LoanHistoryId = c.Long(nullable: false, identity: true),
                        LoanId = c.Long(nullable: false),
                        ClientAccountId = c.Long(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        DealDate = c.DateTime(nullable: false),
                        Amount = c.Double(nullable: false),
                        Interest = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.LoanHistoryId)
                .ForeignKey("dbo.ClientAccounts", t => t.ClientAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Loans", t => t.LoanId, cascadeDelete: true)
                .Index(t => t.LoanId)
                .Index(t => t.ClientAccountId);
            
            AddColumn("dbo.ClientAccounts", "Loan", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoanHistories", "LoanId", "dbo.Loans");
            DropForeignKey("dbo.LoanHistories", "ClientAccountId", "dbo.ClientAccounts");
            DropForeignKey("dbo.Loans", "ClientAccountId", "dbo.ClientAccounts");
            DropIndex("dbo.LoanHistories", new[] { "ClientAccountId" });
            DropIndex("dbo.LoanHistories", new[] { "LoanId" });
            DropIndex("dbo.Loans", new[] { "ClientAccountId" });
            DropColumn("dbo.ClientAccounts", "Loan");
            DropTable("dbo.LoanHistories");
            DropTable("dbo.Loans");
        }
    }
}
