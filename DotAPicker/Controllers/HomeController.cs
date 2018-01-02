using System;
using System.Collections.Generic;
using System.Web.Mvc;

using DotAPicker.Models;
using DotAPicker.ViewModels;


namespace DotAPicker.Controllers
{
    [RequiresAuth(Roles = "Authenticated")]
    public class HomeController : DotAController
    {
        // GET: Hero
        public ActionResult Index()
        {
            return GetItems(nameof(Hero.Name), SortDirections.Ascending);
        }

        public ActionResult GetItems(string sortField, SortDirections sortDirection)
        {
            return View("DotAPicker", new TableViewModel<Hero>()
            {
                Items = CurrentUser.Heroes,
                SortDirection = sortDirection,
                SortField = sortField
            });
        }

        public ActionResult HeroSort(string propertyName, string currentSortString)
        {
            if (!Enum.TryParse(currentSortString, out SortDirections currentSortDirection))
            {
                return Index();
            }

            SortDirections newSortDirection = SortDirections.Ascending;
            if (currentSortDirection == SortDirections.Ascending)
                newSortDirection = SortDirections.Descending;

            return GetItems(propertyName, newSortDirection);
        }

        public ActionResult Detail(int id) => PartialView(new HeroViewModel(GetHeroByID(id), CurrentUser.IsAuthenticated));
    }
}
