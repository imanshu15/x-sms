using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;
using X_SMS_REP;
using X_SMS_REP.RequestModel;

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

        [Route("api/Game/PlayerExist")]
        [HttpGet]
        public IHttpActionResult PlayerExist(string playerName)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.DoesPlayerExist(playerName);
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
        public ResultToken StartGame([FromBody] int gameId)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.StartGame(gameId);
                return result;
            }
        }

        [Route("api/Game/GetPlayerList")]
        [HttpPost]
        public IHttpActionResult GetPlayerList(int gameId)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.GetGamePlayerList(gameId);
                return Json(result);
            }
        }

        [Route("api/Game/JoinGame")]
        [HttpPost]
        public ResultToken JoinGame([FromBody] JoinRequestModel request)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.JoinGame(request);
                return result;
            }
        }

        [Route("api/Game/RemovePlayer")]
        [HttpPost]
        public ResultToken RemovePlayer([FromBody] int playerId)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.RemovePlayer(playerId);
                return result;
            }
        }


        [Route("api/game/sectors")]
        [HttpGet]
        public IHttpActionResult GetSectors()
        {
            using (GameService gameService = new GameService())
            {
                ResultToken token = new ResultToken();
                token.Success = true;
                token.Data = gameService.GetSectorsList();
                return Json(token);
            }
        }
    }
}