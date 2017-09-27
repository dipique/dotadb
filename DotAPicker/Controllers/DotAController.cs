using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class DotAController : Controller
    {
        internal const string dataInd = "data";
        internal DotADB db
        {
            get
            {
                if ((DotADB)(Session[dataInd]) == null)
                    Session[dataInd] = DotADB.Load();

                return (DotADB)Session[dataInd];
            }
            set
            {
                Session[dataInd] = value;
            }
        }

        public IEnumerable<SelectListItem> GetHeroOptions(int selection = -1) =>
            db.Heroes.Select(h => new SelectListItem() {
                Text = h.Name,
                Value = h.ID.ToString(),
                Selected = selection == h.ID
            });
    }
}