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

        public ActionResult Save(DotASettings model)
        {
            //Repace the Tip with the edited Tip
            db.Settings = model;
            db.Save();
            db = DotADB.Load();
            return Index();
        }
    }
}