using System;
using System.Collections.Generic;

namespace FuelTrack.Models
{
    public class ClientDeposite
    {
        public long ClientDepositeId { get; set; }

        public double Amount { get; set; }

        public long ClientAccountId { get; set; }

        public long StationAccountId { get; set; }

        public virtual List<ClientDepositeHistory> History { get; set; }
    }
}