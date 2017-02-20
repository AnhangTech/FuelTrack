namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptiontimestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSubscriptions", "Timestamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSubscriptions", "Timestamp");
        }
    }
}
