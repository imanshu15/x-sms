﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X_SMS_REP;
using X_SMS_REP.RequestModel;

namespace X_SMS.Services
{
    public class GameManager
    {
        public GameDTO CreateGame(string playerName,int playerCount,bool isPrivate) {

            GameDTO returnObj = null;

            GameDTO game = new GameDTO();
            game.CreatedPlayer = playerName;
            game.PlayersCount = playerCount;
            game.IsPublic = !isPrivate;

            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakePostRequest("api/Game/CreateGame", game);
                    ResultToken result = apiClient.ConvertObjectToToken(temp);
                    if (result.Success && result.Data != null)
                    {
                        returnObj = JsonConvert.DeserializeObject<GameDTO>(result.Data.ToString());
                    }
                }
            }
            catch (Exception ex) {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return returnObj;
        }

        public PlayerDTO CreatePlayer(string playerName,int gameId,string connectionId) {

            PlayerDTO returnObj = null;

            JoinRequestModel request = new JoinRequestModel();
            request.PlayerName = playerName;
            request.GameId = gameId;
            request.ConnectionId = connectionId;
            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakePostRequest("api/GamePlayer", request);
                    ResultToken result = apiClient.ConvertObjectToToken(temp);
                    if (result.Success && result.Data != null)
                    {
                        returnObj = JsonConvert.DeserializeObject<PlayerDTO>(result.Data.ToString());
                    }

                }
            }  
            catch (Exception ex) {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return returnObj;
        }

        public GameDTO StartGame(int gameId) {

            GameDTO game = null;
            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakePostRequest("api/Game/StartGame", gameId);
                    ResultToken result = apiClient.ConvertObjectToToken(temp);
                    if (result.Success && result.Data != null)
                    {
                        game = JsonConvert.DeserializeObject<GameDTO>(result.Data.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }
            return game;
        }

    }
}