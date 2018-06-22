using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X_SMS.AIHelper;
using X_SMS_REP;

namespace X_SMS_API.Controllers
{
    public class PlayerAIController : ApiController
    {
        [Route("api/PlayerAi/GetPlayerAIData")]
        [HttpPost]
        public ResultToken GetPlayerAIData([FromBody] GameDTO game)
        {
            ResultToken result = new ResultToken();
            result.Success = true;

            PlayerAI player = new PlayerAI(game);
            result.Data = player.returnBuySellList();
            return result;
        }
    }
}
