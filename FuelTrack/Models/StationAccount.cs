using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    [DisplayName("油站")]
    public class StationAccount
    {
        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("油站名称")]
        public string StationName { get; set; }

        [DisplayName("余款（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Deposite { get; set; }

        [DisplayName("订单")]
        public virtual List<Subscription> Subscriptions { get; set; }

        [DisplayName("余款历史")]
        public virtual List<DepositeHistory> DepositeHistory { get; set; }
    }
}