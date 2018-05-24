using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using X_SMS.Hubs;
using X_SMS.Services;
using X_SMS_REP;

namespace X_SMS.Controllers
{
    public class JoinController : Controller
    {
        private static IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAGame(string playerName,int noOfPlayers)
        {
            GameDTO game = new GameDTO();
            game.CreatedPlayer = playerName;
            game.PlayersCount = noOfPlayers;
            using (APIService client = new APIService()) {
                var result = client.MakePostRequest("api/Game", game);
                _hubContext.Clients.All.GameCreated(result);
                return Json(result);
            }           
        }
    }
}