namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientAccounts",
                c => new
                    {
                        ClientAccountId = c.Long(nullable: false, identity: true),
                        ClientAccountName = c.String(),
                    })
                .PrimaryKey(t => t.ClientAccountId);
            
            CreateTable(
                "dbo.ClientDeposites",
                c => new
                    {
                        ClientDepositeId = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        ClientAccountId = c.Long(nullable: false),
                        StationAccountId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ClientDepositeId)
                .ForeignKey("dbo.ClientAccounts", t => t.ClientAccountId, cascadeDelete: true)
                .Index(t => t.ClientAccountId);
            
            CreateTable(
                "dbo.ClientDepositeHistories",
                c => new
                    {
                        ClientDepositeHistoryId = c.Long(nullable: false, identity: true),
                        ClientDepositeId = c.Long(nullable: false),
                        AmountChange = c.Double(nullable: false),
                        Note = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientDepositeHistoryId)
                .ForeignKey("dbo.ClientDeposites", t => t.ClientDepositeId, cascadeDelete: true)
                .Index(t => t.ClientDepositeId);
            
            CreateTable(
                "dbo.ClientSubscriptions",
                c => new
                    {
                        ClientSubscriptionId = c.Long(nullable: false, identity: true),
                        ClientAccountId = c.Long(nullable: false),
                        StationAccountId = c.Long(nullable: false),
                        Quantity = c.Single(nullable: false),
                        UnitPrice = c.Single(nullable: false),
                        State = c.String(),
                    })
                .PrimaryKey(t => t.ClientSubscriptionId)
                .ForeignKey("dbo.ClientAccounts", t => t.ClientAccountId, cascadeDelete: true)
                .Index(t => t.ClientAccountId);
            
            CreateTable(
                "dbo.ClientSubscriptionHistories",
                c => new
                    {
                        ClientSubscriptionHistoryId = c.Long(nullable: false, identity: true),
                        ClientSubscriptionId = c.Long(nullable: false),
                        UnitPriceChange = c.Single(nullable: false),
                        QuantityChange = c.Single(nullable: false),
                        NewState = c.String(),
                        Note = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientSubscriptionHistoryId)
                .ForeignKey("dbo.ClientSubscriptions", t => t.ClientSubscriptionId, cascadeDelete: true)
                .Index(t => t.ClientSubscriptionId);
            
            CreateTable(
                "dbo.ClientPayments",
                c => new
                    {
                        ClientPaymentId = c.Long(nullable: false, identity: true),
                        ClientSubscriptionHistoryId = c.Long(nullable: false),
                        ClientSubscriptionId = c.Long(nullable: false),
                        ClientAccountId = c.Long(nullable: false),
                        PaymentType = c.String(),
                        ClientDepositeHistory = c.Long(),
                        Amount = c.Double(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientPaymentId)
                .ForeignKey("dbo.ClientSubscriptionHistories", t => t.ClientSubscriptionHistoryId, cascadeDelete: true)
                .Index(t => t.ClientSubscriptionHistoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientSubscriptions", "ClientAccountId", "dbo.ClientAccounts");
            DropForeignKey("dbo.ClientSubscriptionHistories", "ClientSubscriptionId", "dbo.ClientSubscriptions");
            DropForeignKey("dbo.ClientPayments", "ClientSubscriptionHistoryId", "dbo.ClientSubscriptionHistories");
            DropForeignKey("dbo.ClientDeposites", "ClientAccountId", "dbo.ClientAccounts");
            DropForeignKey("dbo.ClientDepositeHistories", "ClientDepositeId", "dbo.ClientDeposites");
            DropIndex("dbo.ClientPayments", new[] { "ClientSubscriptionHistoryId" });
            DropIndex("dbo.ClientSubscriptionHistories", new[] { "ClientSubscriptionId" });
            DropIndex("dbo.ClientSubscriptions", new[] { "ClientAccountId" });
            DropIndex("dbo.ClientDepositeHistories", new[] { "ClientDepositeId" });
            DropIndex("dbo.ClientDeposites", new[] { "ClientAccountId" });
            DropTable("dbo.ClientPayments");
            DropTable("dbo.ClientSubscriptionHistories");
            DropTable("dbo.ClientSubscriptions");
            DropTable("dbo.ClientDepositeHistories");
            DropTable("dbo.ClientDeposites");
            DropTable("dbo.ClientAccounts");
        }
    }
}
