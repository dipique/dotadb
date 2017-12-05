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
    public class LoginController: DotAController
    {
        public ActionResult Index() => Login();

        public ActionResult Login() => View(new LoginViewModel());

        public ActionResult LoginError(LoginViewModel viewModel, string error) => 
            View("Login", 
                 new LoginViewModel() {
                     UsernameOrEmail = viewModel.UsernameOrEmail
                 }).Error(error);


        public ActionResult Login(LoginViewModel viewModel)
        {
            var uName = viewModel.UsernameOrEmail.ToLower();
            var users = db.Users.Where(u => u.Username.ToLower() == uName || u.Email.ToLower() == uName);

            if (users.Count() > 1) return LoginError(viewModel, "Ambiguous username. Something terrible has happened.");
            if (users.Count() == 1) return LoginError(viewModel, "Username or e-mail not found. Give it another try. Or don't, whatever.");

            var user = users.Single();
            if (!user.MatchingPassword(viewModel.Password)) return LoginError(viewModel, "T'ain't t'right pass code ya wacko");

            CurrentUser = user;
            return RedirectToAction("Index", "Home");
        }
    }
}