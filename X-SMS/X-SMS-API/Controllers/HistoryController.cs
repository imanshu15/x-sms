using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using X_SMS_DAL.Services;

namespace X_SMS_API.Controllers
{
    public class HistoryController : ApiController
    {
        [Route("api/History/GetGameList")]
        public IHttpActionResult GetGameList()
        {
            using (HistoryService historyService = new HistoryService())
            {
                var result = historyService.GetGameList();
                return Json(result);
            }
        }
    }
}