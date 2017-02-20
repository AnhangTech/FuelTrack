using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    public class SubscriptionDeliverViewModel
    {
        [DisplayName("订单编号")]
        public long SubscriptionId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("客户订单编号")]
        public long ClientSubscriptionId { get; set; }

        [DisplayName("油站名称")]
        public string StationAccountName { get; set; }

        [DisplayName("数量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float Quantity { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("时间")]
        public DateTime Timestamp { get; set; }
    }
}