using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class HomeController : DotAController
    {
        // GET: Hero
        public ActionResult Index()
        {
            return View("DotAPicker", CurrentUser.Heroes.OrderBy(h => h.Name));
        }

        public ActionResult Detail(int id)
        {
            return PartialView(CurrentUser.Heroes.FirstOrDefault(h => h.ID == id));
        }
    }
}
