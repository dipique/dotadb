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
                {
                    return LoginError(viewModel, "Noice try bustah! Tryna sign in to a public profile widout no passamawhoozit!");
                }
                else
                {
                    if (user.ProfileType == ProfileTypes.ReadOnly)
                    {
                        user.IsAuthenticated = false;
                        SetCurrentUser(user);
                        return RedirectToAction("Index", "Home").Success("Aight, yer logged in ano... anoby... ah, just don't make no changes iffen ya please.");
                    }
                    else
                    {
                        //ProfileTypes.Public
                        user.IsAuthenticated = true;
                        SetCurrentUser(user);
                        return RedirectToAction("Index", "Home").Success("Dude straight up left his front door open and now you're in. Take care of the place!");
                    }
                }
            } 
            else
            {
                if (user.MatchingPassword(viewModel.Password))
                {
                    SetCurrentUser(user);
                    return RedirectToAction("Index", "Home").Success("Look at you, logging in like a pro.");
                }
                else
                {
                    return LoginError(viewModel, "T'ain't t'right pass code ya wacko");
                }
            }    
        }

        public ActionResult LogOut()
        {
            var user = Models.User.DefaultUser;
            CurrentUser = user;
            HttpContext.User = null;
            return RedirectToAction("Index", "Home").Success("Logged out successfully.");
        }

        public ActionResult Register() => View();

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            //logout first
            LogOut();

            //make sure not duplicate username/e-mail
            var user = viewModel.ToUser();
            if (db.Users.Any(u => u.Email == user.Email || u.Name == user.Name))
                return View().Error("This name or e-mail address has already been taken.");

            //make sure the profile to copy exists
            User profileToCopy = null;
            if (!string.IsNullOrWhiteSpace(viewModel.ProfileToCopy))
            {
                profileToCopy = db.Users.Where(u => u.ProfileType == ProfileTypes.Public || u.ProfileType == ProfileTypes.ReadOnly)
                                        .FirstOrDefault(u => u.Name == viewModel.ProfileToCopy || u.Email == viewModel.ProfileToCopy);
                if (profileToCopy == null) return View().Error("Profile to copy doesn't exist or is private.");
            }

            //add user
            db.Users.Add(user);
            db.SaveChanges(true);

            //copy profile if applicable
            var copyProfile = profileToCopy != null;
            var copySuccess = copyProfile ? db.CopyUser(profileToCopy, db.Users.Single(u => u.Name == user.Name), CurrentUser.IsAuthenticated) : false;
            SetCurrentUser(user);

            if (copyProfile)
            {
                return copySuccess ? RedirectToAction("Index", "Home").Success("User profile created and copied from template.")
                                   : RedirectToAction("Index", "Home").Information("User profile created but template copy failed.");
            } else
            {
                return RedirectToAction("Index", "Home").Success("User profile successfully created!");
            }

        }
    }
}