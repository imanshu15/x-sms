using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class GameDetail
    {
        public int[] RandomTrend { get; set; }
        public int[] MarketTrend { get; set; }
        public Dictionary<int, int>[] SectorTrend { get; set; }
        public EventDetail[] EventDetail { get; set; }
        public List<TurnDetail> TurnDetail { get; set; }

    }
}
