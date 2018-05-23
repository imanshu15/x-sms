using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class GameDTO
    {
        public int GameId { get; set; }
        public string GameCode { get; set; }
        public int PlayersCount { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public int CurrentTurn { get; set; }
        public string playerName { get; set; }

    }
}
