using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;
using DotAPicker.Utilities;

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
            ViewBag.HeroOptions = GetHeroOptions(heroID);
            var tip = new Tip() { Patch = CurrentUser.CurrentPatch };
            if (heroID != -1) tip.HeroID = heroID;
            return View("Create", tip);
        }

        // POST: Tip/Create
        [HttpPost]
        public ActionResult Create(Tip model, bool returnToHeroList = false)
        {
            if (db.Tips.Any(h => h.ID == model.ID))
            {
                throw new Exception("Tip ID collision");
            }

            db.Tips.Attach(model);
            db.SaveChanges();

            if (!returnToHeroList) return Index();

            TempData["SelectedHeroID"] = model.HeroID;
            return RedirectToAction("Index", "Hero", new { });
        }

        // GET: Tip/Edit/5
        public ActionResult Edit(int id)
        {
            var tip = CurrentUser.Tips.FirstOrDefault(h => h.ID == id);
            if (tip == null)
            {
                throw new Exception("Tip not found.");
            }
            ViewBag.HeroOptions = GetHeroOptions(tip.HeroID);

            return View("Edit", tip);
        }

        // POST: Tip/Edit/5
        [HttpPost]
        public ActionResult Edit(Tip model)
        {
            db.Tips.Attach(model);

            db.SaveChanges();
            return Index();
        }

        // GET: Tip/Delete/5
        public ActionResult Delete(int id)
        {
            var tip = CurrentUser.Tips.FirstOrDefault(h => h.ID == id);
            if (tip == null)
            {
                throw new Exception("Tip not found.");
            }

            db.Tips.Remove(tip);

            db.SaveChanges();
            return Index();
        }
    }
}
