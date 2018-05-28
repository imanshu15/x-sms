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
        public ActionResult Index()
        {
            return PartialView("_Join");
        }
        [Route("Join/Wait")]
        public ActionResult Wait()
        {
            return PartialView("_Wait");
        }
    }
}