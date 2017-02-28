namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayRequestApprove : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequests", "WithdrawedTimestamp", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequests", "WithdrawedTimestamp");
        }
    }
}
