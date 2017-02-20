namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionchange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StationAccounts",
                c => new
                    {
                        StationAccountId = c.Long(nullable: false, identity: true),
                        StationName = c.String(),
                    })
                .PrimaryKey(t => t.StationAccountId);
            
            CreateTable(
                "dbo.Deposites",
                c => new
                    {
                        DepositeId = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        StationAccountId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.DepositeId)
                .ForeignKey("dbo.StationAccounts", t => t.StationAccountId, cascadeDelete: true)
                .Index(t => t.StationAccountId);
            
            CreateTable(
                "dbo.DepositeHistories",
                c => new
                    {
                        DepositeHistoryId = c.Long(nullable: false, identity: true),
                        DepositeId = c.Long(nullable: false),
                        AmountChange = c.Double(nullable: false),
                        Note = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DepositeHistoryId)
                .ForeignKey("dbo.Deposites", t => t.DepositeId, cascadeDelete: true)
                .Index(t => t.DepositeId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Long(nullable: false, identity: true),
                        StationAccountId = c.Long(nullable: false),
                        Quantity = c.Single(nullable: false),
                        UnitPrice = c.Single(nullable: false),
                        State = c.String(),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.StationAccounts", t => t.StationAccountId, cascadeDelete: true)
                .Index(t => t.StationAccountId);
            
            CreateTable(
                "dbo.SubscriptionHistories",
                c => new
                    {
                        SubscriptionHistoryId = c.Long(nullable: false, identity: true),
                        SubscriptionId = c.Long(nullable: false),
                        UnitPriceChange = c.Single(nullable: false),
                        QuantityChange = c.Single(nullable: false),
                        NewState = c.String(),
                        Note = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionHistoryId)
                .ForeignKey("dbo.Subscriptions", t => t.SubscriptionId, cascadeDelete: true)
                .Index(t => t.SubscriptionId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Long(nullable: false, identity: true),
                        SubscriptionHistoryId = c.Long(nullable: false),
                        SubscriptionId = c.Long(nullable: false),
                        StationId = c.Long(nullable: false),
                        PaymentType = c.String(),
                        DepositeHistory = c.Long(),
                        Amount = c.Double(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.SubscriptionHistories", t => t.SubscriptionHistoryId, cascadeDelete: true)
                .Index(t => t.SubscriptionHistoryId);
            
            AlterColumn("dbo.ClientSubscriptions", "State", c => c.Int(nullable: false));
            CreateIndex("dbo.ClientSubscriptions", "StationAccountId");
            AddForeignKey("dbo.ClientSubscriptions", "StationAccountId", "dbo.StationAccounts", "StationAccountId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientSubscriptions", "StationAccountId", "dbo.StationAccounts");
            DropForeignKey("dbo.Subscriptions", "StationAccountId", "dbo.StationAccounts");
            DropForeignKey("dbo.SubscriptionHistories", "SubscriptionId", "dbo.Subscriptions");
            DropForeignKey("dbo.Payments", "SubscriptionHistoryId", "dbo.SubscriptionHistories");
            DropForeignKey("dbo.Deposites", "StationAccountId", "dbo.StationAccounts");
            DropForeignKey("dbo.DepositeHistories", "DepositeId", "dbo.Deposites");
            DropIndex("dbo.Payments", new[] { "SubscriptionHistoryId" });
            DropIndex("dbo.SubscriptionHistories", new[] { "SubscriptionId" });
            DropIndex("dbo.Subscriptions", new[] { "StationAccountId" });
            DropIndex("dbo.DepositeHistories", new[] { "DepositeId" });
            DropIndex("dbo.Deposites", new[] { "StationAccountId" });
            DropIndex("dbo.ClientSubscriptions", new[] { "StationAccountId" });
            AlterColumn("dbo.ClientSubscriptions", "State", c => c.String());
            DropTable("dbo.Payments");
            DropTable("dbo.SubscriptionHistories");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.DepositeHistories");
            DropTable("dbo.Deposites");
            DropTable("dbo.StationAccounts");
        }
    }
}
