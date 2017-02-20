namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionsimplied : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClientDeposites", "ClientAccountId", "dbo.ClientAccounts");
            DropForeignKey("dbo.ClientDepositeHistories", "ClientDepositeId", "dbo.ClientDeposites");
            DropForeignKey("dbo.Payments", "SubscriptionHistoryId", "dbo.SubscriptionHistories");
            DropForeignKey("dbo.ClientDeposites", "StationAccountId", "dbo.StationAccounts");
            DropIndex("dbo.ClientDeposites", new[] { "ClientAccountId" });
            DropIndex("dbo.ClientDeposites", new[] { "StationAccountId" });
            DropIndex("dbo.ClientDepositeHistories", new[] { "ClientDepositeId" });
            DropIndex("dbo.Payments", new[] { "SubscriptionHistoryId" });
            DropTable("dbo.ClientDeposites");
            DropTable("dbo.ClientDepositeHistories");
            DropTable("dbo.Payments");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.PaymentId);
            
            CreateTable(
                "dbo.ClientDepositeHistories",
                c => new
                    {
                        ClientDepositeHistoryId = c.Long(nullable: false, identity: true),
                        ClientDepositeId = c.Long(nullable: false),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientDepositeHistoryId);
            
            CreateTable(
                "dbo.ClientDeposites",
                c => new
                    {
                        ClientDepositeId = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        ClientAccountId = c.Long(nullable: false),
                        StationAccountId = c.Long(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.ClientDepositeId);
            
            CreateIndex("dbo.Payments", "SubscriptionHistoryId");
            CreateIndex("dbo.ClientDepositeHistories", "ClientDepositeId");
            CreateIndex("dbo.ClientDeposites", "StationAccountId");
            CreateIndex("dbo.ClientDeposites", "ClientAccountId");
            AddForeignKey("dbo.ClientDeposites", "StationAccountId", "dbo.StationAccounts", "StationAccountId", cascadeDelete: true);
            AddForeignKey("dbo.Payments", "SubscriptionHistoryId", "dbo.SubscriptionHistories", "SubscriptionHistoryId", cascadeDelete: true);
            AddForeignKey("dbo.ClientDepositeHistories", "ClientDepositeId", "dbo.ClientDeposites", "ClientDepositeId", cascadeDelete: true);
            AddForeignKey("dbo.ClientDeposites", "ClientAccountId", "dbo.ClientAccounts", "ClientAccountId", cascadeDelete: true);
        }
    }
}
