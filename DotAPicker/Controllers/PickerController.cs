using Newtonsoft.Json;
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

        public ContentResult Heroes()
        {
            var payload = CurrentUser.Heroes;
            var json = JsonConvert.SerializeObject(payload);
            return Content(json, "application/json");
        }
    }
}
