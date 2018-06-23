using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;

namespace X_SMS_API.Controllers
{
    public class AnalystController : ApiController
    {
        public IHttpActionResult GetAnalyst(int gameId, int currentTurn)
        {
            using (AnalystService analystService = new AnalystService())
            {
                var result = analystService.getRecommendations(gameId, currentTurn);
                var tempResult = result != null ? result.Where(a => a.Turn == currentTurn).ToList() : null;
                return Json(tempResult);
            }
        }
    }
}
