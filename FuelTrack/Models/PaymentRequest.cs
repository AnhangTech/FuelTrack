using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    [DisplayName("请款")]
    public class PaymentRequest
    {
        [DisplayName("请款编号")]
        public long PaymentRequestId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }

        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }

        [DisplayName("请款状态")]
        public PaymentRequestState State { get; set; }

        [DisplayName("请款事由")]
        public string Reason { get; set; }

        [DisplayName("业务员编号")]
        public string EmployeeId { get; set; }

        [DisplayName("请求发起时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime StartTimestamp { get; set; }

        [DisplayName("财务总监编号")]
        public string FinanceManagerId { get; set; }

        [DisplayName("财务总监批示")]
        public string FinanceManagerComments { get; set; }

        [DisplayName("财务总监批示时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime? FinanceManagerCommentsTimestamp { get; set; }

        [DisplayName("业务总监编号")]
        public string BusinessManagerId { get; set; }

        [DisplayName("业务总监批示")]
        public string BusinessManagerComments { get; set; }

        [DisplayName("业务总监批示时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime? BusinessManagerCommentsTimestamp { get; set; }

        [DisplayName("备注")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public string Notes { get; set; }

        [DisplayName("油站银行账户名")]
        public string BankAccountName { get; set; }

        [DisplayName("油站银行账号")]
        public string BankAccountNumber { get; set; }

        [DisplayName("油站银行支行名称")]
        public string BankBranch { get; set; }
    }

    public class PaymentRequestApplicationViewModel
    {
        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }

        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }

        [DisplayName("请款事由")]
        public string Reason { get; set; }

        [DisplayName("油站银行账户名")]
        public string BankAccountName { get; set; }

        [DisplayName("油站银行账号")]
        public string BankAccountNumber { get; set; }

        [DisplayName("油站银行支行名称")]
        public string BankBranch { get; set; }
    }

    public class PaymentRequestFinanceManagerApproveViewModel
    {
        [DisplayName("请款编号")]
        public long PaymentRequestId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }

        [DisplayName("油站银行账户名")]
        public string BankAccountName { get; set; }

        [DisplayName("油站银行账号")]
        public string BankAccountNumber { get; set; }

        [DisplayName("油站银行支行名称")]
        public string BankBranch { get; set; }

        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }

        [DisplayName("请款状态")]
        public PaymentRequestState State { get; set; }

        [DisplayName("请款事由")]
        public string Reason { get; set; }

        [DisplayName("业务员")]
        public string Employee { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]

        public DateTime StartTimestamp { get; set; }

        [DisplayName("财务总监批示")]
        public string FinanceManagerComments { get; set; }
    }

    public class PaymentRequestBusinessManagerApproveViewModel
    {
        [DisplayName("请款编号")]
        public long PaymentRequestId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }

        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }

        [DisplayName("请款状态")]
        public PaymentRequestState State { get; set; }

        [DisplayName("油站银行账户名")]
        public string BankAccountName { get; set; }

        [DisplayName("油站银行账号")]
        public string BankAccountNumber { get; set; }

        [DisplayName("油站银行支行名称")]
        public string BankBranch { get; set; }

        [DisplayName("请款事由")]
        public string Reason { get; set; }

        [DisplayName("业务员")]
        public string Employee { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]

        public DateTime StartTimestamp { get; set; }

        [DisplayName("财务总监")]
        public string FinanceManager { get; set; }

        [DisplayName("财务总监批示")]
        public string FinanceManagerComments { get; set; }

        [DisplayName("财务总监批示时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime? FinanceManagerCommentsTimestamp { get; set; }
    }

    public class PaymentRequestCompleteViewModel
    {
        [DisplayName("请款编号")]
        public long PaymentRequestId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }

        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }

        [DisplayName("油站银行账户名")]
        public string BankAccountName { get; set; }

        [DisplayName("油站银行账号")]
        public string BankAccountNumber { get; set; }

        [DisplayName("油站银行支行名称")]
        public string BankBranch { get; set; }

        [DisplayName("请款状态")]
        public PaymentRequestState State { get; set; }

        [DisplayName("请款事由")]
        public string Reason { get; set; }

        [DisplayName("业务员")]
        public string Employee { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]

        public DateTime StartTimestamp { get; set; }

        [DisplayName("财务总监")]
        public string FinanceManager { get; set; }

        [DisplayName("财务总监批示")]
        public string FinanceManagerComments { get; set; }

        [DisplayName("财务总监批示时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime? FinanceManagerCommentsTimestamp { get; set; }

        [DisplayName("业务总监")]
        public string BusinessManager { get; set; }

        [DisplayName("业务总监批示")]
        public string BusinessManagerComments { get; set; }

        [DisplayName("业务总监批示时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime? BusinessManagerCommentsTimestamp { get; set; }
    }

    public enum PaymentRequestState
    {
        Start,
        FinanceManagerApproved,
        BusinessManagerApproved,
        Recharged,
        FinanceManagerRejected,
        BusinessManagerRejected,
        Withdrawed
    }
}