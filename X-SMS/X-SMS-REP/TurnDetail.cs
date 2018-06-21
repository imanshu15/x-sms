using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class TurnDetail
    {
        public int Turn { get; set; }
        public List<SectorDetail> Sectors { get; set; }

        public int[] RandomTrend { get; set; }
        public int[] MarketTrend { get; set; }
        public Dictionary<int, int>[] SectorTrend { get; set; }
        public EventDetail[] EventDetail { get; set; }
    }
}
