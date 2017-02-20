using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace FuelTrack.Models
{
    public class ClientSubscriptionViewModel
    {
        [DisplayName("客户订单号")]
        public long ClientSubscriptionId { get; set; }

        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }

        [DisplayName("客户名称")]
        public string ClientAccountName { get; set; }

        [DisplayName("船名")]
        public string VesselName { get; set; }

        [DisplayName("油站编号")]
        public string StationAccountId { get; set; }

        [DisplayName("数量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float Quantity { get; set; }

        [DisplayName("售价（元/吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public float UnitPrice { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("油站列表")]
        public SelectList StationList { get; set; }

        [DisplayName("时间")]
        public DateTime Timestamp { get; set; }
    }
}