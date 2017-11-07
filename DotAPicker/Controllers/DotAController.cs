using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using DotAPicker.DAL;
using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class DotAController : Controller
    {
        internal const string dataInd = "data";
        internal const string userInd = "user";
        internal DotAContext db
        {
            get
            {
                var tmpDB = (DotAContext)Session[dataInd];
                if (tmpDB == null)
                    Session[dataInd] = tmpDB = new DotAContext();

                return (DotAContext)Session[dataInd];
            }
            set
            {
                Session[dataInd] = value;
            }
        }

        internal User CurrentUser
        {
            get
            {
                //set default user if none is set
                if (Session[userInd] == null)
                {
                    var user = db.Users.First(); //TODO: be able to change user
                    Session[userInd] = user;
                }
                return (User)Session[userInd];
            }
            set => Session[userInd] = value;
        }

        public IEnumerable<SelectListItem> GetHeroOptions(int selection = -1) =>
            CurrentUser.Heroes.Select(h => new SelectListItem() {
                Text = h.Name,
                Value = h.ID.ToString(),
                Selected = selection == h.ID
            }).OrderBy(s => s.Text);

    }
}