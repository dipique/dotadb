using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;
using DotAPicker.ViewModels;

namespace DotAPicker.Controllers
{
    public class TipController : DotAController
    {
        // GET: Tip
        public ActionResult Index()
        {
            var tipVMs = db.Tips.Select(t => DownCast<Tip, TipViewModel>(t)).ToList();
            foreach(var tipVM in tipVMs)
            {
                var hero = db.Heroes.FirstOrDefault(h => h.ID == tipVM.HeroID);
                if (hero != null)
                {
                    tipVM.HeroName = hero.Name;
                    tipVM.HeroAltNames = hero.AltNames;
                }
            }

            return View("Tips", tipVMs);
        }


        // GET: Tip/Create
        public ActionResult Create()
        {
            var tvm = new TipViewModel() { Patch = db.CurrentPatch, HeroOptions = GetHeroOptions() };
            if (db.Tips.Count() > 0) tvm.ID = db.Tips.Max(h => h.ID) + 1;
            return View("Create", tvm);
        }

        // POST: Tip/Create
        [HttpPost]
        public ActionResult Create(TipViewModel model)
        {
            if (db.Tips.Any(h => h.ID == model.ID))
            {
                throw new Exception("Tip ID collision");
            }

            db.Tips.Add(UpCast<Tip, TipViewModel>(model));
            db.Save();
            return Index();
        }

        // GET: Tip/Edit/5
        public ActionResult Edit(int id)
        {
            var tip = db.Tips.FirstOrDefault(h => h.ID == id);
            if (tip == null)
            {
                throw new Exception("Tip not found.");
            }

            var tvm = DownCast<Tip, TipViewModel>(tip);
            tvm.HeroOptions = GetHeroOptions(id);

            return View("Edit", tvm);
        }

        // POST: Tip/Edit/5
        [HttpPost]
        public ActionResult Edit(TipViewModel model)
        {
            var tip = db.Tips.FirstOrDefault(h => h.ID == model.ID);
            if (tip == null)
            {
                throw new Exception("Tip not found.");
            }

            //Repace the Tip with the edited Tip
            db.Tips.Remove(tip);
            db.Tips.Add(UpCast<Tip, TipViewModel>(model)); //the upcast prevents all the extra stuff from being saved

            db.Save();
            db = DotADB.Load();
            return Index();
        }

        // GET: Tip/Delete/5
        public ActionResult Delete(int id)
        {
            var tip = db.Tips.FirstOrDefault(h => h.ID == id);
            if (tip == null)
            {
                throw new Exception("Tip not found.");
            }

            db.Tips.Remove(tip);

            db.Save();
            db = DotADB.Load();
            return Index();
        }
    }
}
