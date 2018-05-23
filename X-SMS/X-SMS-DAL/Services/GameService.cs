using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Database;
using X_SMS_REP;

namespace X_SMS_DAL.Services
{
    public class GameService : IDisposable
    {
        XSmsEntities gameEntities = null;

        public GameService() {
            gameEntities = new XSmsEntities();
        }

        public ResultToken CreateGame(string playerName,int noOfPlayers) {

            ResultToken result = new ResultToken();
            result.Success = true;
            try
            {
                Game newGame = new Game();
                newGame.StartTime = DateTime.UtcNow;
                newGame.PlayersCount = noOfPlayers;
                newGame.CreatedPlayer = playerName;
                newGame.GameCode = GenerateGameCode();
                gameEntities.Games.Add(newGame);
                gameEntities.SaveChanges();

                result.Data = newGame;
            }
            catch (Exception ex) {
                result.Success = false;
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
