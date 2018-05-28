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

        [Route("api/Game")]
        public IHttpActionResult GetGameList() {
            using (GameService gameService = new GameService())
            {
                var result = gameService.GetOpenGameList();
                return Json(result);
            }
        }

        [Route("api/Game/CreateGame")]
        [HttpPost]
        public ResultToken CreateGame([FromBody] GameDTO game)
        {
            using (GameService gameService = new GameService()) {
                var result = gameService.CreateGame(game.CreatedPlayer, game.PlayersCount,game.IsPublic);
                return result;
            }
        }

        [Route("api/Game/StartGame")]
        [HttpPost] 
        public ResultToken StartGame(int gameId)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.StartGame(gameId);
                return result;
            }
        }
    }
}