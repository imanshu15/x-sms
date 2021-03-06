﻿using System;
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

        [Route("api/Bank/CreateAccount")]
        [HttpPost]
        public ResultToken CreateBankAccount([FromBody] PlayerDTO player)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.createBankAccount(player.PlayerId,player.PlayerName);
                return result;
            }
        }

        [Route("api/Broker/BuyStocks")]
        [HttpPost]
        public ResultToken BuyStocks([FromBody] PlayerTransactionsDTO playerTrans)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.buyStocks(playerTrans.PlayerId, playerTrans.Quantity, playerTrans.Stock, playerTrans.Price);
                return result;
            }
        }

        [Route("api/Broker/SellStocks")]
        [HttpPost]
        public ResultToken SellStocks([FromBody] PlayerTransactionsDTO playerTrans)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.sellStocks(playerTrans.PlayerId, playerTrans.Quantity, playerTrans.Stock, playerTrans.Price);
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

        [Route("api/PlayerHistory/GetPurchaseHistory")]
        [HttpGet]
        public ResultToken GetPurchaseHistory([FromBody] int palyerID)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.getPurchasesByPlayer(palyerID);
                return result;
            }
        }

        [Route("api/PlayerHistory/GetSalesHistory")]
        [HttpGet]
        public ResultToken GetSalesHistory([FromBody] int palyerID)
        {
            using (PlayerService playerService = new PlayerService())
            {
                var result = playerService.getSalesByPlayer(palyerID);
                return result;
            }
        }
    }
}
