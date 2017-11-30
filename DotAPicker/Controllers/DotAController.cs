using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.DAL;
using DotAPicker.Models;
using DotAPicker.Utilities;

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
            CurrentUser.Heroes.Select(h => new SelectListItem()
            {
                Text = h.Name,
                Value = h.Id.ToString(),
                Selected = selection == h.Id
            }).OrderBy(s => s.Text);

        public IEnumerable<SelectListItem> GetSubjectOptions(string selection = null)
        {
            int intSelection = -1;
            int.TryParse(selection, out intSelection);
            return CurrentUser.Labels.Select(l => new SelectListItem()
            {
                Text = $"Label: {l}",
                Value = l,
                Selected = selection == l
            }).OrderBy(s => s.Text)
              .Concat(GetHeroOptions(intSelection));
        }

        public Hero GetHeroByID(int id)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.Id == id) ?? CurrentUser.Heroes.First();
            if (hero == null) return hero;
            hero.Relationships = db.Relationships.Where(r => r.HeroObjectId == id ||
                                                             r.HeroSubjectId == id ||
                                                             hero.Labels.Contains(r.LabelSubject) ||
                                                             hero.Labels.Contains(r.LabelObject))
                                                 .ToList();
            hero.Tips = db.Tips.Where(r => r.HeroSubjectId == id ||
                                           hero.Labels.Contains(r.LabelSubject))
                               .ToList();
            return hero;
        }

        public ActionResult UpdatePreference(int heroID, string preference)
        {
            var pref = EnumExt.Parse<HeroPreference>(preference);
            var hero = GetHeroByID(heroID);
            var oldPref = hero.Preference;
            if (oldPref != pref)
            {
                hero.Preference = pref;
                db.Entry(hero).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("Popdown").Success("Preference updated.");
            }
            return View();
        }
    }

    internal static class PopdownExtensions
    {
        public static ActionResult Error(this ActionResult result, string message)
        {
            PopDownCookieMessage(Notification.Danger, message);
            return result;
        }

        public static ActionResult Warning(this ActionResult result, string message)
        {
            PopDownCookieMessage(Notification.Warning, message);
            return result;
        }

        public static ActionResult Success(this ActionResult result, string message)
        {
            PopDownCookieMessage(Notification.Success, message);
            return result;
        }

        public static ActionResult Information(this ActionResult result, string message)
        {
            PopDownCookieMessage(Notification.Info, message);
            return result;
        }

        private static void PopDownCookieMessage(Notification notification, string message)
        {
            //Add the new message
            HttpContext.Current.Response.Cookies.Add(
                new HttpCookie($"{notification.ToString().ToLower()}", message) {
                    Path = "/",
                    Expires = DateTime.Now.AddSeconds(5) //I'm not really sure how long this should be--just long enough for the value to be there 
                });                                      //on page load. But if it's too long, it'll be there when the next action takes place.
        }
    }

    public enum Notification
    {
        Danger,
        Warning,
        Success,
        Info
    }
}