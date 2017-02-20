using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    [DisplayName("艘次")]
    public class Delivery
    {
        [DisplayName("艘次编号")]
        public long ClientAccountId { get; set; }

        [DisplayName("客户名称")]
        [Required(ErrorMessage = "必须输入客户名称")]
        public string ClientAccountName { get; set; }

        [DisplayName("余额（元）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Balance { get; set; }

        [DisplayName("全部贷款（元）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Loan { get; set; }

        [DisplayName("贷款上限（元）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        public double LoanLimit { get; set; }

        [DisplayName("订单")]
        public virtual List<ClientSubscription> Subscriptions { get; set; }
        [DisplayName("金额变动历史")]
        public virtual List<ClientBalanceHistory> BalanceHistory { get; set; }
        [DisplayName("贷款")]
        public virtual List<Loan> Loans { get; set; }
    }
}