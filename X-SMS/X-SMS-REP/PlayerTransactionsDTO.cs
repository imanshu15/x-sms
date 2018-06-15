using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class PlayerTransactionsDTO
    {        
        public int StockId { get; set; }    
        public int PlayerId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
