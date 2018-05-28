using System;
using System.Collections.Generic;
using System.Linq;
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
                    Clients.Group(game.GameCode).updateGameList(playerName);
                    //Clients.All.gameCreated(game);
                    //Clients.All.gameCreated(game);
                }
                else
                {
                    // Needs to be implemented

                }
            }
        }

        public void JoinGame(string playerName, int gameId)
        {
            bool isSuccess = false;
            GameManager gameManager = new GameManager();

            lock (_syncRoot)
            {
                var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                if (game != null && game.Players.Count < game.PlayersCount)
                {
                    PlayerDTO player = gameManager.CreatePlayer(playerName, gameId, Context.ConnectionId);
                    if (player != null) {
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
                else {
                    //Need to be implemented
                }
            }
        }

        public void IsGameStarted(int gameId)
        {
            GameManager gameManager = new GameManager();
            lock (_syncRoot)
            {
                var game = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == gameId);
                if (game != null && game.Players.Count == game.PlayersCount)
                {
                    game = gameManager.StartGame(game.GameId);
                    if (game != null)
                    {
                        var gameObj = EntityStateManager.CurrentGames.FirstOrDefault(x => x.GameId == game.GameId);
                        gameObj.IsStarted = true;

                       Clients.Group(game.GameCode).gameStarted(game);
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
            Clients.Client(Context.ConnectionId).playerList(game.Players.FirstOrDefault());
        }


    }
}