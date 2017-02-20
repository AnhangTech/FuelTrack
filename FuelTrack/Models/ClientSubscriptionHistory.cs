using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelTrack.Models
{
    public class ClientSubscriptionHistory
    {
        [DisplayName("客户订单回溯编号")]
        public long ClientSubscriptionHistoryId { get; set; }

        [DisplayName("客户订单号")]
        public long ClientSubscriptionId { get; set; }

        [DisplayName("售价（元/吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public float UnitPrice { get; set; }

        [DisplayName("数量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float Quantity { get; set; }

        [DisplayName("船名")]
        public string VesselName { get; set; }

        [DisplayName("状态")]
        public ClientSubscriptionState State { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [DisplayName("订单")]
        [ForeignKey("ClientSubscriptionId")]
        public virtual ClientSubscription Subscription { get; set; }
    }
}