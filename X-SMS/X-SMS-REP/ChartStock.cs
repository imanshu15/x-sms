using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class ChartStock
    {
        public int StockId { get; set; }
        public string StockName { get; set; }
        public List<Decimal> PriceList { get; set; }
    }
}
