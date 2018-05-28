using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP.RequestModel
{
    public class JoinRequestModel
    {
        public string PlayerName { get; set; }
        public int GameId { get; set; }
        public string GameCode { get; set; }
        public string ConnectionId { get; set; }
    }
}
