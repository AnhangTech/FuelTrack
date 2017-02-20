namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubscriptionVesselName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSubscriptions", "VesselName", c => c.String());
            AddColumn("dbo.ClientSubscriptionHistories", "VesselName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSubscriptionHistories", "VesselName");
            DropColumn("dbo.ClientSubscriptions", "VesselName");
        }
    }
}
