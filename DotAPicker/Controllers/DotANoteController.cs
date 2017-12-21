using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using DotAPicker.DAL;
using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public abstract class DotANoteContoller<T> : DotAController where T:DotANote, new()
    {
        // GET: Tip
        public ActionResult Index() => View("Index", GetDbSet.Where(n => n.UserId == CurrentUser.Id).ToList());

        public DbSet<T> GetDbSet => (DbSet<T>)typeof(DotAContext).GetProperties()
                                                                 .Single(p => p.PropertyType.IsGenericType &&
                                                                              p.PropertyType.GenericTypeArguments[0] == typeof(T))
                                                                 .GetValue(db);
        public string TypeString => typeof(T).Name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="heroID"></param>
        /// <param name="requestOrigin"></param>
        /// <returns></returns>
        public ActionResult Create(int heroID = -1, bool returnToHeroList = false)
        {
            ViewBag.ReturnToHeroList = returnToHeroList;
            ViewBag.SubjectOptions = GetSubjectOptions(heroID.ToString());
            var note = new T() { Patch = CurrentUser.CurrentPatch, UserId = CurrentUser.Id };
            if (heroID != -1) note.HeroSubjectId = heroID;
            return View("Create", note);
        }

        [HttpPost]
        public ActionResult Create(T model, bool returnToHeroList = false)
        {
            GetDbSet.Add(model);
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Create(model.HeroSubjectId ?? -1, returnToHeroList).Error("You're not allowed to do that.");
            }                

            if (!returnToHeroList || model.HeroSubjectId == null) return Index().Success($"{TypeString} created.");

            TempData["SelectedHeroID"] = model.HeroSubjectId;
            return RedirectToAction("Index", "Hero", new { }).Success($"{TypeString} created.");
        }

        public ActionResult Edit(int id)
        {
            var note = GetDbSet.FirstOrDefault(h => h.Id == id);
            if (note == null) return Index().Error($"{TypeString} not found.");

            ViewBag.SubjectOptions = GetSubjectOptions(note.HeroSubjectId.ToString());
            db.Entry(note).State = EntityState.Detached;

            return View("Edit", note);
        }

        [HttpPost]
        public ActionResult Edit(T model, bool returnToHeroList = false)
        {
            if (!ModelState.IsValid) return Index().Error($"Error: {ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage}");
            if (model == null) return Index().Error("Some'n goofy happened, try again. Or, you know, don't.");

            GetDbSet.Attach(model);
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }

            if (!returnToHeroList || model.HeroSubjectId == null) return Index().Success("Changes saved.");

            TempData["SelectedHeroID"] = model.HeroSubjectId;
            return RedirectToAction("Index", "Hero", new { }).Success("Changes saved.");
        }

        // GET: Tip/Delete/5
        public ActionResult Delete(int id, bool returnToHeroList = false)
        {
            var note = GetDbSet.FirstOrDefault(h => h.Id == id);
            if (note == null) return Index().Error("Tip not found.");

            var heroID = note.HeroSubjectId; //save it before it's deleted

            GetDbSet.Remove(note);
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }

            if (!returnToHeroList || heroID == null) return Index().Success($"{TypeString} deleted.");

            TempData["SelectedHeroID"] = heroID;
            return RedirectToAction("Index", "Hero", new { }).Success($"{TypeString} deleted.");
        }
    }
}
