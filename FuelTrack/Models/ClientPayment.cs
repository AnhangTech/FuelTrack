using System;

namespace FuelTrack.Models
{
    public class ClientPayment
    {
        public long ClientPaymentId { get; set; }

        public long ClientSubscriptionHistoryId { get; set; }


        public long ClientSubscriptionId { get; set; }


        public long ClientAccountId { get; set; }

        // WithDeposite or WithCurrency
        public string PaymentType { get; set; }

        public long? ClientDepositeHistory { get; set; }

        public double Amount { get; set; }

        public DateTime Timestamp { get; set; }
    }
}