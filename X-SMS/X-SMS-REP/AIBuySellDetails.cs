using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class AIBuySellDetails
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int SectorId { get; set; }
        public StockDetail Stock { get; set; }
        public int Quantity { get; set; }
        public bool Buy { get; set; }
    }
}
