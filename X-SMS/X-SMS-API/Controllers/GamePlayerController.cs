using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;

namespace X_SMS_API.Controllers
{
    public class GamePlayerController : ApiController
    {
        public IHttpActionResult Get(int gameId)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.GetGamePlayerList(gameId);
                return Json(result);
            }
        }

    }
}
