using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            CurrentUser.Heroes.Select(h => new SelectListItem() {
                Text = h.Name,
                Value = h.ID.ToString(),
                Selected = selection == h.ID
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

        public Hero GetHeroByID(int id)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.ID == id);
            hero.Relationships = db.Relationships.Where(r => r.HeroObjectID == id || 
                                                             r.HeroSubjectID == id || 
                                                             hero.Labels.Contains(r.LabelSubject) || 
                                                             hero.Labels.Contains(r.LabelObject))
                                                  .ToList();
            hero.Tips = db.Tips.Where(r => r.HeroSubjectID == id ||
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
            }
            return View();
        }

    }
}