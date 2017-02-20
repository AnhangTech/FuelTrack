namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBalance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientBalanceHistories",
                c => new
                    {
                        ClientBalanceHistoryId = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        BalanceType = c.String(),
                        Description = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        ClientAccount_ClientAccountId = c.Long(),
                    })
                .PrimaryKey(t => t.ClientBalanceHistoryId)
                .ForeignKey("dbo.ClientAccounts", t => t.ClientAccount_ClientAccountId)
                .Index(t => t.ClientAccount_ClientAccountId);
            
            AddColumn("dbo.ClientAccounts", "Balance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientBalanceHistories", "ClientAccount_ClientAccountId", "dbo.ClientAccounts");
            DropIndex("dbo.ClientBalanceHistories", new[] { "ClientAccount_ClientAccountId" });
            DropColumn("dbo.ClientAccounts", "Balance");
            DropTable("dbo.ClientBalanceHistories");
        }
    }
}
