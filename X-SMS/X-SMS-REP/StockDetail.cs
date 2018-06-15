using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class StockDetail
    {
        public int StockId { get; set; }
        public string StockName { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
