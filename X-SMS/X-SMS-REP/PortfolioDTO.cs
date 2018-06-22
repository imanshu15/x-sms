using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class PortfolioDTO
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public decimal Balance { get; set; }
        public List<PlayerPortfolioDTO> Transactions { get; set; }
    }
}
