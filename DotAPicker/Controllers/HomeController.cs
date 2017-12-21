using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker;


namespace DotAPicker.Controllers
{
    [RequiresAuth(Roles = "Authenticated")]
    public class HomeController : DotAController
    {
        // GET: Hero
        public ActionResult Index() => View("DotAPicker", CurrentUser.Heroes.OrderBy(h => h.Name));

        public ActionResult Detail(int id) => PartialView(GetHeroByID(id));
    }
}
