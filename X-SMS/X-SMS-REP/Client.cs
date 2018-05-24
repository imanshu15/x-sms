using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class Client
    {
        public string Name { get; set; }
        public bool IsPlaying { get; set; }
        public string ConnectionId { get; set; }

        public GameDTO Game { get; set; }
    }
}
