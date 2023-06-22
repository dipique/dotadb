using System;
using System.Web.Mvc;

namespace DotAPicker.Controllers
{
    [RequiresAuth(Roles = "Authenticated")]
    public class PickerController : DotAController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
