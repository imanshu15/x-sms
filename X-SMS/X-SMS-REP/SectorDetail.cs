using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class SectorDetail
    {
        public SectorDTO Sector { get; set; }
        public int Score { get; set; }
        public List<StockDetail> Stocks { get; set; }
    }
}
