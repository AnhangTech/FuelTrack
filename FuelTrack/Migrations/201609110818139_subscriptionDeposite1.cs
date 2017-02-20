namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionDeposite1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClientBalanceHistories", "ClientAccount_ClientAccountId", "dbo.ClientAccounts");
            DropIndex("dbo.ClientBalanceHistories", new[] { "ClientAccount_ClientAccountId" });
            RenameColumn(table: "dbo.ClientBalanceHistories", name: "ClientAccount_ClientAccountId", newName: "ClientAccountId");
            AlterColumn("dbo.ClientBalanceHistories", "BalanceType", c => c.Int(nullable: false));
            AlterColumn("dbo.ClientBalanceHistories", "ClientAccountId", c => c.Long(nullable: false));
            CreateIndex("dbo.ClientBalanceHistories", "ClientAccountId");
            AddForeignKey("dbo.ClientBalanceHistories", "ClientAccountId", "dbo.ClientAccounts", "ClientAccountId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientBalanceHistories", "ClientAccountId", "dbo.ClientAccounts");
            DropIndex("dbo.ClientBalanceHistories", new[] { "ClientAccountId" });
            AlterColumn("dbo.ClientBalanceHistories", "ClientAccountId", c => c.Long());
            AlterColumn("dbo.ClientBalanceHistories", "BalanceType", c => c.String());
            RenameColumn(table: "dbo.ClientBalanceHistories", name: "ClientAccountId", newName: "ClientAccount_ClientAccountId");
            CreateIndex("dbo.ClientBalanceHistories", "ClientAccount_ClientAccountId");
            AddForeignKey("dbo.ClientBalanceHistories", "ClientAccount_ClientAccountId", "dbo.ClientAccounts", "ClientAccountId");
        }
    }
}
