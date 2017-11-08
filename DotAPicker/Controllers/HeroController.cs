﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class HeroController : DotAController
    {
        // GET: Hero
        public ActionResult Index()
        {
            ViewBag.SelectedHeroID = TempData["SelectedHeroID"];
            return View("Heroes", CurrentUser.Heroes.OrderBy(h => h.Name));
        }

        // GET: Hero/Create
        public ActionResult Create()
        {
            var hero = new Hero() { UserID = CurrentUser.ID };
            return View("Create", hero);
        }

        // POST: Hero/Create
        [HttpPost]
        public ActionResult Create(Hero model)
        {
            if (CurrentUser.Heroes.Any(h => h.Name == model.Name))
            {
                throw new Exception("This name is already in use");
            }

            if (db.Heroes.Any(h => h.ID == model.ID))
            {
                throw new Exception("Hero ID collision");
            }

            db.Heroes.Add(model);
            db.SaveChanges();
            return Index();
        }

        public ActionResult Detail(int id)
        {
            return PartialView(CurrentUser.Heroes.FirstOrDefault(h => h.ID == id));
        }

        // GET: Hero/Edit/5
        public ActionResult Edit(int id)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.ID == id);
            if (hero == null)
            {
                throw new Exception("Hero not found.");
            }

            ///test
            //if (hero.Counters.Count() == 0) hero.Counters.Add("disables");

            ViewBag.LabelOptions = new LabelSet(CurrentUser.LabelOptions);
            return View("Edit", hero);
        }

        // POST: Hero/Edit/5
        [HttpPost]
        public ActionResult Edit(Hero model)
        {
            //Update the edited hero
            db.Heroes.Attach(model);
            db.SaveChanges();

            return Index();
        }

        // GET: Hero/Delete/5
        public ActionResult Delete(int id)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.ID == id);
            if (hero == null)
            {
                throw new Exception("Hero not found.");
            }

            db.Heroes.Remove(hero);
            db.SaveChanges();

            return Index();
        }
    }
}
