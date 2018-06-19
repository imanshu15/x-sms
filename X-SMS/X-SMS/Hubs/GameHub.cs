using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using X_SMS.Services;
using X_SMS_REP;
using X_SMS_REP.RequestModel;

namespace X_SMS.Hubs
{
    public class GameHub : Hub
    {
        
        private object _syncRoot = new object();

        public void CreateGame(string playerName, int playerCount, bool isPrivate)
        {

            bool isSuccess = false;
            GameManager gameManager = new GameManager();

            lock (_syncRoot)
            {
                PlayerDTO player = null;
                GameDTO game = gameManager.CreateGame(playerName, playerCount, isPrivate);
                if (game != null)
                {
                    player = gameManager.CreatePlayer(playerName, game.GameId, Context.ConnectionId);
                    if (player != null)
                    {
                        player.GameCode = game.GameCode;
                        game.Players.Add(player);
                        EntityStateManager.CurrentGames.Add(game);
                        AddPlayer(player);
                        isSuccess = true;
                    }
                }

                if (isSuccess)
                {
                    Groups.Add(Context.ConnectionId, game.GameCode);
                    Clients.Client(Context.ConnectionId).gameCreated(player); // Call method on created players view
                    var games = EntityStateManager.CurrentGames.ToList();
                    var connectedIds = EntityStateManager.Players.Select(a => a.ConnectionId).ToArray();
                    Clients.AllExcept(connectedIds).updateGameList(games);
                }
                else
                {
                    // Needs to be implemented

                }
            }
        }

        public void JoinGame(string playerName, int gameId,string gameCode)
        {
            bool isSuccess = false;
            GameManager gameManager = new GameManager();
            bool isCodeFound = true;

            if (gameId == 0 && gameCode != "")
            {
                var tempGame = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameCode == gameCode);
                if (tempGame == null)
                    isCodeFound = false;
            }

