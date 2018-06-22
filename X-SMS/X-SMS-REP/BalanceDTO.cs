using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class BalanceDTO
    {
        public decimal OpeningBalance { get; set; }
        public decimal AllocatedPrice { get; set; }
        public decimal ProfitPrice { get; set; }
        public decimal Balance { get; set; }
    }
}
