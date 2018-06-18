using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class PlayerStock
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int StockId { get; set; }
        public string StockName { get; set; }
        public int Quantity { get; set; }
        public decimal BoughtPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool IsIncreased { get; set; }
    }
}
