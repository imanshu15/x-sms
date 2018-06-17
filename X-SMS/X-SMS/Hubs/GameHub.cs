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

                GameDTO game = gameManager.CreateGame(playerName, playerCount, isPrivate);
                if (game != null)
                {
                    PlayerDTO player = gameManager.CreatePlayer(playerName, game.GameId, Context.ConnectionId);
                    if (player != null)
                    {
                        game.Players.Add(player);
                        EntityStateManager.CurrentGames.Add(game);
                        AddPlayer(player);
                        isSuccess = true;
                    }
                }

                if (isSuccess)
                {
                    Groups.Add(Context.ConnectionId, game.GameCode);
                    Clients.Client(Context.ConnectionId).gameCreated(game); // Call method on created players view
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
                if (isCodeFound)
                {
                    var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                    if (game == null)
                        game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameCode == gameCode);

                    if (game != null && game.Players.Count < game.PlayersCount)
                    {
                        PlayerDTO player = gameManager.CreatePlayer(playerName, game.GameId, Context.ConnectionId);
                        if (player != null)
                        {
                            game.Players.Add(player);
                            EntityStateManager.CurrentGames.Add(game);
                            AddPlayer(player);
                            isSuccess = true;
                        }
                    }

                    if (isSuccess)
                    {
                        Groups.Add(Context.ConnectionId, game.GameCode);
                        Clients.Client(Context.ConnectionId).joinSuccess(game);
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
            gameObj.CurrentRound = 1;
            var firstTurn = gameObj.GameDetail.TurnDetail[0];
            firstTurn
            Clients.Group(gameObj.GameCode).firstRound(firstTurn);

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

            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));
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
               var currentTurn = gameObj.CurrentRound;
               gameObj.CurrentRound += 1;
               
                var turnDetails = gameObj.GameDetail.TurnDetail[currentTurn];
                Clients.Group(gameObj.GameCode).startRound(firstTurn);

            }

            return isFinished;
        }
    }
}