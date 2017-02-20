using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelTrack.Models
{
    [DisplayName("客户订单")]
    public class ClientSubscription
    {
        [DisplayName("客户订单号")]
        public long ClientSubscriptionId { get; set; }

        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("预定油量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float StartQuantity { get; set; }

        [DisplayName("未加油量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float CurrentQuantity { get; set; }

        [DisplayName("售价（元/吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public float UnitPrice { get; set; }

        [DisplayName("船名")]
        public string VesselName { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("状态")]
        public ClientSubscriptionState State { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [ForeignKey("ClientAccountId")]
        [DisplayName("客户")]
        public virtual ClientAccount Client { get; set; }


        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }

        [DisplayName("订单历史")]
        public virtual List<ClientSubscriptionHistory> History { get; set; }
    }

    public enum ClientSubscriptionState
    {
        [Display(Name = "新建")]
        Created = 0,
        [Display(Name = "已支付")]
        Paid = 1,
        [Display(Name = "部分交付")]
        PartialDelivered = 5,
        [Display(Name = "全部交付")]
        Delivered = 2,
        [Display(Name = "已退款")]
        Refunded = 3,
        [Display(Name = "已完成")]
        Closed = 4
    }
}