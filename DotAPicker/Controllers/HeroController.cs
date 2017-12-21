using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            var hero = new Hero() { UserId = CurrentUser.Id };
            return View("Create", hero);
        }

        // POST: Hero/Create
        [HttpPost]
        public ActionResult Create(Hero model)
        {
            if (CurrentUser.Heroes.Any(h => h.Name == model.Name))
            {
                return RedirectToAction(nameof(Index)).Error("This name is already in use");
            }

            if (db.Heroes.Any(h => h.Id == model.Id))
            {
                return RedirectToAction(nameof(Index)).Error("Hero ID collision");
            }

            db.Heroes.Add(model);
            db.SaveChanges();
            return RedirectToAction(nameof(Index)).Success("Hero created.");
        }

        public ActionResult Detail(int id) => PartialView(GetHeroByID(id));

        // GET: Hero/Edit/5
        public ActionResult Edit(int id)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.Id == id);
            if (hero == null) return Index().Error("Hero not found.");

            ViewBag.LabelOptions = CurrentUser.Labels;

            //detatch entity so it doesn't cause an issue if saved
            db.Entry(hero).State = EntityState.Detached;
            return View("Edit", hero);
        }

        // POST: Hero/Edit/5
        [HttpPost]
        public ActionResult Edit(Hero model)
        {
            if (!ModelState.IsValid) return Index().Error($"Error: {ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage}");

            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            //If there are new labels, save them as well.
            var newLabels = model.DescriptionLabels.Where(l => !CurrentUser.Labels.Contains(l));
            if (newLabels.Count() > 0)
            {
                CurrentUser.Labels.AddRange(newLabels);

                db.SaveChanges();
            }

            return RedirectToAction(nameof(Index)).Success("Edit complete");
        }

        // GET: Hero/Delete/5
        public ActionResult Delete(int id)
        {
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.Id == id);
            if (hero == null)
            {
                return Index().Error("Hero not found.");
            }

            db.Heroes.Remove(hero);
            db.SaveChanges();

            return Index().Success("Hero deleted.");
        }
    }
}
