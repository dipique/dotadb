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
    [AllowAnonymous]
    public class LoginController: DotAController
    {
        [HttpGet]
        public ActionResult Index() => Login();

        public ActionResult Login() => View(new LoginViewModel());

        public ActionResult LoginError(LoginViewModel viewModel, string error) => 
            View("Login", 
                 new LoginViewModel() {
                     UsernameOrEmail = viewModel.UsernameOrEmail
                 }).Error(error);

        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
            var uName = viewModel.UsernameOrEmail?.ToLower() ?? string.Empty;
            var users = db.Users.Where(u => u.Name.ToLower() == uName || u.Email.ToLower() == uName);

            if (users.Count() > 1) return LoginError(viewModel, "Ambiguous username. Something terrible has happened.");
            if (users.Count() == 0) return LoginError(viewModel, "Username or e-mail not found. Give it another try. Or don't, whatever.");

            var user = users.Single();

            var noPasswordEntered = string.IsNullOrEmpty(viewModel.Password);
            if (noPasswordEntered)
            {
                if (user.ProfileType == ProfileTypes.Private)
                    return LoginError(viewModel, "Noice try bustah! Tryna sign in to a public profile widout no passamawhoozit!");

                if (user.ProfileType == ProfileTypes.ReadOnly)
                {
                    user.IsAuthenticated = false;
                    SetCurrentUser(user);
                    return RedirectToAction("Index", "Home").Success("Aight, yer logged in ano... anoby... ah, just don't make no changes iffen ya please.");
                }

                //ProfileTypes.Public
                user.IsAuthenticated = true;
                SetCurrentUser(user);
                return RedirectToAction("Index", "Home").Success("Dude straight up left his front door open and now you're in. Take care of the place!");
            } 

            //password was entered
            if (user.MatchingPassword(viewModel.Password))
            {
                user.IsAuthenticated = true;
                SetCurrentUser(user);
                return RedirectToAction("Index", "Home").Success("Look at you, logging in like a pro.");
            }

            //password mismatch
            return LoginError(viewModel, "T'ain't t'right pass code ya wacko");
        }

        public ActionResult LogOut()
        {
            LogOutUser();
            return RedirectToAction("Index", "Home").Success("Logged out successfully.");
        }

        public void LogOutUser()
        {
            var user = Models.User.DefaultUser;
            CurrentUser = user;
            HttpContext.User = new Principal(user);
        }

        public ActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            //logout first
            LogOutUser();

            //make sure not duplicate username/e-mail
            var user = viewModel.ToUser();
            if (db.Users.Any(u => u.Email == user.Email || u.Name == user.Name || u.Name == user.Email))
                return View(viewModel).Error("This name or e-mail address has already been taken.");

            //make sure the profile to copy exists
            User profileToCopy = null;
            if (!string.IsNullOrWhiteSpace(viewModel.ProfileToCopy))
            {
                profileToCopy = db.Users.Where(u => u.ProfileType == ProfileTypes.Public || u.ProfileType == ProfileTypes.ReadOnly)
                                        .FirstOrDefault(u => u.Name == viewModel.ProfileToCopy || u.Email == viewModel.ProfileToCopy);
                if (profileToCopy == null) return View().Error("Profile to copy doesn't exist or is private.");
            }

            //set up user to see default tips & relationships
            user.SyncedProfiles.Add(Models.User.DEFAULT_USER);

            //add user
            db.Users.Add(user);
            db.SaveChanges(true);

            //copy profile if applicable
            var copyProfile = profileToCopy != null;
            var copySuccess = copyProfile ? db.CopyUser(profileToCopy, user, true, false) : false;
            SetCurrentUser(user);

            return copyProfile ? copySuccess ? RedirectToAction("Index", "Home").Success("User profile created and copied from template.")
                                             : RedirectToAction("Index", "Home").Information("User profile created but template copy failed.")
                               : RedirectToAction("Index", "Home").Success("User profile successfully created!");
        }

        private const string PWD_RESET_URL = "dotapad.com/Login/PasswordReset";

        public ActionResult ForgotPassword() => View();
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = db.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email);
            if (user != null)
            {
                var token = Guid.NewGuid().ToString();
                user.PasswordResetToken = token;
                db.SaveChanges();
                var resetURL = $"{PWD_RESET_URL}?token={token}";

                var body = $"This yo password reset telegraph. Reset token-y skidoobadaddle be: {token}. Yuccan click d's'ere lunkmuffin and use it thur.\n\n{resetURL}\n\nHere t'is agin:\n\n{token}";
                Email.SendEmail(user.Email, "Poissenvaken Rusitt", body);                
            }

            return RedirectToAction(nameof(PasswordReset)).Success("If that e-mail is on file you'll get a password reset token. If it ain't that token will not know da way.");
        }

        public ActionResult PasswordReset(string token = null) => string.IsNullOrEmpty(token) ? View() : View(new PasswordResetViewModel() { PasswordResetToken = token });

        [HttpPost]
        public ActionResult PasswordReset(PasswordResetViewModel model)
        {
            var name = model.UsernameOrEmail?.ToLower() ?? string.Empty;
            var user = db.Users.Where(u => u.Name.ToLower() == name || u.Email.ToLower() == name)
                                .FirstOrDefault(u => u.PasswordResetToken == model.PasswordResetToken);
            if (user == null) return View(model).Warning("Nice try buster. You won't get one on me that easily!");

            //actually reset the password
            user.SetNewPassword(model.NewPassword); //this also clears the token
            db.SaveChanges();

            SetCurrentUser(user);
            return RedirectToAction("Index", "Home").Success("Password has been reset! Nice job buddy, I'm proud of you. <3");
        }
    }
}