namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deposite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StationAccounts", "Deposite", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StationAccounts", "Deposite");
        }
    }
}
