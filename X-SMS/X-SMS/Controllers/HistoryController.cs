using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace X_SMS.Controllers
{
    public class HistoryController : Controller
    {
        /// <summary>
        /// The History
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}