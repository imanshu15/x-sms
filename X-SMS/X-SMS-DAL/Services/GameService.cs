﻿using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Database;
using X_SMS_DAL.Global;
using X_SMS_DAL.Mapper;
using X_SMS_REP;
using X_SMS_REP.RequestModel;

namespace X_SMS_DAL.Services
{
    public class GameService : IDisposable
    {
        XSmsEntities gameEntities = null;
       
        public GameService() {
            gameEntities = new XSmsEntities();
        }

        public ResultToken CreateGame(string playerName,int noOfPlayers,bool isPublic) {

            ResultToken result = new ResultToken();
            result.Success = true;
            try
            {
                Game newGame = new Game();
                newGame.StartTime = DateTime.UtcNow;
                newGame.PlayersCount = noOfPlayers;
                newGame.CreatedPlayer = playerName;
                newGame.IsPublic = isPublic;
                newGame.IsActive = true;
                newGame.GameCode = GenerateGameCode();
                gameEntities.Games.Add(newGame);
                gameEntities.SaveChanges();
                GameDTO gameDto = Mapping.Mapper.Map<GameDTO>(newGame);
                result.Data = gameDto;
            }
            catch (Exception ex) {
                result.Success = false;
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }
            return result;
        }

        public List<GameDTO> GetOpenGameList() {

            var gameList = gameEntities.Games.Where(a => a.IsActive == true && a.IsPublic == true && a.IsStarted == false).OrderByDescending(b => b.GameId).ToList();

            var gameDTOList = Mapping.Mapper.Map<List<GameDTO>>(gameList);

            return gameDTOList;

        }

        public bool DoesPlayerExist(string playerName) {

            var player = gameEntities.Players.Where(a => a.PlayerName == playerName && a.IsActive == true).Count();

            return (player >= 1 ? true : false);
        }

        public List<PlayerDTO> GetGamePlayerList(int gameId)
        {

            var playesList = gameEntities.Players.Where(a => a.GameId == gameId && a.IsActive == true).OrderByDescending(b => b.GameId).ToList();

            var playeDTOList = Mapping.Mapper.Map<List<PlayerDTO>>(playesList);

            return playeDTOList;

        }

        public ResultToken JoinGame(JoinRequestModel request) {

            ResultToken result = new ResultToken();
            result.Success = true;
            try
            {
                if (request.GameId == 0)
                {
                    request.GameId = gameEntities.Games.Where(a => a.GameCode.Equals(request.GameCode)).Select(x => x.GameId).FirstOrDefault();
                }

                Player player = new Player();
                player.GameId = request.GameId;
                player.PlayerName = request.PlayerName;
                player.ConnectionId = request.ConnectionId;
                player.IsActive = true;

                gameEntities.Players.Add(player);
                gameEntities.SaveChanges();
                PlayerDTO playerDTO = Mapping.Mapper.Map<PlayerDTO>(player);
                result.Data = playerDTO;
            }
            catch (Exception ex)
            {
                result.Success = false;
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;

        }

        public ResultToken RemoveGame(int gameId)
        {
            ResultToken result = new ResultToken();
            result.Success = true;
            try
            {
                var game = gameEntities.Games.FirstOrDefault(a => a.GameId == gameId);
                if (game != null)
                {
                    game.IsStarted = false;
                    game.IsCanceled = true;
                    game.IsActive = false;
                    gameEntities.SaveChanges();
                   // GameDTO gameDto = Mapping.Mapper.Map<GameDTO>(game);
                    //result.Data = gameDto;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;
        }

        public List<PlayerDTO> GetWinner(int gameId)
        {
            List<PlayerDTO> returnList = new List<PlayerDTO>();

            var players = gameEntities.Players.Where(a => a.GameId == gameId).ToList();

            foreach (var player in players) {
                PlayerDTO temp = new PlayerDTO();
                temp.PlayerId = player.PlayerId;
                temp.PlayerName = player.PlayerName;

                var bank = gameEntities.BankAccounts.FirstOrDefault(x => x.PlayerId == player.PlayerId);
                var transCount = gameEntities.Transcations.Where(a => a.AccountId == bank.AccountId).Count();
                BankAccountDTO bankAcc = Mapping.Mapper.Map<BankAccountDTO>(bank);
                temp.BankAccount = bankAcc;
                temp.NoOfTransactions = transCount;

                returnList.Add(temp);
            }

            return returnList.OrderByDescending(a => a.BankAccount.Balance).ThenByDescending(x => x.NoOfTransactions).ToList();
        }

        public object GameOver(PlayerDTO winner)
        {
            ResultToken result = new ResultToken();
            result.Success = true;
            try
            {
                var game = gameEntities.Games.FirstOrDefault(a => a.GameId == winner.GameId);
                if (game != null)
                {
                    game.IsStarted = false;
                    game.IsCanceled = false;
                    game.IsActive = false;
                    game.Winner = winner.PlayerName;
                    game.EndTime = DateTime.Now;
                    gameEntities.SaveChanges();
                    //GameDTO gameDto = Mapping.Mapper.Map<GameDTO>(game);
                   // result.Data = gameDto;
                }
                var details = GameDataManager.gameDetails.FirstOrDefault(a => a.Key == winner.GameId);
                if (details.Value != null)
                {
                    Database.GameDetail temp = new Database.GameDetail();
                    temp.GameId = winner.GameId;
                    temp.Details = details.Value.ToString();
                    gameEntities.GameDetails.Add(temp);
                    gameEntities.SaveChanges();
                    GameDataManager.gameDetails.Remove(details.Key);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;
        }

        public List<SectorDTO> GetSectorsList()
        {
           var sectors = gameEntities.Sectors.ToList();
            List<SectorDTO> sectorsList = Mapping.Mapper.Map<List<SectorDTO>>(sectors);
            return sectorsList;
        }

        public ResultToken StartGame(int gameId) {

            ResultToken result = new ResultToken();
            result.Success = true;

            var game = gameEntities.Games.FirstOrDefault(a => a.GameId == gameId);
            try { 
                if (game != null) {
                    game.IsStarted = true;
                    gameEntities.SaveChanges();
                    GameDTO gameDto = Mapping.Mapper.Map<GameDTO>(game);
                    result.Data = gameDto;

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;
        }


        public ResultToken RemovePlayer(int playerId)
        {

            ResultToken result = new ResultToken();
            result.Success = true;
            try
            { 
                var player = gameEntities.Players.FirstOrDefault(a => a.PlayerId == playerId);
                if (player != null)
                {
                    player.IsActive = false;
                    gameEntities.SaveChanges();
                    //PlayerDTO playerDTO = Mapping.Mapper.Map<PlayerDTO>(player);
                   // result.Data = playerDTO;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return result;

        }

        public string GenerateGameCode()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            GuidString = GuidString.Replace("/", "");
            return GuidString;
        }

        public void Dispose()
        {
            if (gameEntities != null) {
                gameEntities.Dispose();
                gameEntities = null;
            }
        }
    }
}
