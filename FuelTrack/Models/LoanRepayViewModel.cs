using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    public class LoanRepayViewModel
    {
        [DisplayName("贷款编号")]
        public long LoanId { get; set; }

        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }

        [DisplayName("客户名称")]
        public string ClientAccountName { get; set; }

        [DisplayName("贷款金额（元）")]
        public double StartAmount { get; set; }

        [DisplayName("剩余金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double CurrentAmount { get; set; }

        [DisplayName("当前剩余利息（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double CurrentInterest { get; set; }

        [DisplayName("起始日期")]
        public DateTime StartDate { get; set; }

        [DisplayName("还款金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        public double RepayAmount { get; set; }

        [DisplayName("还款日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime RepayDate { get; set; }

        [DisplayName("不计息天数")]
        [Range(0, int.MaxValue, ErrorMessage = "必须大于等于0.")]
        public int FreeDays { get; set; }

        [DisplayName("年利率（%）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        public double InterestRate { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }
    }
}