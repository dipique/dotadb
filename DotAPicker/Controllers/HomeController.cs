using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker;


namespace DotAPicker.Controllers
{
    [CustomAuthorize(Roles = "Authenticated")]
    public class HomeController : DotAController
    {
        // GET: Hero
        public ActionResult Index()
        {
            if (CurrentUser == null) return DependencyResolver.Current.GetService<LoginController>().Index();
            return View("DotAPicker", CurrentUser.Heroes.OrderBy(h => h.Name));
        }            

        public ActionResult Detail(int id) => PartialView(GetHeroByID(id));
    }
}
