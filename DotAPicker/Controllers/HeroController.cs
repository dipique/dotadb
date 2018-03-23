using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using DotAPicker.Models;
using DotAPicker.ViewModels;

namespace DotAPicker.Controllers
{
    public class HeroController : DotAController
    {
        // GET: Hero
        public ActionResult Index()
        {
            ViewBag.SelectedHeroID = TempData["SelectedHeroID"];
            TempData["SelectedHeroID"] = null;
            return GetItems(nameof(Hero.Name), SortDirections.Ascending, null);
        }

        public ActionResult GetItems(string sortField, SortDirections sortDirection, int? selectedHeroID = null)
        {
            if (selectedHeroID != null)
            {
                ViewBag.SelectedHeroID = selectedHeroID;
            }
            return View("Heroes", new TableViewModel<Hero>() {
                Items = CurrentUser.Heroes,
                SortDirection = sortDirection,
                SortField = sortField
            });
        }

        public ActionResult HeroSort(string propertyName, string currentSortString)
        {
            if (!Enum.TryParse(currentSortString, out SortDirections currentSortDirection))
            {
                return Index();
            }

            SortDirections newSortDirection = SortDirections.Ascending;
            if (currentSortDirection == SortDirections.Ascending)
                newSortDirection = SortDirections.Descending;

            return GetItems(propertyName, newSortDirection, null);
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
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return RedirectToAction(nameof(Index)).Error("You're not allowed to that.");
            }
            return RedirectToAction(nameof(Index)).Success("Hero created.");
        }

        public ActionResult Detail(int id) => PartialView(new HeroViewModel(GetHeroByID(id), CurrentUser.IsAuthenticated));

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
            ViewBag.SelectedHeroID = model.Id;
            if (!ModelState.IsValid) return Index().Error($"Error: {ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage}");

            db.Entry(model).State = EntityState.Modified;
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }

            //If there are new labels, save them as well.
            var newLabels = model.DescriptionLabels.Where(l => !CurrentUser.Labels.Contains(l));
            if (newLabels.Count() > 0)
            {
                CurrentUser.Labels.AddRange(newLabels);

                if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
                {
                    return Index().Error("You're not allowed to that.");
                }
            }

            TempData["SelectedHeroID"] = model.Id;
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

            //check if there are any tips or relationships that depend on this hero
            if (CurrentUser.Tips.Any(t => t.HeroSubjectId == id) ||
                CurrentUser.Relationships.Any(r => r.HeroSubjectId == id || r.HeroObjectId == id))
            {
                return Index().Error("You can't delete a hero that has associated tips or relationships.");
            }

            db.Heroes.Remove(hero);
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }

            return Index().Success("Hero deleted.");
        }

        [HttpGet]
        public ActionResult CreateTip(int heroID) => UpdateTip(heroID, 0);

        [HttpGet]
        public ActionResult UpdateTip(int heroID, int id)
        {
            var heroOption = heroID < 1 ? null : heroID.ToString();
            ViewBag.SubjectOptions = GetSubjectOptions(heroOption);

            //if it's a new tip, return the partial view
            if (id < 1)
            {
                var newTip = new Tip() {
                    Patch = CurrentUser.CurrentPatch,
                    UserId = CurrentUser.Id
                };
                if (heroID > 0 && db.Heroes.Any(h => h.Id == heroID)) newTip.HeroSubjectId = heroID;
                return PartialView("TipDialog", newTip);
            }

            //if it's getting update, get the tip
            Tip tip = db.Tips.FirstOrDefault(t => t.Id == id);
            if (tip == null) return RedirectToAction(nameof(Index)).Error("Umm. I couldn't find that tip... and I'm not sure how that's possible.");
            return PartialView("TipDialog", tip);
        }

        [HttpPost]
        public ActionResult UpdateTip(Tip model)
        {
            if (model == null) return Index();
            var addTip = model.Id < 1;

            if (addTip)
                db.Tips.Add(model);
            else
                db.Tips.Attach(model);

            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
                return RedirectToAction(nameof(Index)).Error("You're not allowed to do that.");

            return RedirectToAction(nameof(Index)).Success($"Tip {(addTip ? "added" : "updated")}.");
        }

        [HttpGet]
        public ActionResult CreateRelationship(int heroID) => UpdateRelationship(heroID, 0);

        [HttpGet]
        public ActionResult UpdateRelationship(int heroID, int id)
        {
            var heroOption = heroID < 1 ? null : heroID.ToString();
            ViewBag.SubjectOptions = GetSubjectOptions(heroOption);

            //if it's a new tip, return the partial view
            if (id < 1)
            {
                var newRelationship = new Relationship()
                {
                    Patch = CurrentUser.CurrentPatch,
                    UserId = CurrentUser.Id
                };
                if (heroID > 0 && db.Heroes.Any(h => h.Id == heroID)) newRelationship.HeroSubjectId = heroID;
                return PartialView("RelationshipDialog", newRelationship);
            }

            //if it's getting updated, get the relationship
            Relationship tip = db.Relationships.FirstOrDefault(t => t.Id == id);
            if (tip == null) return RedirectToAction(nameof(Index)).Error("Umm. I couldn't find that relationship... and I'm not sure how that's possible.");
            return PartialView("RelationshipDialog", tip);
        }

        [HttpPost]
        public ActionResult UpdateRelationship(Relationship model)
        {
            if (model == null) return Index();
            var addRelationship = model.Id < 1;

            if (addRelationship)
                db.Relationships.Add(model);
            else
                db.Relationships.Attach(model);

            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
                return RedirectToAction(nameof(Index)).Error("You're not allowed to do that.");

            return RedirectToAction(nameof(Index)).Success($"Relationship {(addRelationship ? "added" : "updated")}.");
        }
    }
}
