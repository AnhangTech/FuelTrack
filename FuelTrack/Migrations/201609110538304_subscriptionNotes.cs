namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSubscriptions", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSubscriptions", "Notes");
        }
    }
}
