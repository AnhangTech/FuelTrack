namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionDeposite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientDeposites", "Notes", c => c.String());
            AddColumn("dbo.ClientDepositeHistories", "Amount", c => c.Double(nullable: false));
            AddColumn("dbo.ClientDepositeHistories", "Notes", c => c.String());
            CreateIndex("dbo.ClientDeposites", "StationAccountId");
            AddForeignKey("dbo.ClientDeposites", "StationAccountId", "dbo.StationAccounts", "StationAccountId", cascadeDelete: true);
            DropColumn("dbo.ClientDepositeHistories", "AmountChange");
            DropColumn("dbo.ClientDepositeHistories", "Note");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClientDepositeHistories", "Note", c => c.String());
            AddColumn("dbo.ClientDepositeHistories", "AmountChange", c => c.Double(nullable: false));
            DropForeignKey("dbo.ClientDeposites", "StationAccountId", "dbo.StationAccounts");
            DropIndex("dbo.ClientDeposites", new[] { "StationAccountId" });
            DropColumn("dbo.ClientDepositeHistories", "Notes");
            DropColumn("dbo.ClientDepositeHistories", "Amount");
            DropColumn("dbo.ClientDeposites", "Notes");
        }
    }
}
