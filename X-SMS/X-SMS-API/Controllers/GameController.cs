using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;
using X_SMS_REP;

namespace X_SMS_API.Controllers
{
    public class GameController : ApiController
    {
        private const int MaxPlayersCount = 4;

        public ResultToken Post([FromBody] GameDTO game)
        {
            using (GameService gameService = new GameService()) {
                var result = gameService.CreateGame(game.playerName, game.PlayersCount);
                return result;
            }
        }
    }
}