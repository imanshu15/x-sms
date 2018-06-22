using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class PlayerPortfolioDTO
    {
        public string StockName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> PlayerId { get; set; }
        public Nullable<int> StockId { get; set; }
        public Nullable<int> GameId { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public bool IsWithdraw { get; set; }
        public bool IsDeposit { get; set; }
        public decimal Amount { get; set; }

        public Nullable<int> SectorId { get; set; }
        public string SectorName { get; set; }
    }
}
