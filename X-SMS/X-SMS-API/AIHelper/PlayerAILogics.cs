using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_REP;
using X_SMS_DAL.Database;
using X_SMS_DAL.Services;

namespace X_SMS_API.AIHelper
{
    class PlayerAILogics : IDisposable
    {
        StockDetail shouldBuy;
        StockDetail shouldSell;
        int shouldntBuy;
        int shouldntSell;
        PlayerService playerService = null;
        XSmsEntities aiEntities = null;
        GameDTO curGame = null;
        List<AIBuySellDetails> list = null;

        public PlayerAILogics()
        {
            aiEntities = new XSmsEntities();
            playerService = new PlayerService();
            list = new List<AIBuySellDetails>();
        }

        public void setGame(GameDTO game)
        {
            curGame = game;
        }

        public void BuyStocksForFirstTime(int playerID)
        {
            list.Clear();
            List<Stock> stocks = playerService.getAllStocks();
            List<Stock> firstThree = new List<Stock>(stocks.OrderBy(c => c.StartingPrice).Take(3));
            decimal totPrice = 0;
            foreach (Stock item in firstThree)
            {
                totPrice += item.StartingPrice; 
            }
            foreach (Stock item in firstThree)
            {
                StockDetail stockDetail = new StockDetail();
                stockDetail.StockId = item.StockId;
                stockDetail.StockName = item.StockName;
                stockDetail.StartingPrice = item.StartingPrice;
                stockDetail.CurrentPrice = 0;

                try
                {
                    int avgPrice = (int)((item.StartingPrice / totPrice) * 100);
                    int quanCategory = CheckQuanAmountForFirstTime(avgPrice);
                    int buyingQuanRange = CalcQuantityForFirstTime(quanCategory); //buying for first time
                    int buyingQuan = (int)(buyingQuanRange / item.StartingPrice);

                    AIBuySellDetails itemBuySell = new AIBuySellDetails();
                    if (buyingQuan > 0)
                    {
                        itemBuySell.GameId = curGame.GameId;
                        itemBuySell.PlayerId = playerID;
                        itemBuySell.SectorId = item.SectorId;
                        itemBuySell.Quantity = buyingQuan;
                        itemBuySell.Stock = stockDetail;
                        itemBuySell.Buy = true;
                        list.Add(itemBuySell);
                    }
                        //playerService.buyStocks(playerID, buyingQuan, stockDetail, item.StartingPrice);
                }
                catch (Exception e)
                {
                }
            }                
        }

        public void SellAllStocks(int playerId, List<StockDetail> ownStocks)
        {
            list.Clear();
            
            foreach (StockDetail item in ownStocks)
            {
                try
                {
                    int quan = playerService.checkStockQuantity(playerId, item.StockId);

                    AIBuySellDetails itemBuySell = new AIBuySellDetails();
                    if (quan > 0)
                    {
                        itemBuySell.GameId = curGame.GameId;
                        itemBuySell.PlayerId = playerId;
                        itemBuySell.SectorId = item.SectorId;
                        itemBuySell.Quantity = quan;
                        itemBuySell.Stock = item;
                        itemBuySell.Buy = false;
                        list.Add(itemBuySell);
                    }
                    //playerService.buyStocks(playerID, buyingQuan, stockDetail, item.StartingPrice);
                }
                catch (Exception e)
                {
                }
            }
        }

        public void CheckHistoryForBuy(List<TurnDetail> prevTurns, List<StockDetail> currentStocks)
        {
            //shouldBuy = new StockDetail();
            //List<TurnDetail> prevTurns = prevTurnsArray.Cast<TurnDetail>().ToList();

            foreach (TurnDetail turn in prevTurns)
            {
                foreach (SectorDetail sector in turn.Sectors)
                {
                    foreach (StockDetail stock in sector.Stocks)
                    {
                        int i = 0;
                        foreach (StockDetail owned in currentStocks)
                        {
                            if (stock.StockId != owned.StockId)
                            {
                                i++; continue;
                            }
                            else
                            {
                                if (shouldBuy == null)
                                    shouldBuy = stock;
                                else if (shouldBuy.CurrentPrice > stock.CurrentPrice)
                                    shouldBuy = stock;
                            }
                        }
                        if (i == currentStocks.Capacity && stock.CurrentPrice > shouldBuy.CurrentPrice)
                        {//this stock is never bought but its cur price is greater than shouldBuy's price
                            shouldBuy = stock;
                        }
                    } 
                }
            }
        }

