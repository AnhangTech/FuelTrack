namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionhistory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClientPayments", "ClientSubscriptionHistoryId", "dbo.ClientSubscriptionHistories");
            DropIndex("dbo.ClientPayments", new[] { "ClientSubscriptionHistoryId" });
            AddColumn("dbo.ClientSubscriptionHistories", "UnitPrice", c => c.Single(nullable: false));
            AddColumn("dbo.ClientSubscriptionHistories", "Quantity", c => c.Single(nullable: false));
            AddColumn("dbo.ClientSubscriptionHistories", "State", c => c.Int(nullable: false));
            AddColumn("dbo.ClientSubscriptionHistories", "Notes", c => c.String());
            DropColumn("dbo.ClientSubscriptionHistories", "UnitPriceChange");
            DropColumn("dbo.ClientSubscriptionHistories", "QuantityChange");
            DropColumn("dbo.ClientSubscriptionHistories", "NewState");
            DropColumn("dbo.ClientSubscriptionHistories", "Note");
            DropTable("dbo.ClientPayments");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.ClientPaymentId);
            
            AddColumn("dbo.ClientSubscriptionHistories", "Note", c => c.String());
            AddColumn("dbo.ClientSubscriptionHistories", "NewState", c => c.String());
            AddColumn("dbo.ClientSubscriptionHistories", "QuantityChange", c => c.Single(nullable: false));
            AddColumn("dbo.ClientSubscriptionHistories", "UnitPriceChange", c => c.Single(nullable: false));
            DropColumn("dbo.ClientSubscriptionHistories", "Notes");
            DropColumn("dbo.ClientSubscriptionHistories", "State");
            DropColumn("dbo.ClientSubscriptionHistories", "Quantity");
            DropColumn("dbo.ClientSubscriptionHistories", "UnitPrice");
            CreateIndex("dbo.ClientPayments", "ClientSubscriptionHistoryId");
            AddForeignKey("dbo.ClientPayments", "ClientSubscriptionHistoryId", "dbo.ClientSubscriptionHistories", "ClientSubscriptionHistoryId", cascadeDelete: true);
        }
    }
}
