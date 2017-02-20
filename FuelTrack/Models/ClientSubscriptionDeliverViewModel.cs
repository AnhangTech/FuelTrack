using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{

    [DisplayName("客户订单")]
    public class ClientSubscriptionDeliverViewModel
    {

        [DisplayName("客户订单号")]
        public long ClientSubscriptionId { get; set; }


        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }


        [DisplayName("客户名称")]
        public string ClientAccountName { get; set; }


        [DisplayName("油站名称")]
        public string StationAccountName { get; set; }


        [DisplayName("船名")]
        public string VesselName { get; set; }


        [DisplayName("数量（吨）")]
        [Range(0, float.MaxValue, ErrorMessage = "必须大于等于0.")]
        public float Quantity { get; set; }


        [DisplayName("备注")]
        public string Notes { get; set; }
    }
}