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
    public class HistoryService : IDisposable
    {
        XSmsEntities gameEntities = null;

        public HistoryService()
        {
            gameEntities = new XSmsEntities();
        }

        public List<GameDTO> GetGameList()
        {
            var gameList = gameEntities.Games.Where(x => x.IsActive == false && x.IsCanceled == false && x.IsStarted == false && x.Winner != null).Select(g => new GameDTO()
            {
                GameId = g.GameId,
                GameCode = g.GameCode,
                CreatedPlayer = g.CreatedPlayer,
                EndTime = g.EndTime,
                StartTime = g.StartTime,
                Winner = g.Winner,
                PlayersCount = g.PlayersCount
            }).OrderBy(a => a.StartTime).ToList();

            return gameList;
        }

        public void GetGameSummary(int gameId) {

            var game = gameEntities.Games.FirstOrDefault(a => a.GameId == gameId);
            var players = gameEntities.Players.Where(a => a.GameId == gameId).ToList();
        }

        public List<PortfolioDTO> GetGameTransactions(int gameId) {

            var players = gameEntities.Players.Where(x => x.GameId == gameId && x.IsActive == true).ToList();

            var list = gameEntities.ViewPlayerPortfolios.Where(a => a.GameId == gameId).ToList();

            var returnList = new List<PortfolioDTO>();

            foreach (var player in players) {

                var temp = new PortfolioDTO();
                temp.PlayerId = player.PlayerId;
                temp.PlayerName = player.PlayerName;
                temp.Balance = player.BankAccounts.FirstOrDefault() != null ? player.BankAccounts.FirstOrDefault().Balance : 0;
                temp.Transactions = list.Where(a => a.PlayerId == player.PlayerId).Select(g => new PlayerPortfolioDTO()
                {
                    StockName = g.StockName,
                    Quantity = g.Quantity,
                    PlayerId = g.PlayerId,
                    StockId = g.StockId,
                    GameId = g.GameId,
                    UnitPrice = g.UnitPrice,
                    IsWithdraw = g.IsWithdraw,
                    IsDeposit = g.IsDeposit,
                    Amount = g.Amount,
                    SectorId = g.SectorId,
                    SectorName = g.SectorName
                }).ToList();

                returnList.Add(temp);
            }

            return returnList;
        }

        public void Dispose()
        {
            if (gameEntities != null)
            {
                gameEntities.Dispose();
                gameEntities = null;
            }
        }
    }
}

