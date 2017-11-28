using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class RelationshipController : DotAController
    {
        // GET: Relationship
        public ActionResult Index()
        {
            var relationshipVMs = db.Relationships.Include(r => r.HeroSubject)
                                                  .Include(r => r.HeroObject)
                                                  .Where(r => r.UserId == CurrentUser.Id);
            var test = CurrentUser.Id;
            return View("Relationships", relationshipVMs.ToList());
        }


        // GET: Relationship/Create
        public ActionResult Create(int heroID = -1, bool returnToHeroList = false)
        {
            ViewBag.ReturnToHeroList = returnToHeroList;
            ViewBag.SubjectOptions = GetSubjectOptions(heroID.ToString());

            var tvm = new Relationship() { Patch = CurrentUser.CurrentPatch, UserId = CurrentUser.Id };
            if (heroID != -1) tvm.HeroSubjectId = heroID;
            return View("Create", tvm);
        }

        // POST: Relationship/Create
        [HttpPost]
        public ActionResult Create(Relationship model, bool returnToHeroList = false)
        {
            db.Relationships.Add(model);
            db.SaveChanges();
            var heroID = model.HeroSubjectId ?? model.HeroObjectId;

            if (!returnToHeroList || heroID == null) return Index().Success("Relationship created.");

            TempData["SelectedHeroID"] = heroID;
            return RedirectToAction("Index", "Hero", new { }).Success("Relationship created.");
        }

        // GET: Relationship/Edit/5
        public ActionResult Edit(int id)
        {
            var relationship = CurrentUser.Relationships.FirstOrDefault(h => h.Id == id);
            if (relationship == null) return Index().Error("Relationship not found.");

            ViewBag.SubjectOptions = GetSubjectOptions(id.ToString());
            db.Entry(relationship).State = EntityState.Detached;
            return View("Edit", relationship);
        }

        // POST: Relationship/Edit/5
        [HttpPost]
        public ActionResult Edit(Relationship model, bool returnToHeroList = false)
        {
            if (!ModelState.IsValid) return Index().Error($"Error: {ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage}");
            if (model == null) return Index().Error("Some'n goofy happened, try again. Or, you know, don't.");

            db.Relationships.Attach(model);
            db.SaveChanges();
            var heroID = model.HeroSubjectId ?? model.HeroObjectId;
            if (!returnToHeroList || heroID == null) return Index().Success("Changes saved.");

            TempData["SelectedHeroID"] = heroID;
            return RedirectToAction("Index", "Hero", new { }).Success("Changes saved.");
        }

        // GET: Relationship/Delete/5
        public ActionResult Delete(int id, bool returnToHeroList = false)
        {
            var relationship = CurrentUser.Relationships.FirstOrDefault(h => h.Id == id);
            if (relationship == null)
            {
                return Index().Error("Relationship not found.");
            }
            var heroID = relationship.HeroSubjectId ?? relationship.HeroObjectId; //save it before it's deleted
            db.Relationships.Remove(relationship);

            db.SaveChanges(); 
            if (!returnToHeroList || heroID == null) return Index().Success("Relationship deleted.");

            TempData["SelectedHeroID"] = heroID;
            return RedirectToAction("Index", "Hero", new { }).Success("Relationship deleted.");
        }
    }
}
