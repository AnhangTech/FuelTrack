namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fullcatch : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DepositeHistories", "DepositeId", "dbo.Deposites");
            DropIndex("dbo.Deposites", new[] { "StationAccountId" });
            DropIndex("dbo.DepositeHistories", new[] { "DepositeId" });
            AddColumn("dbo.DepositeHistories", "StationAccountId", c => c.Long(nullable: false));
            AddColumn("dbo.DepositeHistories", "Amount", c => c.Double(nullable: false));
            AddColumn("dbo.DepositeHistories", "ChangeType", c => c.Int(nullable: false));
            AddColumn("dbo.DepositeHistories", "Description", c => c.String());
            AddColumn("dbo.Subscriptions", "TotalQuantity", c => c.Single(nullable: false));
            AddColumn("dbo.Subscriptions", "Notes", c => c.String());
            AddColumn("dbo.Subscriptions", "Timestamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.SubscriptionHistories", "ClientAccountId", c => c.Long());
            AddColumn("dbo.SubscriptionHistories", "UnitPrice", c => c.Single(nullable: false));
            AddColumn("dbo.SubscriptionHistories", "Quantity", c => c.Single(nullable: false));
            AddColumn("dbo.SubscriptionHistories", "State", c => c.Int(nullable: false));
            AddColumn("dbo.SubscriptionHistories", "Notes", c => c.String());
            AlterColumn("dbo.Subscriptions", "State", c => c.Int(nullable: false));
            CreateIndex("dbo.DepositeHistories", "StationAccountId");
            DropColumn("dbo.DepositeHistories", "DepositeId");
            DropColumn("dbo.DepositeHistories", "AmountChange");
            DropColumn("dbo.DepositeHistories", "Note");
            DropColumn("dbo.SubscriptionHistories", "UnitPriceChange");
            DropColumn("dbo.SubscriptionHistories", "QuantityChange");
            DropColumn("dbo.SubscriptionHistories", "NewState");
            DropColumn("dbo.SubscriptionHistories", "Note");
            DropTable("dbo.Deposites");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Deposites",
                c => new
                    {
                        DepositeId = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        StationAccountId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.DepositeId);
            
            AddColumn("dbo.SubscriptionHistories", "Note", c => c.String());
            AddColumn("dbo.SubscriptionHistories", "NewState", c => c.String());
            AddColumn("dbo.SubscriptionHistories", "QuantityChange", c => c.Single(nullable: false));
            AddColumn("dbo.SubscriptionHistories", "UnitPriceChange", c => c.Single(nullable: false));
            AddColumn("dbo.DepositeHistories", "Note", c => c.String());
            AddColumn("dbo.DepositeHistories", "AmountChange", c => c.Double(nullable: false));
            AddColumn("dbo.DepositeHistories", "DepositeId", c => c.Long(nullable: false));
            DropIndex("dbo.DepositeHistories", new[] { "StationAccountId" });
            AlterColumn("dbo.Subscriptions", "State", c => c.String());
            DropColumn("dbo.SubscriptionHistories", "Notes");
            DropColumn("dbo.SubscriptionHistories", "State");
            DropColumn("dbo.SubscriptionHistories", "Quantity");
            DropColumn("dbo.SubscriptionHistories", "UnitPrice");
            DropColumn("dbo.SubscriptionHistories", "ClientAccountId");
            DropColumn("dbo.Subscriptions", "Timestamp");
            DropColumn("dbo.Subscriptions", "Notes");
            DropColumn("dbo.Subscriptions", "TotalQuantity");
            DropColumn("dbo.DepositeHistories", "Description");
            DropColumn("dbo.DepositeHistories", "ChangeType");
            DropColumn("dbo.DepositeHistories", "Amount");
            DropColumn("dbo.DepositeHistories", "StationAccountId");
            CreateIndex("dbo.DepositeHistories", "DepositeId");
            CreateIndex("dbo.Deposites", "StationAccountId");
            AddForeignKey("dbo.DepositeHistories", "DepositeId", "dbo.Deposites", "DepositeId", cascadeDelete: true);
        }
    }
}
