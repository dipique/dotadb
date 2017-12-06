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

            ////temp
            //user.SetNewPassword("password");
            //db.Entry(user).State = EntityState.Modified;
            //db.SaveChanges();
            ////----

            if (!user.MatchingPassword(viewModel.Password)) return LoginError(viewModel, "T'ain't t'right pass code ya wacko");

            user.IsAuthenticated = true;
            HttpContext.User = new Principal(user);
            CurrentUser = user;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            CurrentUser = null;
            HttpContext.User = null;
            return RedirectToAction(nameof(Login));
        }
    }
}