using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    public class ClientSubscriptionHistoryViewModel
    {
        public long ClientAccountId { get; set; }

        public string ClientAccountName { get; set; }

        public DateTime Month { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalPaid { get; set; }

        public double TotalDelivered { get; set; }
    }

    public class MonthlyClientSubscriptionHistoryViewModel
    {
        public DateTime Month { get; set; }

        public double TotalPaid { get; set; }

        public double TotalDelivered { get; set; }

        public List<ClientSubscriptionHistoryViewModel> ClientSubscriptionSummaries { get; set; }
    }
}