namespace FuelTrack.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class FuelTrackContext : DbContext
    {
        // Your context has been configured to use a 'FuelTrack' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'FuelTrack.Models.FuelTrack' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'FuelTrack' 
        // connection string in the application configuration file.
        public FuelTrackContext()
            : base("name=FuelTrackContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<ClientAccount> ClientAccounts { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.ClientSubscription> ClientSubscriptions { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.StationAccount> StationAccounts { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.ClientSubscriptionHistory> ClientSubscriptionHistories { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.ClientBalanceHistory> ClientBalanceHistories { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.Loan> Loans { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.LoanHistory> LoanHistories { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.Subscription> Subscriptions { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.SubscriptionHistory> SubscriptionHistories { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.DepositeHistory> DepositeHistories { get; set; }

        public System.Data.Entity.DbSet<FuelTrack.Models.PaymentRequest> PaymentRequests { get; set; }
    }
}