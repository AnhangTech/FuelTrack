using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelTrack.Models
{
    [DisplayName("订单")]
    public class Subscription
    {
        [DisplayName("订单编号")]
        public long SubscriptionId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("起始数量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float StartQuantity { get; set; }

        [DisplayName("剩余数量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float CurrentQuantity { get; set; }

        [DisplayName("原价（元/吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public float UnitPrice { get; set; }

        [DisplayName("状态")]
        public SubscriptionState State { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [DisplayName("订单历史")]
        public virtual List<SubscriptionHistory> History { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }
    }

    public enum SubscriptionState
    {
        Created = 0,
        Paid = 1,
        PartialDelivered = 3,
        Delivered = 4,
        Refunded = 5,
        Closed = 6
    }
}