            lock (_syncRoot)
            {
                PlayerDTO player = null;

                if (isCodeFound)
                {
                    var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                    if (game == null)
                        game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameCode == gameCode);

                    if (game != null && game.Players.Count < game.PlayersCount)
                    {
                         player = gameManager.CreatePlayer(playerName, game.GameId, Context.ConnectionId);
                        if (player != null)
                        {
                            player.GameCode = game.GameCode;
                            game.Players.Add(player);
                            AddPlayer(player);
                            isSuccess = true;
                        }
                    }

                    if (isSuccess)
                    {
                        Groups.Add(Context.ConnectionId, game.GameCode);
                        Clients.Client(Context.ConnectionId).joinSuccess(player);
                        Clients.Group(game.GameCode).notifyJoinedPlayers(playerName);
                    }
                    else
                    {
                        //Need to be implemented
                    }
                }
                else {
                    Clients.Client(Context.ConnectionId).invalidGameCode();
                }
            }
        }

        public void IsGameStarted(int gameId)
        {
            GameManager gameManager = new GameManager();
            lock (_syncRoot)
            {
                GameLogicManager gameLogic = new GameLogicManager();

                var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                if (game != null && game.Players.Count == game.PlayersCount)
                {
                    game = gameManager.StartGame(game.GameId);
                    if (game != null)
                    {
                        var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == game.GameId);
                        gameObj.IsStarted = true;
                        gameObj.GameDetail = gameLogic.GetGameData(gameObj.GameId);
                        Clients.Group(game.GameCode).gameStarted(game);
                        SetupGame(gameObj.GameId);
                    }
                    else
                    {
                        // Need to be implemented
                    }
                }
            }
        }


        private void AddPlayer(PlayerDTO player) {
            var client = EntityStateManager.Players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (client == null)
            {
                EntityStateManager.Players.Add(player);
            }
            else {
                //Needs to be implemented throughly
            }
        }

        public void RequestPlayerList(int gameId)
        {
            var game = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);
            Clients.Client(Context.ConnectionId).playerList(game.Players.ToList());
        }

        public void DisconnectPlayer(int gameId)
        {
            GameManager gameManager = new GameManager();
            var player = EntityStateManager.Players.FirstOrDefault(a => a.ConnectionId == Context.ConnectionId);
            var game = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);
            if (player != null)
            {
                bool isGameOver = false;
                EntityStateManager.Players.Remove(player);
                gameManager.DisconnectPlayer(player.PlayerId);

                if (game != null)
                {
                    EntityStateManager.CurrentGames.Remove(game);
                    game.Players.Remove(player);
                }

                Clients.Group(game.GameCode).playerDisconnected(player.PlayerName);
                              
            }
        }

        public void DisconnectGame(int gameId) {
            // Need to be implemented
        }

        public void GetCurrentGameList() {
            var games = EntityStateManager.CurrentGames.ToList();
            Clients.Client(Context.ConnectionId).currentGameList(games);
        }

        //---------------------------------------------------------------------------------------------------------------------------
        // GameBoard 

        private void SetupGame(int gameId)
        {
            var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);
            gameObj.CurrentRound = 0;
            var isFinished = NextRound(gameObj.GameId);
            //System.Threading.Timer timer = null;
            //timer = new System.Threading.Timer(new TimerCallback(y =>
            //{
            //    try
            //    {
            //        var isFinished = NextRound(gameObj.GameId);
            //        if (isFinished)
            //        {
            //            timer.Dispose();
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}));

            //timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private bool NextRound(int gameId) {

             bool isFinished = false;
            var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);

            if (gameObj.CurrentRound > EntityStateManager.NumberOfRounds)
            {
                isFinished = true;
                //GAME OVER
            }
            else {
               gameObj.CurrentRound += 1;
               if(gameObj.CurrentRound == 1)
                    System.Threading.Thread.Sleep(3000);

                var turnDetails = gameObj.GameDetail.TurnDetail.FirstOrDefault(x => x.Turn == gameObj.CurrentRound);
                if(turnDetails != null)
                    Clients.Group(gameObj.GameCode).startRound(turnDetails);

            }

            return isFinished;
        }

        public void BuyStocks(int gameId,int playerId,int sectorId,StockDetail stockTo,int quantity)
        {
            GameLogicManager gameLogic = new GameLogicManager();
            lock (_syncRoot)
            {
                var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                if (game != null)
                {
                   var turn = game.GameDetail.TurnDetail.FirstOrDefault(x => x.Turn == game.CurrentRound);
                    if (turn != null) {
                        var sector = turn.Sectors.FirstOrDefault(y => y.Sector.SectorId == sectorId);
                        if (sector != null) {
                            var stock = sector.Stocks.FirstOrDefault(z => z.StockId == stockTo.StockId);

                            var token = gameLogic.BuyStocks(playerId, stockTo, quantity,stock.CurrentPrice);

                            if (token.Success)
                            {
                                if (token.Data != null)
                                {
                                    var player = game.Players.FirstOrDefault(o => o.PlayerId == playerId);
                                    var temp = (PlayerTransactionsDTO)token.Data;
                                    temp.PlayerName = player.PlayerName;
                                    temp.StockName = stock.StockName;

                                    PlayerStock pStock = new PlayerStock();
                                    pStock.Quantity = quantity;
                                    pStock.SectorId = sectorId;
                                    pStock.StockId = stockTo.StockId;
                                    pStock.StockName = stock.StockName;
                                    pStock.SectorName = sector.Sector.SectorName;
                                    pStock.BoughtPrice = stock.CurrentPrice;

                                    player.PlayerStocks.Add(pStock);
                                    player.BankAccount.Balance -= (quantity * stock.CurrentPrice);
                                    Clients.Client(Context.ConnectionId).stockBuySuccess(player.BankAccount.Balance);
                                    Clients.Group(game.GameCode).playerBoughtStock(temp);
                                }
                                else {
                                    token.Success = false;
                                    token.Message = "An error occurred";
                                }

                            }

                            if(!token.Success){
                                Clients.Client(Context.ConnectionId).stockBuyFailed(token);
                            }
                        }
                    }
                }
            }
        }

        public void GetPlayerStocks(int gameId,int playerId)
        {
            GameLogicManager gameLogic = new GameLogicManager();
            lock (_syncRoot)
            {
                var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                var player = game.Players.FirstOrDefault(x => x.PlayerId == playerId);
                var playerStocks = player.PlayerStocks.ToList();
                foreach (var temp in playerStocks) {
                    temp.CurrentPrice = gameLogic.GetStockValue(gameId, temp.SectorId, temp.StockId);
                    temp.IsIncreased = (temp.CurrentPrice > temp.BoughtPrice) ? true : false;
                }
                Clients.Client(Context.ConnectionId).loadPlayerStocksList(playerStocks.GroupBy(x => x.StockId).ToList());
            }
        }

        public void SellStocks(int gameId, int playerId, int sectorId, StockDetail stockTo, int quantity)
        {
            GameLogicManager gameLogic = new GameLogicManager();
            lock (_syncRoot)
            {
                var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                if (game != null)
                {
                    var turn = game.GameDetail.TurnDetail.FirstOrDefault(x => x.Turn == game.CurrentRound);
                    if (turn != null)
                    {
                        var sector = turn.Sectors.FirstOrDefault(y => y.Sector.SectorId == sectorId);
                        if (sector != null)
                        {
                            var stock = sector.Stocks.FirstOrDefault(z => z.StockId == stockTo.StockId);

                            var token = gameLogic.SellStocks(playerId, stockTo, quantity, stock.CurrentPrice);

                            if (token.Success)
                            {
                                if (token.Data != null)
                                {
                                    var player = game.Players.FirstOrDefault(o => o.PlayerId == playerId);
                                    var temp = (PlayerTransactionsDTO)token.Data;
                                    temp.PlayerName = player.PlayerName;
                                    temp.StockName = stock.StockName;

                                    var tempStocks = player.PlayerStocks.Where(b => b.StockId == stockTo.StockId).ToList();

                                    foreach (var tempStock in tempStocks) {
                                        tempStock.Quantity -= quantity;
                                        if (tempStock.Quantity < 0)
                                        {
                                            quantity = -(tempStock.Quantity);
                                        }
                                        else if (tempStock.Quantity == 0) {
                                            break;
                                        }
                                    }
                                    var stockToRemove = tempStocks.Where(c => c.Quantity <= 0).Select(x => x.SectorId).ToList();

                                    foreach (var removeStock in stockToRemove)
                                    {
                                        var tempStockToRemove = tempStocks.Where(a => a.StockId == removeStock).ToList();
                                        foreach (var tempStk in tempStockToRemove) {
                                            if (tempStk.Quantity == 0)
                                                tempStocks.Remove(tempStk);
                                        }
                                    }

                                    player.BankAccount.Balance += (quantity * stock.CurrentPrice);
                                    Clients.Client(Context.ConnectionId).stockSellSuccess(player.BankAccount.Balance);
                                    Clients.Group(game.GameCode).playerSoldStock(temp);
                                }
                                else
                                {
                                    token.Success = false;
                                    token.Message = "An error occurred";
                                }

                            }

                            if (!token.Success)
                            {
                                Clients.Client(Context.ConnectionId).stockSellFailed(token);
                            }
                        }
                    }
                }
            }
       }
    }
}