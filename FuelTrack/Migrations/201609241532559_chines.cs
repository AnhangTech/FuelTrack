namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chines : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionHistories", "StationAccountId", c => c.Long());
            DropColumn("dbo.SubscriptionHistories", "ClientAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubscriptionHistories", "ClientAccountId", c => c.Long());
            DropColumn("dbo.SubscriptionHistories", "StationAccountId");
        }
    }
}
