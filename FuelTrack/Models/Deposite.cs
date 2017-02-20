using System.Collections.Generic;

namespace FuelTrack.Models
{
    public class Deposite
    {
        public long DepositeId { get; set; }

        public double Amount { get; set; }

        public long StationAccountId { get; set; }

        public virtual List<DepositeHistory> History { get; set; }
    }
}