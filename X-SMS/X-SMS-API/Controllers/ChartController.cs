using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;

namespace X_SMS_API.Controllers
{
    public class ChartController : ApiController
    {
        [Route("api/Chart/StockValues")]
        public IHttpActionResult GetStockValues(int gameId,int sectorId,int stockId,int turn)
        {
            using (ChartService chartService = new ChartService())
            {
                var result = chartService.GetStocksValues(gameId,sectorId,stockId,turn);
                return Json(result);
            }
        }

        [Route("api/Chart/SectorStocksValues")]
        public IHttpActionResult GetSectorStockValues(int gameId, int sectorId, int turn)
        {
            using (ChartService chartService = new ChartService())
            {
                var result = chartService.GetSectorStockValues(gameId, sectorId, turn);
                return Json(result);
            }
        }
    }
}
