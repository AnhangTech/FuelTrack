using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Models
{
    [DisplayName("客户统计")]
    public class ClientStatisticsViewModel
    {
        [DisplayName("起始日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime StartDate { get; set; }

        [DisplayName("终止日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime EndDate { get; set; }

        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }

        [DisplayName("客户名称")]
        public string ClientAccountName { get; set; }

        [DisplayName("订购总量")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalQuantity { get; set; }

        [DisplayName("售油款总计")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalIncreasedSubscriptionAmount { get; set; }

        [DisplayName("新增支付总金额")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalPaidAmount { get; set; }

        [DisplayName("截止时间截止时间末完成的订单暂时储存在油站的油量")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalIncompleteQuantity { get; set; }

        [DisplayName("新增资金")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalIncreasedBalance { get; set; }

        [DisplayName("新增贷款")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double TotalIncreasedLoan { get; set; }

        [DisplayName("利润")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Profit { get; set; }

        [DisplayName("客户订单")]
        public List<ClientSubscription> ClientSubscriptions { get; set; }

        [DisplayName("客户列表")]
        public SelectList ClientAccountList { get; set; }
    }
}