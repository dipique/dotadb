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

        [HttpGet]
        public ActionResult ExportProfile() => View(new ExportProfileViewModel());
        public ActionResult ExportProfile(ExportProfileViewModel viewModel)
        {
            var profileString = new ProfileCopy(CurrentUser, viewModel.IncludeNotes).ToXML();
            return File(Encoding.Unicode.GetBytes(profileString),
                        "text/plain",
                        $"{CurrentUser.Name}-dpexport-{DateTime.Now.ToString("yyMMddhhmmss")}.xml");
                       //.Success("Export successfully completed!"); //this doesn't work, not sure why. TODO: figure out.
        }

        [HttpGet]
        public ActionResult ImportProfile() => View(new ImportProfileViewModel());
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
                db.ImportProfile(profile, CurrentUser);
                return View(new ImportProfileViewModel()).Success("Successfully imported profile!");
            }
            catch (Exception)
            {
                throw;
                //return View(new ImportProfileViewModel()).Error("Failed while trying to import profile data.");
            }

        }

    }
}