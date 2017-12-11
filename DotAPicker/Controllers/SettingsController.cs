using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.DAL;
using DotAPicker.Models;
using DotAPicker.ViewModels;

namespace DotAPicker.Controllers
{
    public class SettingsController: DotAController
    {
        [HttpGet]
        public ActionResult DeleteProfile() => View();
        public ActionResult DeleteProfile(DeleteProfileViewModel viewModel)
        {
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.Password)) return View().Error("Wrong password.");

            db.DeleteUser(CurrentUser);
            CurrentUser = null;
            HttpContext.User = null;

            return RedirectToAction("Login", "Login", null).Success("User profile deleted");
        }

        [HttpGet]
        public ActionResult PasswordReset() => View();
        public ActionResult PasswordReset(PasswordResetViewModel viewModel)
        {
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.CurrentPassword)) return View().Error("Wrong password.");
            CurrentUser.SetNewPassword(viewModel.NewPassword);
            db.Entry(CurrentUser).State = EntityState.Modified;
            db.SaveChanges();
            ModelState.Clear();
            return View().Success("Password successfully changed.");
        }

        [HttpGet]
        public ActionResult ReplaceProfile() => View();
        public ActionResult ReplaceProfile(ReplaceProfileViewModel viewModel)
        {
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.Password)) return View().Error("Wrong password.");            

            //make sure the profile to copy exists
            User profileToCopy = null;
            if (!string.IsNullOrWhiteSpace(viewModel.ProfileToCopy))
            {
                profileToCopy = db.Users.Where(u => u.ProfileType == ProfileTypes.Public)
                                        .FirstOrDefault(u => u.Name == viewModel.ProfileToCopy || u.Email == viewModel.ProfileToCopy);
                if (profileToCopy == null) return View().Error("Profile to copy doesn't exist or is private.");
            }

            //clear existing data
            db.ClearUserData(CurrentUser);
            ModelState.Clear();

            //copy new data
            return db.CopyUser(profileToCopy, CurrentUser) ? RedirectToAction("Index", "Hero", null).Success("Profile successfully copied.")
                                                           : View().Error("Profile copy failed.");            
            
        }
    }
}