using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public class Hero
    {
        public string Name { get; set; } //this is a unique ID
        public string AltNames { get; set; } //can be searched as well so that, for example, BS can find Bloodseeker

        public string Notes { get; set; }
        public HeroPreference Preference { get; set; } = HeroPreference.Indifferent;        
    }

    public enum HeroPreference
    {
        
        Hated = 0,
        Disliked = 1,
        Indifferent = 2,
        Preferred = 3,
        Favorite = 4
    }
}