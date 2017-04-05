namespace FuelTrack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAmountToDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PaymentRequests", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PaymentRequests", "Amount", c => c.Double(nullable: false));
        }
    }
}
