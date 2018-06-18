using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Database;
using X_SMS_DAL.Mapper;
using X_SMS_REP;

namespace X_SMS_DAL.Services
{
    public class PlayerService : IDisposable
    {
        XSmsEntities playerEntities = null;

        public PlayerService()
        {
            playerEntities = new XSmsEntities();
        }

        //public ResultToken registerPlayer(string playerFirstName, string playerSecondName)
        //{

        //}

        public ResultToken createBankAccount(int playerId,string playerName)
        {
            ResultToken result = new ResultToken();
            result.Success = true;

            try
            {
                //var accounts = playerEntities.BankAccounts.Where(c => c.AccountName == playerName && c.IsActive == true).FirstOrDefault();

                //if (accounts == null)
                //{
                    BankAccount newAccount = new BankAccount();
                    newAccount.AccountName = playerName;
                    newAccount.PlayerId = playerId;
                    newAccount.Balance = (decimal)1000;
                    newAccount.IsActive = true;
                    playerEntities.BankAccounts.Add(newAccount);
                    playerEntities.SaveChanges();

                    result.Data = newAccount;
                //}
                //else
                //{
                //    result.Success = false;
                //    result.Message = "Account name already exists";
                //}
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }

            return result;

        }

        //buyStocks()
        public ResultToken buyStocks(int playerID, int quantity, StockDetail stock, decimal price)
        {
            ResultToken result = new ResultToken();
            result.Success = true;
            bool gotMoney = false;

            decimal currentAccBalance = checkBankBalance(playerID);
            if (currentAccBalance >= quantity * price)
            {
                gotMoney = true;
            }
            else
            {
                result.Success = false;
                result.Message = "Account balance insufficient";
                return result;
            }
            //price is right?

            int accID = getAccountID(playerID);

            if (gotMoney && accID > 0)
            {
                try
                {
                    playerEntities.BuyStocks(playerID, accID, quantity, stock.StockId, price);
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                    return result;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Transaction incomplete";
                return result;
            }

            return result;
        }

        //sellStocks()
        public ResultToken sellStocks(int playerID, int quantity, StockDetail stock, decimal price)
        {
            ResultToken result = new ResultToken();
            result.Success = true;
            bool gotSupply = false;

            int quantityBalance = checkStockQuantity(playerID, stock.StockId);
            if (quantityBalance >= quantity)
            {
                gotSupply = true;
            }
            else
            {
                result.Success = false;
                result.Message = "Insufficient Stocks";
                return result;
            }
            // price is right?


            int accID = getAccountID(playerID); //acc id for update bank acc details

            if (gotSupply && accID > 0)
            {
                try
                {
                    playerEntities.SellStocks(playerID, accID, quantity, stock.StockId, price);
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                    return result;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Transaction incomplete";
                return result;
            }


            return result;
        }

        //portfolio
        public ResultToken viewPortfolio(int playerID)
        {
            ResultToken result = new ResultToken();
            result.Success = true;

            var playersPortfolio = playerEntities.ViewPlayerPortfolios.Where(c => c.PlayerId == playerID);
            //map as view obj
            return result;
        }

        public List<Stock> getAllStocks()
        {
            try
            {
                var allStocks = playerEntities.Stocks.ToList();
                //var stocksDTO = Mapping.Mapper.Map<List<StockDTO>>(allStocks);
                return allStocks;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //priceOfStocks() *****************************************************************
        public List<decimal> priceOfStocks(List<StockDetail> stocks)
        {
            List<decimal> prices = new List<decimal>();
            foreach(StockDetail item in stocks)
            {
                prices.Add(item.CurrentPrice);
            }
            return prices;
        }

        //getRecommendationsFromAnalyst()

        public bool priceIsRight(StockDetail stock, decimal currentPrice)
        {
            decimal price = stock.CurrentPrice;
            if (price == currentPrice)
                return true;
            else
                return false;
        }

        public int checkStockQuantity(int playerID, int stockID)
        {
            int stockQuan = 0;

            try
            {
                foreach (var item in playerEntities.PlayerStocks.Where(c => c.PlayerId == playerID && c.StockId == stockID))
                {
                    if (item != null)
                    {
                        stockQuan += item.Quantity; 
                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }

            return stockQuan;
        }

        public decimal amountSpentForStocks(int playerID, int stockID)
        {
            decimal amount = 0;

            try
            {
                foreach (var item in playerEntities.PlayerStocks.Where(c => c.PlayerId == playerID && c.StockId == stockID && c.Quantity>0))
                {
                    if (item == null)
                        break;
                    else
                        amount += item.Quantity * item.UnitPrice;
                }
            }
            catch (Exception e)
            {
                return -1;
            }

            return amount;
        }

        public decimal checkBankBalance(int playerID)
        {
            decimal accBalance = 0;
            try
            {
                var accountDetails = playerEntities.BankAccounts.Where(c => c.PlayerId == playerID).FirstOrDefault();
                accBalance = accountDetails.Balance;
            }
            catch (Exception e)
            {
                return -1;
            }

            return accBalance;
        }

        public int getAccountID(int playerID)
        {
            int accID = 0;

            try
            {
                var acc = playerEntities.BankAccounts.Where(c => c.PlayerId == playerID).FirstOrDefault();
                accID = acc.AccountId;
            }
            catch (Exception e)
            {
                return -1;
            }

            return accID;
        }

        public void Dispose()
        {
            if(playerEntities != null)
            {
                playerEntities.Dispose();
                playerEntities = null;
            }
        }

    }
}
