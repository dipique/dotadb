using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

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

        /// <summary>
        /// Takes an object and converts it to its parent type, stripping off any unique information
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public TChild DownCast<TParent, TChild>(TParent parentObj)
        {
            string output = JsonConvert.SerializeObject(parentObj);
            return JsonConvert.DeserializeObject<TChild>(output);
        }

        public TParent UpCast<TParent, TChild>(TChild childObj)
        {
            string output = JsonConvert.SerializeObject(childObj);
            return JsonConvert.DeserializeObject<TParent>(output);
        }
    }
}