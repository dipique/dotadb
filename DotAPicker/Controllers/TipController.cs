using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class TipController : DotAController
    {
        // GET: Tip
        public ActionResult Index()
        {
            return View("Tips", CurrentUser.Tips);
        }

        // GET: Tip/Create
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
            var tip = new Tip() { Patch = CurrentUser.CurrentPatch, UserId = CurrentUser.Id };
            if (heroID != -1) tip.HeroSubjectId = heroID;
            return View("Create", tip);
        }

        // POST: Tip/Create
        [HttpPost]
        public ActionResult Create(Tip model, bool returnToHeroList = false)
        {
            db.Tips.Add(model);
            db.SaveChanges();

            if (!returnToHeroList || model.HeroSubjectId == null) return Index().Success("Tip created.");

            TempData["SelectedHeroID"] = model.HeroSubjectId;
            return RedirectToAction("Index", "Hero", new { }).Success("Tip created.");
        }

        // GET: Tip/Edit/5
        public ActionResult Edit(int id)
        {
            var tip = CurrentUser.Tips.FirstOrDefault(h => h.Id == id);
            if (tip == null) return Index().Error("Tip not found.");

            ViewBag.SubjectOptions = GetSubjectOptions(tip.HeroSubjectId.ToString());
            db.Entry(tip).State = EntityState.Detached;

            return View("Edit", tip);
        }

        // POST: Tip/Edit/5
        [HttpPost]
        public ActionResult Edit(Tip model, bool returnToHeroList = false)
        {
            if (!ModelState.IsValid) return Index().Error($"Error: {ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage}");
            if (model == null) return Index().Error("Some'n goofy happened, try again. Or, you know, don't.");

            db.Tips.Attach(model);
            db.SaveChanges();

            if (!returnToHeroList || model.HeroSubjectId == null) return Index().Success("Changes saved.");

            TempData["SelectedHeroID"] = model.HeroSubjectId;
            return RedirectToAction("Index", "Hero", new { }).Success("Changes saved.");
        }

        // GET: Tip/Delete/5
        public ActionResult Delete(int id, bool returnToHeroList = false)
        {
            var tip = CurrentUser.Tips.FirstOrDefault(h => h.Id == id);
            if (tip == null) return Index().Error("Tip not found.");

            var heroID = tip.HeroSubjectId; //save it before it's deleted

            db.Tips.Remove(tip);
            db.SaveChanges();

            if (!returnToHeroList || heroID == null) return Index().Success("Tip deleted.");

            TempData["SelectedHeroID"] = heroID;
            return RedirectToAction("Index", "Hero", new { }).Success("Tip deleted.");
        }
    }
}
