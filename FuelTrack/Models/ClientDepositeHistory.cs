using System;

namespace FuelTrack.Models
{
    public class ClientDepositeHistory
    {
        public long ClientDepositeHistoryId { get; set; }

        public long ClientDepositeId { get; set; }

        public double AmountChange { get; set; }

        public string Note { get; set; }

        public DateTime Timestamp { get; set; }     
    }
}