using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class EventDetail
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int Effect { get; set; }
        public bool IsSector { get; set; }
        public int SectorId { get; set; }
        public bool IsStock { get; set; }
        public int Duration { get; set; }
    }
}
