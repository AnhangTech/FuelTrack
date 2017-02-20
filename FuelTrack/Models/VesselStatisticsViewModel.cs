using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Models
{
    public class VesselStatisticsViewModel
    {
        [DisplayName("起始日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime StartDate { get; set; }

        [DisplayName("终止日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime EndDate { get; set; }

        [DisplayName("船名")]
        public string VesselName { get; set; }

        [DisplayName("加油吨数")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalQuantity { get; set; }

        [DisplayName("加油艘次")]
        public int TotalCount { get; set; }

        [DisplayName("客户订单")]
        public List<ClientSubscription> ClientSubscriptions { get; set; }

        [DisplayName("船只列表")]
        public SelectList VesselList { get; set; }
    }
}