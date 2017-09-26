using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public class Tip: PatchRelative
    {
        public int HeroID { get; set; }


        public TipType Type { get; set; } = TipType.Other;
        public string Text { get; set; }
    }

    public enum TipType
    {
        Counter,
        Strategy,
        ItemBuild,
        AbilityBuild,
        AbilityUse,
        Other
    }
}