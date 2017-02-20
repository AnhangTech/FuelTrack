using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelTrack.Models
{
    [DisplayName("金额变动历史")]
    public class ClientBalanceHistory
    {
        [DisplayName("金额变动历史编号")]
        public long ClientBalanceHistoryId { get; set; }
        [DisplayName("客户编号")]
        public long ClientAccountId { get; set; }
        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }
        [DisplayName("金额变化类型")]
        public BalanceChangeType BalanceType { get; set; }
        [DisplayName("描述")]
        public string Description { get; set; }
        [DisplayName("时间")]
        [DisplayFormat(DataFormatString ="{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }
        [DisplayName("客户")]
        [ForeignKey("ClientAccountId")]
        public virtual ClientAccount Client { get; set; }
    }

    public enum BalanceChangeType
    {
        Recharge,
        Refund,
        Loan,
        Pay,
        LoanRepay,
        Cashout
    }
}