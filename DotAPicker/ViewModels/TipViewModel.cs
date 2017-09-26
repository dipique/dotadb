using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Xml.Serialization;
using DotAPicker.Models;

namespace DotAPicker.ViewModels
{
    public class TipViewModel: Tip
    {

        public string HeroName { get; set; }
        public string HeroAltNames { get; set; }

        public IEnumerable<SelectListItem> HeroOptions { get; set; }
    }
}