        public void CheckHistoryForSell(List<TurnDetail> prevTurns, List<StockDetail> currentStocks)
        {
            //shouldSell = new StockDetail();
            //List<TurnDetail> prevTurns = prevTurnsArray.Cast<TurnDetail>().ToList();

            foreach (TurnDetail turn in prevTurns)
            {
                foreach (SectorDetail sector in turn.Sectors)
                {
                    foreach (StockDetail stock in sector.Stocks)
                    {
                        foreach (StockDetail owned in currentStocks)
                        {
                            if (stock.StockId != owned.StockId)
                                continue;
                            else
                            {
                                if (shouldSell == null)
                                    shouldSell = stock;
                                else if (shouldSell.CurrentPrice < stock.CurrentPrice)
                                    shouldSell = stock;
                            }
                        }
                    } 
                }
            }
        }

        public void SetBuySellForAI(List<StockDetail> ownStocks, int playerID)
        {
            list.Clear();
            CheckHistoryWithCurrent(ownStocks);

            if(ownStocks != null)
            {
                foreach (StockDetail item in ownStocks)
                {
                    AIBuySellDetails itemBuySell = new AIBuySellDetails();
                    decimal averagePrice = 0;

                    try
                    {
                        averagePrice = playerService.amountSpentForStocks(playerID, item.StockId);
                    }
                    catch (Exception)
                    {
                        averagePrice = 0;
                    }
                    if (averagePrice > 0)
                    {
                        if (((item.CurrentPrice - averagePrice) >= (decimal)0.2) && item.StockId != shouldntSell) //sell
                        {
                            try
                            {
                                int decidingPrice = (int)((averagePrice * 100) / item.CurrentPrice);
                                int quanCategory = CheckQuanAmount(decidingPrice);
                                int sellingQuan = CalcQuantity(quanCategory, playerService.checkStockQuantity(playerID, item.StockId));
                                if (quanCategory > 0 && sellingQuan > 0 && curGame != null)
                                {
                                    itemBuySell.GameId = curGame.GameId;
                                    itemBuySell.PlayerId = playerID;
                                    itemBuySell.SectorId = item.SectorId;
                                    itemBuySell.Quantity = sellingQuan;
                                    itemBuySell.Stock = item;
                                    itemBuySell.Buy = false;
                                    list.Add(itemBuySell);
                                }
                                    //hub.SellStocks(curGame.GameId, playerID, item.SectorId, item, sellingQuan);
                                    //playerService.sellStocks(playerID, sellingQuan, item, item.CurrentPrice);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else if (((item.CurrentPrice - averagePrice) < (decimal)0.2) && item.StockId != shouldntBuy)//buy
                        {
                            try
                            {
                                int decidingPrice = (int)((item.CurrentPrice * 100)/ averagePrice);
                                int quanCategory = CheckQuanAmount(decidingPrice);
                                int buyingQuan = CalcQuantity(quanCategory, playerService.checkStockQuantity(playerID, item.StockId));
                                if (quanCategory > 0 && buyingQuan > 0)
                                {
                                    itemBuySell.GameId = curGame.GameId;
                                    itemBuySell.PlayerId = playerID;
                                    itemBuySell.SectorId = item.SectorId;
                                    itemBuySell.Quantity = buyingQuan;
                                    itemBuySell.Stock = item;
                                    itemBuySell.Buy = true;
                                    list.Add(itemBuySell);
                                }
                                    //hub.BuyStocks(curGame.GameId, playerID, item.SectorId, item, buyingQuan);
                                    //playerService.buyStocks(playerID, buyingQuan, item, item.CurrentPrice);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                }
            }
        }

        private void CheckHistoryWithCurrent(List<StockDetail> ownStocks)
        {
            if (shouldSell != null && shouldBuy != null && ownStocks != null)
            {
                foreach (StockDetail item in ownStocks)
                {
                    if (item.StockId != shouldBuy.StockId)
                        continue;
                    else
                    {
                        if (item.CurrentPrice > shouldBuy.CurrentPrice)
                            shouldntBuy = item.StockId;
                    }
                }

                foreach (StockDetail item in ownStocks)
                {
                    if (item.StockId != shouldSell.StockId)
                        continue;
                    else
                    {
                        if (item.CurrentPrice < shouldSell.CurrentPrice)
                            shouldntSell = item.StockId;
                    }
                }
            }
        }

        private int CheckQuanAmount(int decidingPrice)
        {
            switch (decidingPrice)
            {
                case int n when (n >= 104 && n <= 107): //1 - max buying side should buy
                    return 1;
                case 103:
                    return 2;
                case 102:
                    return 3;
                case 101:
                    return 4;
                case 100:       //lowest should buy
                    return 5;
                case int n when (n >= 90 && n <= 95): // selling should sell max
                    return 10;  
                case 96:
                    return 9;
                case 97:
                    return 8;
                case 98:
                    return 7;
                case 99:
                    return 6;   //lowest should sell
                default:
                    return 0;
            }
        }

        private int CalcQuantity(int category, int currentStockQuan)
        {
            switch (category)
            {
                case 1:
                    return ((currentStockQuan * 90) / 100);               //buy start
                case 2:
                    return ((currentStockQuan * 80) / 100);
                case 3:
                    return ((currentStockQuan * 70) / 100);
                case 4:
                    return ((currentStockQuan * 60) / 100);
                case 5:
                    return ((currentStockQuan * 50) / 100); //buy calc end
                case 6:
                    return ((currentStockQuan * 50) / 100);   //sell start
                case 7:
                    return ((currentStockQuan * 60) / 100);
                case 8:
                    return ((currentStockQuan * 70) / 100);
                case 9:
                    return ((currentStockQuan * 80) / 100);
                case 10:
                    return ((currentStockQuan * 90) / 100);    //sell max end
                default:
                    return 0;
            } 
        }

        private int CheckQuanAmountForFirstTime(int decidingPrice)
        {
            switch (decidingPrice)
            {
                case int n when (n >= 1 && n <= 10):
                    return 1;
                case int n when (n > 10 && n <= 20):
                    return 2;
                case int n when (n > 20 && n <= 30):
                    return 3;
                case int n when (n > 30 && n <= 40):
                    return 4;
                case int n when (n > 40 && n <= 50):
                    return 5;
                case int n when (n > 50 && n <= 60):
                    return 6;
                case int n when (n > 60 && n <= 70):
                    return 7;
                case int n when (n > 70 && n <= 80):
                    return 8;
                case int n when (n > 80 && n <= 90):
                    return 9;
                case int n when (n > 90 && n <= 100):
                    return 10;
                default:
                    return 0;
            }
        }

        private int CalcQuantityForFirstTime(int category)
        {
            switch (category)
            {
                case 1:
                    return (500 * 50) / 100;
                case 2:
                    return (500 * 40) / 100;
                case 3:
                    return (500 * 30) / 100;
                case 4:
                    return (500 * 25) / 100;
                case 5:
                    return (500 * 20) / 100;
                case 6:
                    return (500 * 15) / 100;
                case 7:
                    return (500 * 10) / 100;
                case 8:
                    return (500 * 5) / 100;
                case 9:
                    return (500 * 3) / 100;
                case 10:
                    return (500 * 1) / 100;

                default:
                    return 0;
            }
        }

        public List<AIBuySellDetails> returnBuySellList()
        {
            if (list != null)
                return list;
            else
                return null;
        }

        public void Dispose()
        {
            if (playerService != null)
            {
                playerService.Dispose();
                playerService = null;
            }
            if (aiEntities != null)
            {
                aiEntities.Dispose();
                aiEntities = null;
            }
            if (shouldSell != null)
                shouldSell = null;
            if (shouldBuy != null)
                shouldBuy = null;
            if (curGame != null)
                curGame = null;
            if (list != null)
                list = null;
        }
    }
}