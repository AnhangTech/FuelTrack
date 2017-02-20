namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addvalidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ClientAccounts", "ClientAccountName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ClientAccounts", "ClientAccountName", c => c.String());
        }
    }
}
