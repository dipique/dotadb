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
        public string AltNames { get; set; } = string.Empty; //can be searched as well so that, for example, BS can find Bloodseeker

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; } = string.Empty;
        public HeroPreference Preference { get; set; } = HeroPreference.Indifferent;

        public string NameSet => $"{Name}{User.STD_DELIM}{AltNames}{User.STD_DELIM}{Initials}";

        //For example, Ancient Apparition would be aa. Makes it so you don't need to type in every single one as an alternate name.
        public string Initials
        {
            get
            {
                //split name into words
                var words = Name.Split(' ');
                if (words.Count() <= 1) return string.Empty;

                //return the first letter of each word
                return string.Join(string.Empty, words.Select(w => w.Substring(0, 1))).ToLower();
            }
        }

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