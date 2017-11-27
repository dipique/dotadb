using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;
using DotAPicker.Utilities;

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

            if (!returnToHeroList) return Index();

            TempData["SelectedHeroID"] = model.HeroSubjectId;
            return RedirectToAction("Index", "Hero", new { });
        }

        // GET: Relationship/Edit/5
        public ActionResult Edit(int id)
        {
            var relationship = CurrentUser.Relationships.FirstOrDefault(h => h.Id == id);
            if (relationship == null)
            {
                throw new Exception("Relationship not found.");
            }

            ViewBag.SubjectOptions = GetSubjectOptions(id.ToString());
            db.Entry(relationship).State = EntityState.Detached;
            return View("Edit", relationship);
        }

        // POST: Relationship/Edit/5
        [HttpPost]
        public ActionResult Edit(Relationship model)
        {
            //Update the edited Relationship
            db.Relationships.Attach(model);
            db.SaveChanges();

            return Index();
        }

        // GET: Relationship/Delete/5
        public ActionResult Delete(int id)
        {
            var relationship = CurrentUser.Relationships.FirstOrDefault(h => h.Id == id);
            if (relationship == null)
            {
                throw new Exception("Relationship not found.");
            }

            db.Relationships.Remove(relationship);

            db.SaveChanges();
            return Index();
        }
    }
}
