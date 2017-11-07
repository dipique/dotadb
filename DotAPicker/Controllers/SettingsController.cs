//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Web;
//using System.Web.Mvc;

//using DotAPicker.Models;
//using DotAPicker.Utilities;

//namespace DotAPicker.Controllers
//{
//    public class SettingsController : DotAController
//    {
//        // GET: Settings
//        public ActionResult Index()
//        {
//            return View("Edit", CurrentUser.Settings);
//        }

//        public ActionResult Save(List<Setting> settings)
//        {
//            db.UpdateSettings(CurrentUser.ID, settings);
//            return Index();
//        }
//    }


//}