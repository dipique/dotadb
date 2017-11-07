using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public class Tip: PatchRelative
    {
        public int HeroID { get; set; }
        public Hero Hero { get; set; }


        public TipType Type { get; set; } = TipType.Other;
        
        [Required]
        public string Text { get; set; }
    }

    public enum TipType
    {
        Counter,

        Strategy,

        [Display(Name = "Item Build")]
        ItemBuild,

        [Display(Name = "Ability Build")]
        AbilityBuild,

        [Display(Name = "Abilty Use")]
        AbilityUse,

        Other
    }
}