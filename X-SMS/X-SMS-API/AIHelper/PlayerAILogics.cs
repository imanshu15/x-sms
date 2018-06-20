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
        PlayerService playerService = null;
        XSmsEntities aiEntities = null;
        public PlayerAILogics()
        {
            aiEntities = new XSmsEntities();
            playerService = new PlayerService();
        }

        public void BuyStocksForFirstTime(int playerID)
        {
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
                    int quanCategory = CheckQuanAmount(avgPrice);
                    int buyingQuanRange = CalcQuantityForFirstTime(quanCategory); //buying for first time
                    int buyingQuan = (int)(buyingQuanRange / item.StartingPrice);

                    if(buyingQuan > 0)
                        playerService.buyStocks(playerID, buyingQuan, stockDetail, item.StartingPrice);
                }
                catch (Exception e)
                {
                }
            }
                
        }

        public void CheckHistoryForBuy(List<TurnDetail>[] prevTurnsArray, List<StockDetail> currentStocks)
        {
            shouldBuy = new StockDetail();
            List<TurnDetail> prevTurns = prevTurnsArray.Cast<TurnDetail>().ToList();

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

        public void CheckHistoryForSell(List<TurnDetail>[] prevTurnsArray, List<StockDetail> currentStocks)
        {
            shouldSell = new StockDetail();
            List<TurnDetail> prevTurns = prevTurnsArray.Cast<TurnDetail>().ToList();

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
            List<StockDetail> currentStocks = CheckHistoryWithCurrent(ownStocks, shouldBuy, shouldSell);
            if(currentStocks != null)
            {
                foreach (StockDetail item in currentStocks)
                {
                    decimal averagePrice = 0;
                    decimal shouldSellIF = 0;
                    decimal shoulBuyIF = 0;
                    try
                    {
                        averagePrice = playerService.amountSpentForStocks(playerID, item.StockId) / playerService.checkStockQuantity(playerID, item.StockId);
                        shoulBuyIF = (averagePrice * 35) / 100;
                        shouldSellIF = (averagePrice * 635) / 100;
                    }
                    catch (Exception)
                    {
                        averagePrice = 0;
                    }
                    if (averagePrice > 0)
                    {
                        if (item.CurrentPrice >= shouldSellIF)
                        {
                            try
                            {
                                int decidingPrice = (int)(averagePrice / item.CurrentPrice) * 100;
                                int quanCategory = CheckQuanAmount(decidingPrice);
                                int sellingQuan = CalcQuantity(quanCategory, playerService.checkStockQuantity(playerID, item.StockId));
                                if (quanCategory > 0 && sellingQuan > 0)
                                    playerService.sellStocks(playerID, sellingQuan, item, item.CurrentPrice);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else if (item.CurrentPrice < shoulBuyIF)
                        {
                            try
                            {
                                int decidingPrice = (int)(item.CurrentPrice / averagePrice) * 100;
                                int quanCategory = CheckQuanAmount(decidingPrice);
                                int buyingQuan = CalcQuantity(quanCategory, playerService.checkStockQuantity(playerID, item.StockId));
                                if (quanCategory > 0 && buyingQuan > 0)
                                    playerService.buyStocks(playerID, buyingQuan, item, item.CurrentPrice);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                    else if (averagePrice == 0) //AI never bought this stock
                    {

                    }
                }
            }
        }

        public List<StockDetail> CheckHistoryWithCurrent(List<StockDetail> ownStocks, StockDetail shouldBuy, StockDetail shouldSell)
        {
            if (shouldSell != null && shouldSell != null)
            {
                foreach (StockDetail item in ownStocks)
                {
                    if (ownStocks != null)
                    {
                        if (item.StockId != shouldBuy.StockId)
                            continue;
                        else
                        {
                            if (item.CurrentPrice > shouldBuy.CurrentPrice)
                                ownStocks.Remove(item);
                        } 
                    }
                }

                foreach (StockDetail item in ownStocks)
                {
                    if (ownStocks != null)
                    {
                        if (item.StockId != shouldSell.StockId)
                            continue;
                        else
                        {
                            if (item.CurrentPrice < shouldSell.CurrentPrice)
                                ownStocks.Remove(item);
                        } 
                    }
                }
                return ownStocks;
            }
            else
                return ownStocks;
        }

        public int CheckQuanAmount(int decidingPrice)
        {
            switch (decidingPrice)
            {
                case int n when (n >= 1 && n <= 25):
                    return 1;
                case int n when (n > 25 && n <= 50):
                    return 2;
                case int n when (n > 50 && n <= 75):
                    return 3;
                case int n when (n > 75 && n <= 99):
                    return 4;
                default:
                    return 0;
            }
        }

        public int CalcQuantity(int category, int currentStockQuan)
        {
            switch (category)
            {
                case 1:
                    return currentStockQuan;
                case 2:
                    return (currentStockQuan * 75) / 100;
                case 3:
                    return (currentStockQuan * 50) / 100;
                case 4:
                    return (currentStockQuan * 25) / 100;
                default:
                    return 0;
            } 
        }

        public int CalcQuantityForFirstTime(int category)
        {
            switch (category)
            {
                case 1:
                    return (500 * 75) / 100;
                case 2:
                    return (500 * 50) / 100;
                case 3:
                    return (500 * 25) / 100;
                case 4:
                    return 0;
                default:
                    return 0;
            }
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
        }
    }
}