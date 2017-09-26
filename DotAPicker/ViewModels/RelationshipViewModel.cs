using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.ViewModels
{
    public class RelationshipViewModel: Relationship
    {
        public IEnumerable<SelectListItem> HeroOptions { get; set; }

        public Hero Hero1 { get; set; }
        public Hero Hero2 { get; set; }
        public bool IncludesHero(string heroName) => Hero2?.Name == heroName && Hero1?.Name == heroName;
        public string AltNameSet => $"{Hero1?.AltNames}|{Hero2?.AltNames}";
    }
}