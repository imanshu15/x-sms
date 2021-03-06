﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public void CreateGame(string playerName, int playerCount, bool isPrivate,bool isPlayerAi)
        {

            bool isSuccess = false;
            GameManager gameManager = new GameManager();

            lock (_syncRoot)
            {
                PlayerDTO player = null;
                GameDTO game = gameManager.CreateGame(playerName, playerCount, isPrivate);
                if (game != null)
                {
                    game.JoinedPlayerCount = 0;
                    player = gameManager.CreatePlayer(playerName, game.GameId, Context.ConnectionId);
                    if (player != null)
                    {
                        player.GainAmount = 0;
                        player.SpendAmount = 0;
                        player.GameCode = game.GameCode;
                        player.NoOfTransactions = 0;
                        game.Players.Add(player);
                        game.JoinedPlayerCount += 1;

                        EntityStateManager.CurrentGames.Add(game);
                        AddPlayer(player);
                        //CREATE PLAYER AI
                        if (isPlayerAi)
                        {
                            JoinGame("PLAYER_AI", game.GameId, "", true);
                            game.IsPlayerAIAvailable = true;
                        }
                        else {
                            game.IsPlayerAIAvailable = false;
                        }
                        isSuccess = true;
                    }

                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(1200000);
                        CheckGameEnded(game.GameId);
                    });
                }

                if (isSuccess)
                {
                    Groups.Add(Context.ConnectionId, game.GameCode);
                    Clients.Client(Context.ConnectionId).gameCreated(player); // Call method on created players view
                    var games = EntityStateManager.CurrentGames.Where(a => a.IsPublic == true).ToList();
                    var connectedIds = EntityStateManager.Players.Select(a => a.ConnectionId).ToArray();
                    Clients.AllExcept(connectedIds).updateGameList(games);
                }
                else
                {
                    Clients.Client(Context.ConnectionId).gameCreationFailed();
                }
            }
        }

        public void JoinGame(string playerName, int gameId,string gameCode, bool isPlayerAI = false)
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

                    if ((game != null && !game.IsPlayerAIAvailable && game.Players.Count < game.PlayersCount) || (game != null && game.IsPlayerAIAvailable && game.Players.Count < game.PlayersCount + 1))
                    {
                        if (!isPlayerAI)
                        {
                            game.JoinedPlayerCount += 1;
                            player = gameManager.CreatePlayer(playerName, game.GameId, Context.ConnectionId);
                        }
                        else
                        {
                            player = gameManager.CreatePlayer(playerName, game.GameId, "NO CONNECTION");
                        }
                        player.IsPlayerAI = isPlayerAI;

                        if (player != null)
                        {
                            player.SpendAmount = 0;
                            player.GainAmount = 0;
                            player.GameCode = game.GameCode;
                            player.NoOfTransactions = 0;
                            game.Players.Add(player);
                            AddPlayer(player);
                            isSuccess = true;
                        }
                    }

                    if (isSuccess)
                    {
                        if (!isPlayerAI) {
                            Groups.Add(Context.ConnectionId, game.GameCode);
                            Clients.Client(Context.ConnectionId).joinSuccess(player);
                        }
                        Clients.Group(game.GameCode).notifyJoinedPlayers(playerName);
                    }
                    else
                    {
                        if (!isPlayerAI)
                            Clients.Client(Context.ConnectionId).gameJoinFailed();
                    }
                }
                else {
                    if (!isPlayerAI)
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
                if ((game != null && !game.IsPlayerAIAvailable && game.Players.Count == game.PlayersCount) || (game != null && game.IsPlayerAIAvailable && game.Players.Count == (game.PlayersCount + 1)))
                {
                    game = gameManager.StartGame(game.GameId);
                    if (game != null)
                    {
                        var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == game.GameId);
                        gameObj.IsStarted = true;
                        Clients.Group(game.GameCode).allPlayersConnected();

                        // SET UP GAME DATA
                        var sectors = gameLogic.GetSectors();
                        Clients.Group(game.GameCode).setUpSectors(sectors);

                        gameObj.GameDetail = gameLogic.GetGameData(gameObj.GameId);
                        Clients.Group(game.GameCode).setUpGameData(gameObj.GameDetail);
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
            string gameCode = "";
            GameManager gameManager = new GameManager();
            //var player = EntityStateManager.Players.FirstOrDefault(a => a.ConnectionId == Context.ConnectionId);
            var game = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);
            gameCode = game.GameCode;
            var players = game.Players; 
            if (players != null)
            {
                foreach (var player in players)
                {               
                    EntityStateManager.Players.Remove(player);
                    gameManager.DisconnectPlayer(player.PlayerId);
                    game.Players.Remove(player);
                }
                gameManager.DisconnectGame(game.GameId);
                EntityStateManager.CurrentGames.Remove(game);
                Clients.Group(gameCode).playerDisconnected();                    
            }
        }

        public void DisconnectGame(int gameId) {
            // Need to be implemented
        }

        public void GetCurrentGameList() {
            var games = EntityStateManager.CurrentGames.Where(a => a.IsPublic == true && a.IsStarted == false).OrderByDescending(b => b.StartTime).ToList();
            Clients.Client(Context.ConnectionId).currentGameList(games);
        }

        private void CheckGameEnded(int gameId) {

            var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);

            if (game != null)
            {
                DisconnectPlayer(game.GameId);
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------
        // GameBoard 

        private void SetupGame(int gameId)
        {
            var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);
            gameObj.CurrentRound = 0;
            // var isFinished = NextRound(gameObj.GameId);
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer(new TimerCallback(y =>
            {
                try
                {
                    var isFinished = NextRound(gameObj.GameId);
                    if (isFinished)
                    {
                        timer.Dispose();
                    }
                }
                catch
                {
                }
            }));

            timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private bool NextRound(int gameId) {

            GameManager gameManager = new GameManager();
            bool isFinished = false;
            var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);

            if (gameObj.CurrentRound >= EntityStateManager.NumberOfRounds)
            {
                var gameCode = gameObj.GameCode;
                Clients.Group(gameCode).gameIsOver();
                //GAME OVER
                isFinished = true;
                var players = gameObj.Players.ToList();
                
                var winner = players.OrderByDescending(a => a.BankAccount.Balance).ThenByDescending(b => b.NoOfTransactions).FirstOrDefault();
                GetGameLeaders(gameObj.GameId);
                if (players != null)
                {
                    foreach (var player in players)
                    {
                        EntityStateManager.Players.Remove(player);
                        gameManager.DisconnectPlayer(player.PlayerId);
                        gameObj.Players.Remove(player);
                    }
                }
                gameManager.GameEnded(winner);
                EntityStateManager.CurrentGames.Remove(gameObj);
                Clients.Group(gameCode).gameOver(winner.GameId);
            }
            else {
               gameObj.CurrentRound += 1;
                if (gameObj.CurrentRound == 1)
                {
                    System.Threading.Thread.Sleep(3000);
                }
                else {
                    foreach (var ply in gameObj.Players) {
                        GetPlayerStocks(gameObj.GameId,ply.PlayerId);
                    }
                }

                var turnDetails = gameObj.GameDetail.TurnDetail.FirstOrDefault(x => x.Turn == gameObj.CurrentRound);
                if(turnDetails != null)
                    Clients.Group(gameObj.GameCode).startRound(turnDetails);

                //PlayerAI player = new PlayerAI(gameObj);
                if (gameObj.IsPlayerAIAvailable)
                {
                    GameLogicManager gameLogic = new GameLogicManager();
                    var playerAIData = gameLogic.GetPlayerAIData(gameObj);
                    decideBuySellForAI(playerAIData);
                }
                GetGameLeaders(gameObj.GameId);
            }

            return isFinished;
        }

        public void GetGameLeaders(int gameId) {

            var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(a => a.GameId == gameId);
            var gamePlayers = gameObj.Players.Where(a => a.IsActive == true).OrderByDescending(b => b.BankAccount.Balance).ThenByDescending(c => c.NoOfTransactions).ToList();
            Clients.Group(gameObj.GameCode).loadGameLeaders(gamePlayers);
        }

        public void BuyStocks(int gameId,int playerId,int sectorId,int stockId,int quantity)
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
                            var stock = sector.Stocks.FirstOrDefault(z => z.StockId == stockId);
                            var player = game.Players.FirstOrDefault(o => o.PlayerId == playerId);
                            var token = gameLogic.BuyStocks(playerId, stock, quantity,stock.CurrentPrice);

                            if (token.Success)
                            {
                                if (token.Data != null)
                                {
                                    var temp = (PlayerTransactionsDTO)token.Data;
                                    temp.PlayerName = player.PlayerName;
                                    temp.StockName = stock.StockName;

                                    PlayerStock pStock = new PlayerStock();
                                    pStock.Quantity = quantity;
                                    pStock.SectorId = sectorId;
                                    pStock.StockId = stock.StockId;
                                    pStock.StockName = stock.StockName;
                                    pStock.SectorName = sector.Sector.SectorName;
                                    pStock.BoughtPrice = stock.CurrentPrice;

                                    player.PlayerStocks.Add(pStock);
                                    player.BankAccount.Balance -= (quantity * stock.CurrentPrice);
                                    player.NoOfTransactions += 1;
                                    player.SpendAmount += quantity * stock.CurrentPrice;

                                    BalanceDTO balance = new BalanceDTO();
                                    balance.OpeningBalance = 1000;
                                    balance.AllocatedPrice = player.SpendAmount;
                                    balance.ProfitPrice = player.GainAmount;
                                    balance.Balance = player.BankAccount.Balance;

                                    if (!player.IsPlayerAI)
                                        Clients.Client(Context.ConnectionId).stockBuySuccess(balance);

                                    Clients.Group(game.GameCode).playerBoughtStock(temp);

                                    GetGameLeaders(game.GameId);
                                }
                                else {
                                    token.Success = false;
                                    token.Message = "An error occurred";
                                }

                            }

                            if(!token.Success && !player.IsPlayerAI)
                            {
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
                    temp.Percentage = ((temp.CurrentPrice - temp.BoughtPrice)/ temp.BoughtPrice) * 100;
                    temp.Profit = (temp.CurrentPrice * temp.Quantity) - (temp.BoughtPrice * temp.Quantity);
                }
                Clients.Client(player.ConnectionId).loadPlayerStocksList(playerStocks);
            }
        }

        public void SellStocks(int gameId, int playerId, int sectorId, int stockId, int quantity)
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
                            var stock = sector.Stocks.FirstOrDefault(z => z.StockId == stockId);
                            var player = game.Players.FirstOrDefault(o => o.PlayerId == playerId);

                            var token = gameLogic.SellStocks(playerId, stock, quantity, stock.CurrentPrice);

                            if (token.Success)
                            {
                                if (token.Data != null)
                                {
                                    var temp = (PlayerTransactionsDTO)token.Data;
                                    temp.PlayerName = player.PlayerName;
                                    temp.StockName = stock.StockName;

                                    var tempStocks = player.PlayerStocks.Where(b => b.StockId == stockId).ToList();

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
                                    var stockToRemove = tempStocks.Where(c => c.Quantity <= 0).Select(x => x.StockId).ToList();

                                    foreach (var removeStock in stockToRemove)
                                    {
                                        var tempStockToRemove = tempStocks.Where(a => a.StockId == removeStock).ToList();
                                        foreach (var tempStk in tempStockToRemove) {
                                            if (tempStk.Quantity <= 0)
                                                player.PlayerStocks.Remove(tempStk);
                                        }
                                    }

                                    player.BankAccount.Balance += (quantity * stock.CurrentPrice);
                                    player.GainAmount = quantity * stock.CurrentPrice;
                                    temp.PlayerAccBalance = player.BankAccount.Balance;

                                    BalanceDTO balance = new BalanceDTO();
                                    balance.OpeningBalance = 1000;
                                    balance.AllocatedPrice = player.SpendAmount;
                                    balance.ProfitPrice = player.GainAmount;
                                    balance.Balance = player.BankAccount.Balance;

                                    if (!player.IsPlayerAI)
                                        Clients.Client(Context.ConnectionId).stockSellSuccess(balance);
                                    Clients.Group(game.GameCode).playerSoldStock(temp);

                                    GetGameLeaders(game.GameId);
                                }
                                else
                                {
                                    token.Success = false;
                                    token.Message = "An error occurred";
                                }

                            }

                            if (!token.Success && !player.IsPlayerAI)
                            {
                                Clients.Client(Context.ConnectionId).stockSellFailed(token);
                            }
                        }
                    }
                }
            }
       }

        public void decideBuySellForAI(List<AIBuySellDetails> list)
        {
            try
            {
                foreach (AIBuySellDetails item in list)
                {
                    if (item.Buy)
                        BuyStocks(item.GameId, item.PlayerId, item.SectorId, item.Stock.StockId, item.Quantity);
                    else if (!item.Buy)
                    {
                        SellStocks(item.GameId, item.PlayerId, item.SectorId, item.Stock.StockId, item.Quantity);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}