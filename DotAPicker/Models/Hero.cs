using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public class Hero
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [DisplayName("Alt. Names")]
        public string AltNames { get; set; } //can be searched as well so that, for example, BS can find Bloodseeker

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public HeroPreference Preference { get; set; } = HeroPreference.Indifferent;

        public LabelSet Synergies { get; set; } = new LabelSet();
        public LabelSet Counters { get; set; } = new LabelSet();
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