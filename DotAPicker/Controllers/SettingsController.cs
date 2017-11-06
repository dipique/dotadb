using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class SettingsController : DotAController
    {
        // GET: Settings
        public ActionResult Index()
        {
            return View("Edit", db.Settings);
        }

        //TODO: complete revamp
    }
}