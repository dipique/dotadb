using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotAPicker.Models
{
    public class Hero: UserOwnedEntity
    {
        public string Name { get; set; }

        [DisplayName("Alt. Names")]
        public string AltNames { get; set; } //can be searched as well so that, for example, BS can find Bloodseeker

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public HeroPreference Preference { get; set; } = HeroPreference.Indifferent;

        public string NameSet => $"{Name}|{AltNames}|{Labels}";

        public string Labels
        {
            get => string.Join(User.STD_DELIM, DescriptionLabels);
            set => DescriptionLabels = value.Split(User.STD_DELIM[0]).ToList();
        }

        [NotMapped, Display(Name = "Description Labels")]
        public List<string> DescriptionLabels { get; set; } = new List<string>();

        [NotMapped]
        public virtual List<Tip> Tips { get; set; } = new List<Tip>();

        [NotMapped]
        public virtual List<Relationship> Relationships { get; set; } = new List<Relationship>();
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