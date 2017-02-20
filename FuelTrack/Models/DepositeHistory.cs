using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelTrack.Models
{

    [DisplayName("余款历史")]
    public class DepositeHistory
    {
        [DisplayName("余款历史编号")]
        public long DepositeHistoryId { get; set; }

        [DisplayName("油站编号")]
        public long StationAccountId { get; set; }

        [DisplayName("金额（元）")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}")]
        public double Amount { get; set; }

        [DisplayName("变更类型")]
        public DepositeChangeType ChangeType { get; set; }

        [DisplayName("描述")]
        public string Description { get; set; }

        [DisplayName("时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日 HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [DisplayName("油站")]
        [ForeignKey("StationAccountId")]
        public virtual StationAccount Station { get; set; }
    }

    public enum DepositeChangeType
    {
        Recharge,
        Refund,
        Pay,
        Cashout
    }
}