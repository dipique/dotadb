using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class UserController : DotAController
    {
        // GET: User
        public ActionResult Index() //=> View(db.Users.ToList()); //prevent people from seeing the full list
        {
            return View(db.Users.Where(u => u.Id == CurrentUser.Id).ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return Index().Error("Invalid User ID");
            if (id != CurrentUser.Id) return Index().Error("Sorry man that ain't yours and I think you probably knew that.");
            User user = db.Users.Find(id);
            if (user == null) Index().Error("Invalid User Id");

            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return RedirectToAction("Login", "Register");
            //return View(new User());
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,CurrentPatch,ShowDeprecatedTips,ShowDeprecatedRelationships,Labels")] User user)
        {
            return RedirectToAction("Login", "Register").Error("We don't create users that way anymore buster.");
            //if (!IsUniqueUserName(user, db.Users)) return View(user).Error("Username must be unique.");

            //if (ModelState.IsValid)
            //{
            //    db.Users.Add(user);
            //    if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            //    {
            //        return Index().Error("You're not allowed to that.");
            //    }
            //    return RedirectToAction("Index");
            //}

            //return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null && CurrentUser != null) id = CurrentUser.Id;
            if (id == null) return Index().Error("You'll need to log in first, sry.");
            if (!CurrentUser.IsAuthenticated) return Index().Error("You'll need to log in first, sry.");

            User user = db.Users.Find(id);
            if (user == null)
            {
                if (id == null) return Index().Error("That user ID wasn't found.");
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,CurrentPatch,ShowDeprecatedTips,ShowDeprecatedRelationships,Labels")] User user)
        {
            if (CurrentUser.Id == user.Id) db.Entry(CurrentUser).State = EntityState.Detached;

            //check for errors
            if (!ModelState.IsValid) return View(user).Error("Invalid entries.");
            if (!IsUniqueUserName(user, db.Users)) return View(user).Error("Username must be unique.");

            //make the update
            db.Entry(user).State = EntityState.Modified;
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }

            return RedirectToAction("Index").Success("Updated!");
        }

        public static bool IsUniqueUserName(User user, IEnumerable<User> users) => users.Any(u => (user.Id <= 0 || user.Id != u.Id) 
                                                                                               && (u.Email == user.Name || u.Name == user.Email));

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return Index().Error("You'll need to log in first, sry.");
            if (!CurrentUser.IsAuthenticated) return Index().Error("You'll need to log in first, sry.");

            User user = db.Users.Find(id);
            if (user == null) return Index().Error("User not found.");

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }
            return RedirectToAction("Index");
        }
    }
}
