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
        public const string SESSION_IND_DATA = "data";
        public const string SESSION_IND_USR = "user";
        internal DotAContext db
        {
            get
            {
                if (Session[SESSION_IND_DATA] == null)
                    Session[SESSION_IND_DATA] = new DotAContext();

                return (DotAContext)Session[SESSION_IND_DATA];
            }
            set
            {
                Session[SESSION_IND_DATA] = value;
            }
        }

        internal User CurrentUser
        {
            get
            {
                var user = (User)Session[SESSION_IND_USR];
                if (user.Unloaded)
                {
                    user = db.Users.FirstOrDefault(u => u.Name == user.Name);
                    user.IsAuthenticated = false;
                    Session[SESSION_IND_USR] = user;
                }
                return (User)Session[SESSION_IND_USR];
            }
            set => Session[SESSION_IND_USR] = value;
        }

        public void SetCurrentUser(User user)
        {
            HttpContext.User = new Principal(user);
            CurrentUser = user;
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
            return CurrentUser.Labels.Select(l => new SelectListItem() {
                Text = $"Label: {l}",
                Value = l,
                Selected = selection == l
            }).OrderBy(s => s.Text)
              .Concat(GetHeroOptions(intSelection));
        }

        public Hero GetHeroByID(int heroID)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.Id == heroID);
            if (hero == null) return new Hero();
            var heroName = hero.Name;

            //get synced profile IDs and whether they will allow editing
            var profiles = db.Users.Where(u => CurrentUser.SyncedProfiles.Contains(u.Name) || u.Name == CurrentUser.Name)
                                   .ToDictionary(u => u.Id, u => u.ProfileType == ProfileTypes.Public 
                                                              || (CurrentUser.IsAuthenticated && u.Id == CurrentUser.Id));
            var keys = profiles.Select(p => p.Key);

            hero.Relationships = db.Relationships.Where(r => keys.Contains(r.UserId))
                                                 .Where(r => r.HeroObject.Name == heroName ||
                                                             r.HeroSubject.Name == heroName ||
                                                             hero.Labels.Contains(r.LabelSubject) ||
                                                             hero.Labels.Contains(r.LabelObject)).ToList()   //bring into memory
                                                 .Select(r => { r.Editable = profiles[r.UserId];
                                                                if (r.UserId != CurrentUser.Id) {
                                                                    r.ProfileName = db.Users.Find(r.UserId).Name;
                                                                }                                                     
                                                                return r;
                                                 }).ToList();

            hero.Tips = db.Tips.Where(t => keys.Contains(t.UserId))
                               .Where(t => t.HeroSubject.Name == heroName ||
                                           hero.Labels.Contains(t.LabelSubject)).ToList()
                               .Select(t => { t.Editable = profiles[t.UserId];
                                              if (t.UserId != CurrentUser.Id) {
                                                  t.ProfileName = db.Users.Find(t.UserId).Name;
                                              }
                                              return t;
                               }).ToList();
            return hero;
        }

        public ActionResult UpdateHeroImage(int heroID) => RedirectToAction("UpdateHeroImage", "Settings", new { heroID });

        public ActionResult UpdatePreference(int heroID, string preference)
        {
            var pref = EnumExt.Parse<HeroPreference>(preference);
            var hero = GetHeroByID(heroID);

            if (hero == null) return PartialView("Popdown").Error("You do not have permission to modify this data.");

            var oldPref = hero.Preference;
            if (oldPref != pref)
            {
                hero.Preference = pref;
                db.Entry(hero).State = EntityState.Modified;
                if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
                {
                    return PartialView("Popdown").Error("You're not allowed to update that.");
                }
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
                    Expires = DateTime.Now.AddSeconds(1) //I'm not really sure how long this should be--just long enough for the value to be there 
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