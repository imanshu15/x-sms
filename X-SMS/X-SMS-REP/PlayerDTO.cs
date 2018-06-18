using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class PlayerDTO
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string SecondaryName { get; set; }
        public string ConnectionId { get; set; }
        public int GameId { get; set; }
        public bool IsActive { get; set; }

        public BankAccountDTO BankAccount { get; set; }
        public string GameCode { get; set; }
        public List<PlayerStock> PlayerStocks { get; set; }
    }
}
