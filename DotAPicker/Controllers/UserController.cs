using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using DotAPicker.DAL;
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
            if (id != CurrentUser.Id) return RedirectToAction(nameof(Index)).Error("Sorry man that ain't yours and I think you probably knew that.");
            User user = db.Users.Find(id);
            if (user == null) return RedirectToAction(nameof(Index)).Error("Invalid User Id");

            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return RedirectToAction("Register", "Login");
            //return View(new User());
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
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
            if (!CurrentUser.IsAuthenticated) return RedirectToAction(nameof(Index)).Error("You'll need to log in first, sry.");

            User user = db.Users.Find(id);
            if (user == null)
            {
                if (id == null) return RedirectToAction(nameof(Index)).Error("That user ID wasn't found.");
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            //check for errors
            if (!ModelState.IsValid) return View(user).Error("Invalid entries.");
            if (!IsUniqueUserName(user, db.Users)) return View(user).Error("Username must be unique.");
            if (user.Id != CurrentUser.Id) return View(user).Error("How did you even get here?");

            //set the parameters
            CurrentUser.Name = user.Name;
            CurrentUser.ShowDeprecatedRelationships = user.ShowDeprecatedRelationships;
            CurrentUser.ShowDeprecatedTips = user.ShowDeprecatedTips;
            CurrentUser.CurrentPatch = user.CurrentPatch;
            CurrentUser.ProfileType = user.ProfileType;
            db.Entry(CurrentUser).State = EntityState.Modified;

            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return Index().Error("You're not allowed to that.");
            }

            return RedirectToAction("Index", "Hero").Success("Updated!");
        }

        public static bool IsUniqueUserName(User user, IEnumerable<User> users) => !users.Any(u => (user.Id <= 0 || user.Id != u.Id) 
                                                                                                && (u.Email == user.Name || u.Name == user.Email));

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index)).Error("You'll need to log in first, sry.");
            if (!CurrentUser.IsAuthenticated) return RedirectToAction(nameof(Index)).Error("You'll need to log in first, sry.");

            User user = db.Users.Find(id);
            if (user == null) return RedirectToAction(nameof(Index)).Error("User not found.");

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (CurrentUser.Id != id) return RedirectToAction(nameof(Index)).Error("You're not allowed to that.");

            User user = db.Users.Find(id);
            if (!db.ClearUserData(user)) return RedirectToAction(nameof(Index)).Error("Error while clearing profile.");

            db.Users.Remove(user);
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return RedirectToAction(nameof(Index)).Error("You're not allowed to that.");
            }

            //Success, go back to default profile
            CurrentUser = null;
            return RedirectToAction(nameof(Index)).Success("User profile deleted.");
        }
    }
}
