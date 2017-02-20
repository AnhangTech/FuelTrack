namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubscriptionStation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "StartQuantity", c => c.Single(nullable: false));
            AddColumn("dbo.Subscriptions", "CurrentQuantity", c => c.Single(nullable: false));
            AddColumn("dbo.SubscriptionHistories", "VesselName", c => c.String());
            DropColumn("dbo.Subscriptions", "TotalQuantity");
            DropColumn("dbo.Subscriptions", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscriptions", "Quantity", c => c.Single(nullable: false));
            AddColumn("dbo.Subscriptions", "TotalQuantity", c => c.Single(nullable: false));
            DropColumn("dbo.SubscriptionHistories", "VesselName");
            DropColumn("dbo.Subscriptions", "CurrentQuantity");
            DropColumn("dbo.Subscriptions", "StartQuantity");
        }
    }
}
