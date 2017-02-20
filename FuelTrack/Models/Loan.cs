using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    [DisplayName("贷款")]
    public class Loan
    {
        [DisplayName("贷款编号")]
        public long LoanId { get; set; }

        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }

        [DisplayName("贷款金额（元）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double StartAmount { get; set; }

        [DisplayName("剩余金额（元）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double CurrentAmount { get; set; }

        [DisplayName("起始日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime StartDate { get; set; }

        [DisplayName("结束日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime? EndDate { get; set; }

        [DisplayName("不计息天数")]
        [Range(0, int.MaxValue, ErrorMessage = "必须大于等于0.")]
        public int FreeDays { get; set; }

        [DisplayName("年利率（%）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        public double InterestRate { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [DisplayName("客户")]
        [ForeignKey("ClientAccountId")]
        public virtual ClientAccount Client { get; set; }

        [DisplayName("贷款历史")]
        public virtual List<LoanHistory> Histories { get; set; }
    }
}