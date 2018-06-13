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
    public class GamePlayerController : ApiController
    {

        [Route("api/Player/BuyStock")]
        public IHttpActionResult GetGameList(int stockId,int amount,int playerId)
        {
            using (GameService gameService = new GameService())
            {
                var result = gameService.GetOpenGameList();
                return Json(result);
            }
        }

    }
}
