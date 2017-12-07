using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.DAL;
using DotAPicker.Models;
using DotAPicker.Utilities;
using DotAPicker.ViewModels;

namespace DotAPicker.Controllers
{
    public class SettingsController: DotAController
    {
        public ActionResult UserSettings() => View();

        public ActionResult DeleteProfile(DeleteProfileViewModel viewModel)
        {
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.Password)) return View("UserSettings").Error("Wrong password.");

            db.DeleteUser(CurrentUser);

            return RedirectToAction("Login", "Login", null).Success("User profile deleted");
        }

        public ActionResult PasswordReset(PasswordResetViewModel viewModel)
        {
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.CurrentPassword)) return View("UserSettings").Error("Wrong password.");
            CurrentUser.SetNewPassword(viewModel.NewPassword);
            db.Entry(CurrentUser).State = EntityState.Modified;
            db.SaveChanges();
            return View("UserSettings").Success("Password successfully changed.");
        }

        public ActionResult ReplaceProfile(ReplaceProfileViewModel viewModel)
        {
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.Password)) return View("UserSettings").Error("Wrong password.");            

            //make sure the profile to copy exists
            User profileToCopy = null;
            if (!string.IsNullOrWhiteSpace(viewModel.ProfileToCopy))
            {
                profileToCopy = db.Users.Where(u => u.ProfileType == ProfileTypes.Public)
                                        .FirstOrDefault(u => u.Name == viewModel.ProfileToCopy || u.Email == viewModel.ProfileToCopy);
                if (profileToCopy == null) return View("UserSettings").Error("Profile to copy doesn't exist or is private.");
            }

            //clear existing data
            db.ClearUserData(CurrentUser);

            //copy new data
            return db.CopyUser(profileToCopy, CurrentUser) ? View("UserSettings").Success("Profile successfully copied.")
                                                           : View("UserSettings").Error("Profile copy failed.");            
            
        }
    }
}