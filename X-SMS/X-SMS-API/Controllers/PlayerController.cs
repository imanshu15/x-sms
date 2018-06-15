using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;
using X_SMS_REP;
using X_SMS_REP.RequestModel;

namespace X_SMS_API.Controllers
{
    public class PlayerController : ApiController
    {
        public ResultToken CreateBankAccount([FromBody] PlayerDTO player)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.createBankAccount(player.PlayerName);
                return result;
            }
        }

        public ResultToken BuyStocks([FromBody] PlayerTransactionsDTO playerTrans)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.buyStocks(playerTrans.PlayerId, playerTrans.Quantity, playerTrans.StockId, playerTrans.Price);
                return result;
            }
        }

        public ResultToken SellStocks([FromBody] PlayerTransactionsDTO playerTrans)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.sellStocks(playerTrans.PlayerId, playerTrans.Quantity, playerTrans.StockId, playerTrans.Price);
                return result;
            }
        }

        //public IHttpActionResult getPortfolio(int playerID)
        //{
        //    using (PlayerService playerService = new PlayerService())
        //    {
        //        var result = playerService.viewPortfolio(playerID);
        //        return result;
        //    }
        //}
    }
}
