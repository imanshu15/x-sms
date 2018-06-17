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
  //  [RoutePrefix("api/trends")]
    public class TrendController : ApiController
    {
       // [Route("getGameTrends")]
       // [HttpGet]
        public IHttpActionResult GetGameTrends(int gameId)
        {
            using (GameTrendService gameTrendService = new GameTrendService())
            {
                var result = gameTrendService.GetGameTrendList(gameId);
                return Json(result);
            }
        }
    }
}