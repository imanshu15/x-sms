using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using X_SMS_REP;
using X_SMS_API.AIHelper;

namespace X_SMS.AIHelper
{
    public class PlayerAI : IDisposable
    {
        PlayerAILogics aiLogics = null;
        GameDTO game = null;
        static Timer timer = null;
        string name = "COMPUTER_AI";
        PlayerDTO player = null;
        List<TurnDetail> prevTurnDetails = null;
        List<StockDetail> ownStocks = null;
        List<AIBuySellDetails> list = null;

        public PlayerAI(GameDTO gameObj)
        {
            aiLogics = new PlayerAILogics();
            initiateCalculation(gameObj);
            list = new List<AIBuySellDetails>();
        }

        private void initiateCalculation(GameDTO gameObj)
        {
            //get current AI player
            player = gameObj.Players.Where(c => c.PlayerName.Contains(name)).FirstOrDefault();
            //get all turns in game
            List<TurnDetail> allTurnDetails = gameObj.GameDetail.TurnDetail.ToList();
            //get only passed turns in game to check historicol data
            for (int i = 1; i < gameObj.CurrentRound; i++)
            {
                TurnDetail turn = allTurnDetails.Where(c => c.Turn == i).FirstOrDefault();
                prevTurnDetails.Add(turn);
            }
            //get AI's stocks n convert to suitable obj type
            ownStocks = map_StockDetail_PlayerStock(player.PlayerStocks);
            //validation
            if(player != null && prevTurnDetails != null && ownStocks != null)
            {
                game = gameObj;
                aiLogics.setGame(game);

                if (gameObj.CurrentRound == 1)
                {
                    try
                    {
                        aiLogics.BuyStocksForFirstTime(player.PlayerId); 
                        //check for null own stocks ?
                        aiLogics.SetBuySellForAI(ownStocks, player.PlayerId);
                        list = aiLogics.returnBuySellList();
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    startCalculations(player.PlayerId, prevTurnDetails, ownStocks);
                }
            }      
        }

        private void startCalculations(int playerID, List<TurnDetail> prevTurns, List<StockDetail> ownStocks)
        {
            try
            {
                aiLogics.CheckHistoryForBuy(prevTurns, ownStocks);
                aiLogics.CheckHistoryForSell(prevTurns, ownStocks);
                aiLogics.SetBuySellForAI(ownStocks, playerID);
                list = aiLogics.returnBuySellList();
            }
            catch (Exception)
            {
            }
        }

        //private void startThread()
        //{
        //    DateTime started = DateTime.Now;

        //    timer = new Timer(TimeSpan.FromSeconds(15).TotalMilliseconds);
        //    if (!isMinuteUp(started))
        //    {
        //        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        //    }
        //    else
        //        timer.Dispose(); //DISPOSE TIMER ???
        //    timer.Enabled = true;
        //    timer.AutoReset = true;
        //    timer.Start();
        //}
       
        //private void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        startCalculations(player.PlayerId, prevTurnDetails, ownStocks);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private bool isMinuteUp(DateTime started)
        //{
        //    if (started >= DateTime.Now)
        //        return true;
        //    else
        //        return false; 
        //}

        private List<StockDetail> map_StockDetail_PlayerStock(List<PlayerStock> playerStocks)
        {
            List<StockDetail> stocks = new List<StockDetail>();
            foreach(PlayerStock item in playerStocks)
            {
                StockDetail stock = new StockDetail();
                stock.StockId = item.StockId;
                stock.SectorId = item.SectorId;
                stock.StockName = item.StockName;
                stock.StartingPrice = item.BoughtPrice;
                stock.CurrentPrice = item.CurrentPrice;
                stocks.Add(stock);
            }
            return stocks;
        }

        public List<AIBuySellDetails> returnBuySellList()
        {
            if (list != null)
            {
                return list;
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (aiLogics != null)
            {
                aiLogics.Dispose();
                aiLogics = null;
            }
            game = null;
            timer.Dispose();
            timer = null;
            player = null;
            prevTurnDetails = null;
            ownStocks = null;
            list = null;
        }
    }
}