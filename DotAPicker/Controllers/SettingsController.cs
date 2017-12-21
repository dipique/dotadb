using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;

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

            if (!db.DeleteUser(CurrentUser))
            {
                return View(viewModel).Error("Something went wrong or you don't have enough permissions to do that thing there.");
            }
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
            if (db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                ModelState.Clear();
                return View().Success("Password successfully changed.");
            }
            else
                return View().Error("You're not allowed to do that.");
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
                profileToCopy = db.Users.Where(u => u.ProfileType == ProfileTypes.Public || u.ProfileType == ProfileTypes.ReadOnly)
                                        .FirstOrDefault(u => u.Name == viewModel.ProfileToCopy || u.Email == viewModel.ProfileToCopy);
                if (profileToCopy == null) return View().Error("Profile to copy doesn't exist or is private.");
            }

            //clear existing data
            if (!db.ClearUserData(CurrentUser))
            {
                return View(viewModel).Error("Something went wrong or you don't have enough permissions to do that thing there.");
            }
            ModelState.Clear();

            //copy new data
            return db.CopyUser(profileToCopy, CurrentUser, CurrentUser.IsAuthenticated) ? RedirectToAction("Index", "Hero", null).Success("Profile successfully copied.")
                                                                                        : View().Error("Profile copy failed.");            
            
        }

        [HttpGet]
        public ActionResult ExportProfile() => View(new ExportProfileViewModel() { IncludeNotes = true });
        public ActionResult ExportProfile(ExportProfileViewModel viewModel)
        {
            var profileString = new ProfileCopy(CurrentUser, viewModel.IncludeNotes).ToXML();
            return File(Encoding.Unicode.GetBytes(profileString),
                        "text/plain",
                        $"{CurrentUser.Name}-dpexport-{DateTime.Now.ToString("yyMMddhhmmss")}.xml");
                       //.Success("Export successfully completed!"); //this doesn't work, not sure why. TODO: figure out.
        }

        [HttpGet]
        public ActionResult ImportProfile() => View(new ImportProfileViewModel() { IncludeNotes = true });
        public ActionResult ImportProfile(ImportProfileViewModel viewModel)
        {
            var serializer = new XmlSerializer(typeof(ProfileCopy));

            //Parse the input file
            ProfileCopy profile = null;
            try { profile = (ProfileCopy)serializer.Deserialize(viewModel.PostedFile.InputStream); }
            catch (Exception)
            {
                throw;
                //return View(new ImportProfileViewModel()).Error("Couldn't parse upload file.");
            }

            //Import the data
            try
            {
                db.ClearUserData(CurrentUser);
                if (!db.ImportProfile(profile, CurrentUser, viewModel.IncludeNotes))
                {
                    return View(viewModel).Error("Something went wrong or you don't have enough permissions to do that thing there.");
                }
                return View(new ImportProfileViewModel() { IncludeNotes = true }).Success("Successfully imported profile!");
            }
            catch (Exception)
            {
                throw;
                //return View(new ImportProfileViewModel()).Error("Failed while trying to import profile data.");
            }

        }

        [HttpGet]
        public ActionResult ProfileSettings() => View(new ProfileSettingsViewModel() { ProfileType = CurrentUser.ProfileType });
        public ActionResult ProfileSettings(ProfileSettingsViewModel viewModel)
        {
            //make sure this user is allowed to make this change
            if (!CurrentUser.IsAuthenticated) return View(new ProfileSettingsViewModel() { ProfileType = CurrentUser.ProfileType }).Information("You're a dumbass. Seriously.");

            //See if there was a change
            if (viewModel.ProfileType == CurrentUser.ProfileType) return View(viewModel).Information("Dude that's what it already is.");

            //save the change
            CurrentUser.ProfileType = viewModel.ProfileType;
            db.Entry(CurrentUser).State = EntityState.Modified;
            if (!db.SaveChangesB(CurrentUser.IsAuthenticated))
            {
                return View(new ProfileSettingsViewModel() { ProfileType = CurrentUser.ProfileType }).Error("You're not allowed to do that");
            }

            return View(new ProfileSettingsViewModel() { ProfileType = CurrentUser.ProfileType }).Success("Profile type changed!");
        }

    }
}