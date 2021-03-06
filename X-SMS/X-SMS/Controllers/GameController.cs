﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace X_SMS.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GameBoard()
        {
            return PartialView("_GameBoard");
        }

        public ActionResult Summary()
        {
            return View("Winner");
        }
    }
}