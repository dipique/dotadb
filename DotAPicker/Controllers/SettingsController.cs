using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
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
            //validate password
            if (!CurrentUser.MatchingPassword(viewModel.Password)) return View().Error("Wrong password.");

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

        [HttpGet]
        public ActionResult ImportLabels() => View(new ImportLabelsViewModel());
        public ActionResult ImportLabels(ImportLabelsViewModel viewModel)
        {
            if (viewModel.PostedFile == null) return View(viewModel).Error("Umm... you know you DO need to upload SOMETHING, don't you?");

            int MAX_LABEL_FILE_SIZE = 100000; //no more than 100KB, which is already a shit ton of labels
            char FILE_DELIM_CHAR = '\t';
            char[] controlChars = { '\t', '\r', '\n' };
            char[] allowedExtraChars = { ' ', '-', '\''};
            if (viewModel.PostedFile.ContentLength > MAX_LABEL_FILE_SIZE) return View(viewModel).Error("Seriously, how many labels you got?! Fuck off. (Or you might have picked the wrong file or tried to upload an Excel file or some shit.");

            //get the string that represents the file contents
            var contents = new StreamReader(viewModel.PostedFile.InputStream).ReadToEnd().Replace("\r", "");

            //check for weird shit
            if (contents.Any(c => !char.IsLetterOrDigit(c) && !controlChars.Contains(c) && !allowedExtraChars.Contains(c))) return View(viewModel).Error("Let's stick with letters and numbers in our damn labels iffen you please.");
            var lines = contents.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (lines.Count() <= 1) return View(viewModel).Error("Gotta give me more than that buddy. At least one hero per label.");

            //get the labels
            var labels = lines[0].Split(FILE_DELIM_CHAR).Select(s => s.Trim()).ToArray();
            var labelCount = labels.Count();

            //create the list to be imported
            var labelAssignments = new List<KeyValuePair<string, string>>(); //hero name, label name
            for (int lineIndex = 1; lineIndex < lines.Count(); lineIndex++)
            {
                var heroes = lines[lineIndex].Split(FILE_DELIM_CHAR).Select(s => s.Trim())
                                                                    .Select(s => s.Length == 1 ? string.Empty : s) //removes single-character labels
                                                                    .ToArray();
                if (heroes.Count() > labelCount) return View(viewModel).Error("Seems like there are more hero categories than actual labels, which is a problem.");
                for (int heroIndex = 0; heroIndex < heroes.Count(); heroIndex++)
                {
                    var label = labels[heroIndex];
                    if (!string.IsNullOrWhiteSpace(label))
                    {
                        labelAssignments.Add(new KeyValuePair<string, string>(heroes[heroIndex], labels[heroIndex]));
                    }
                }
            }

            //We've made it to this point, let's add some labels!
            foreach (var heroLabels in labelAssignments.GroupBy(la => la.Key)
                                                       .Where(lag => CurrentUser.Heroes.Any(h => h.Name.ToUpper() == lag.Key.ToUpper()))) //where the hero exists already
            {
                var hero = CurrentUser.Heroes.First(h => h.Name.ToUpper() == heroLabels.Key.ToUpper());
                var newLabels = heroLabels.Select(kvp => kvp.Value) //get the labels for the hero
                                          .Where(lbl => !hero.DescriptionLabels.Any(dl => dl.ToUpper().Trim() == lbl.ToUpper())); //but only those that don't already exist
                if (newLabels.Count() == 0) continue; //don't make any changes if none of the labels are new

                //add the new labels
                hero.DescriptionLabels.AddRange(heroLabels.Select(kvp => kvp.Value));
                hero.DescriptionLabels.RemoveAll(l => string.IsNullOrWhiteSpace(l)); //remove any empty labels
                db.Entry(hero).State = EntityState.Modified;
                if (!db.SaveChangesB(true))
                {
                    return View(viewModel).Error("Sorry, there was a problem saving the changes to the database. Maybe you're too ugly.");
                }
            }

            return View().Success("Labels imported successfully! Frankly I'm a little surprised.");
        }

        [HttpGet]
        public ActionResult UploadHeroImage(int heroID) => View(new UploadHeroImageViewModel(heroID));
        public ActionResult UploadHeroImage(UploadHeroImageViewModel viewModel)
        {
            //make sure the hero ID belongs to someone signed in
            if (!CurrentUser.IsAuthenticated) return View(viewModel).Error("You must be authenticated to upload images.");

            //Make sure it's a PNG
            var filename = viewModel?.PostedFile?.FileName ?? string.Empty;
            if (filename.Length < 5 || filename.Substring(filename.Length - 2).ToUpper() == "PNG") return View(viewModel).Error("What don't you flippin understand about P, N flippin G?");

            //make sure it's a hero the user has access to, and get the hero name
            var hero = CurrentUser.Heroes.FirstOrDefault(h => h.Id == viewModel.HeroID);
            if (hero == null) return View(viewModel).Error("Sorry kiddo, you can't touch that there hero. Nice try though. Really.");
            var heroName = hero.Name;

            //make sure the file is less than 1Mb
            const int MAX_FILE_SIZE = 1048576; //bytes
            if (viewModel.PostedFile.ContentLength > MAX_FILE_SIZE) return View(viewModel).Error("Too big. I ain't even gonna tell you how big it can be. You just have to GUESS.");

            //copy the file to a temp directory on the server
            var tmpPath = HttpContext.Server.MapPath("~/tmp");
            var tmpTargetPath = Path.Combine(tmpPath, filename);

            //make sure it's the right dimensions
            const int IMG_WIDTH = 120;
            const int IMG_HEIGHT = 68;
            viewModel.PostedFile.SaveAs(tmpTargetPath);
            using (var img = Image.FromFile(tmpTargetPath))
                if (img.Height != IMG_HEIGHT || img.Width != IMG_WIDTH) return View(viewModel).Error("It's just gotta be those dimensions. Listen, I don't make the rules. Well, I do. But they ain't changin'.");

            //cool! Bring the file in!
            var targetPath = HeroViewModel.GetImgFilename(heroName);
            var file = new FileInfo(tmpTargetPath);
            try
            {
                file.MoveTo(targetPath);
            }
            catch (Exception)
            {
                return View(viewModel).Error("You know, I actually tried. But those obstructionist librals done did me in.");
            }

            //after all that hopefully it worked
            TempData["SelectedHeroID"] = viewModel.HeroID;
            return RedirectToAction("Index", "Hero").Success("File successfully uploaded");
        }
    }
}