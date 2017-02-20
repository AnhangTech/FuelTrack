namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubscriptionAddStartQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSubscriptions", "StartQuantity", c => c.Single(nullable: false));
            AddColumn("dbo.ClientSubscriptions", "CurrentQuantity", c => c.Single(nullable: false));
            DropColumn("dbo.ClientSubscriptions", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClientSubscriptions", "Quantity", c => c.Single(nullable: false));
            DropColumn("dbo.ClientSubscriptions", "CurrentQuantity");
            DropColumn("dbo.ClientSubscriptions", "StartQuantity");
        }
    }
}
