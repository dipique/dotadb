using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class HomeController : DotAController
    {
        // GET: Hero
        public ActionResult Index()
        {
            ViewBag.SelectedHeroID = TempData["SelectedHeroID"];
            return View("DotAPicker", db.Heroes.OrderBy(h => h.Name));
        }

        // GET: Hero/Create
        public ActionResult Create()
        {
            var hero = new Hero();
            if (db.Heroes.Count() > 0) hero.ID = db.Heroes.Max(h => h.ID) + 1;
            return View("Create", hero);
        }

        // POST: Hero/Create
        [HttpPost]
        public ActionResult Create(Hero model)
        {
            if (db.Heroes.Any(h => h.Name == model.Name))
            {
                throw new Exception("This name is already in use");
            }

            if (db.Heroes.Any(h => h.ID == model.ID))
            {
                throw new Exception("Hero ID collision");
            }

            db.Heroes.Add(model);
            db.Save();
            return Index();
        }

        public ActionResult Detail(int id)
        {
            return PartialView(new HeroDetailViewModel(id, db));
        }

        // GET: Hero/Delete/5
        public ActionResult Delete(int id)
        {
            var hero = db.Heroes.FirstOrDefault(h => h.ID == id);
            if (hero == null)
            {
                throw new Exception("Hero not found.");
            }

            db.Heroes.Remove(hero);

            db.Save();
            db = DotADB.Load();
            return Index();
        }
    }
}
