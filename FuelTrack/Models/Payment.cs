using System;

namespace FuelTrack.Models
{
    public class Payment
    {
        public long PaymentId { get; set; }

        public long SubscriptionHistoryId { get; set; }


        public long SubscriptionId { get; set; }


        public long StationId { get; set; }

        // WithDeposite or WithCurrency
        public string PaymentType { get; set; }

        public long? DepositeHistory { get; set; }

        public double Amount { get; set; }

        public DateTime Timestamp { get; set; }
    }
}