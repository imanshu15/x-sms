using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class EventDTO
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int MinEffect { get; set; }
        public int MaxEffect { get; set; }
        public bool IsSector { get; set; }
        public bool IsStock { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public double Probability { get; set; }
        public int Chance { get; set; }
    }
}
