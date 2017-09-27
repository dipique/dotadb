using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.ViewModels;
using DotAPicker.Models;
using DotAPicker.Utilities;

namespace DotAPicker.Controllers
{
    public class RelationshipController : DotAController
    {
        // GET: Relationship
        public ActionResult Index()
        {
            var relationshipVMs = db.Relationships.Select(t => Casting.DownCast<Relationship, RelationshipViewModel>(t)).ToList();
            foreach (var relationshipVM in relationshipVMs)
            {
                relationshipVM.Hero1 = db.Heroes.FirstOrDefault(h => h.ID == relationshipVM.Hero1ID);
                relationshipVM.Hero2 = db.Heroes.FirstOrDefault(h => h.ID == relationshipVM.Hero2ID);
            }

            return View("Relationships", relationshipVMs);
        }


        // GET: Relationship/Create
        public ActionResult Create()
        {
            var tvm = new RelationshipViewModel() { Patch = db.CurrentPatch, HeroOptions = GetHeroOptions() };
            if (db.Relationships.Count() > 0) tvm.ID = db.Relationships.Max(h => h.ID) + 1;
            return View("Create", tvm);
        }

        // POST: Relationship/Create
        [HttpPost]
        public ActionResult Create(RelationshipViewModel model)
        {
            if (db.Relationships.Any(h => h.ID == model.ID))
            {
                throw new Exception("Relationship ID collision");
            }

            db.Relationships.Add(Casting.UpCast<Relationship, RelationshipViewModel>(model));
            db.Save();
            return Index();
        }

        // GET: Relationship/Edit/5
        public ActionResult Edit(int id)
        {
            var relationship = db.Relationships.FirstOrDefault(h => h.ID == id);
            if (relationship == null)
            {
                throw new Exception("Relationship not found.");
            }

            var tvm = Casting.DownCast<Relationship, RelationshipViewModel>(relationship);
            tvm.HeroOptions = GetHeroOptions(id);

            return View("Edit", tvm);
        }

        // POST: Relationship/Edit/5
        [HttpPost]
        public ActionResult Edit(RelationshipViewModel model)
        {
            var relationship = db.Relationships.FirstOrDefault(h => h.ID == model.ID);
            if (relationship == null)
            {
                throw new Exception("Relationship not found.");
            }

            //Repace the Relationship with the edited Relationship
            db.Relationships.Remove(relationship);
            db.Relationships.Add(Casting.UpCast<Relationship, RelationshipViewModel>(model)); //the upcast prevents all the extra stuff from being saved

            db.Save();
            db = DotADB.Load();
            return Index();
        }

        // GET: Relationship/Delete/5
        public ActionResult Delete(int id)
        {
            var relationship = db.Relationships.FirstOrDefault(h => h.ID == id);
            if (relationship == null)
            {
                throw new Exception("Relationship not found.");
            }

            db.Relationships.Remove(relationship);

            db.Save();
            db = DotADB.Load();
            return Index();
        }
    }
}
