using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FuelTrack.Models
{
    public class HomeViewModel
    {
        [DisplayName("快到期贷款")]
        public List<Loan> DueSoonLoans { get; set; }

        [DisplayName("到期贷款")]
        public List<Loan> DueLoans { get; set; }
    }
}