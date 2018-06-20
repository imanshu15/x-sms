﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X_SMS_REP;

namespace X_SMS.Services
{
    public class GameLogicManager
    {
        public GameDetail GetGameData(int gameId)
        {
            GameDetail returnObj = new GameDetail();
            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakeGetRequest("api/trend?gameId=" + gameId.ToString());
                    ResultToken result = apiClient.ConvertObjectToToken(temp);
                    if (result.Success && result.Data != null)
                    {
                        returnObj = JsonConvert.DeserializeObject<GameDetail>(result.Data.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return returnObj;
        }

        public ResultToken BuyStocks(int playerId,StockDetail stock,int quantity,decimal price)
        {
            ResultToken result = null;

            PlayerTransactionsDTO obj = new PlayerTransactionsDTO();
            obj.PlayerId = playerId;
            obj.Stock.StockId = stock.StockId;
            obj.Quantity = quantity;
            obj.Price = price;

            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakePostRequest("api/Broker/BuyStocks", obj);
                    result = apiClient.ConvertObjectToToken(temp);
                    if (result != null && result.Success)
                        result.Data = obj;
                }
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;
        }

        public ResultToken SellStocks(int playerId, StockDetail stock, int quantity, decimal price)
        {
            ResultToken result = null;

            PlayerTransactionsDTO obj = new PlayerTransactionsDTO();
            obj.PlayerId = playerId;
            obj.Stock.StockId = stock.StockId;
            obj.Quantity = quantity;
            obj.Price = price;

            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakePostRequest("api/Broker/SellStocks", obj);
                    result = apiClient.ConvertObjectToToken(temp);
                    if (result != null && result.Success)
                        result.Data = obj;
                }
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;
        }

        public decimal GetStockValue(int gameId, int sectorId, int stockId)
        {
            decimal returnValue = 0;

            var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
            if (game != null)
            {
                var turn = game.GameDetail.TurnDetail.FirstOrDefault(x => x.Turn == game.CurrentRound);
                if (turn != null)
                {
                    var sector = turn.Sectors.FirstOrDefault(y => y.Sector.SectorId == sectorId);
                    if (sector != null)
                    {
                        returnValue = sector.Stocks.FirstOrDefault(a => a.StockId == stockId).CurrentPrice;
                    }
                }
            }
            return returnValue;
        }
    }
}