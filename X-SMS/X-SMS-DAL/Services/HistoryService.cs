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
            }).ToList();

            return gameList;
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

