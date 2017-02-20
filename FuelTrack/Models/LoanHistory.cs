using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelTrack.Models
{
    [DisplayName("贷款历史")]
    public class LoanHistory
    {
        [DisplayName("贷款历史编号")]
        public long LoanHistoryId { get; set; }

        [DisplayName("贷款编号")]
        public long LoanId { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [DisplayName("发生日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime DealDate { get; set; }

        [DisplayName("额金（元）")]
        public double Amount { get; set; }

        [DisplayName("利息（元）")]
        [Range(0, double.MaxValue, ErrorMessage = "必须大于等于0.")]
        public double Interest { get; set; }

        [DisplayName("类型")]
        public LoanChangeType ChangeType { get; set; }

        [DisplayName("备注")]
        public string Notes { get; set; }

        [DisplayName("贷款")]
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }
    }

    public enum LoanChangeType
    {
        Loan,
        Repay
    }
}