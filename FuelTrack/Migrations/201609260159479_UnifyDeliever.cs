namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnifyDeliever : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionHistories", "ClientSubscriptionId", c => c.Long());
            AddColumn("dbo.SubscriptionHistories", "ClientAccountName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubscriptionHistories", "ClientAccountName");
            DropColumn("dbo.SubscriptionHistories", "ClientSubscriptionId");
        }
    }
}
