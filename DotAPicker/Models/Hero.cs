using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DotAPicker.Utilities;

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

        public string NameSet => $"{Name}|{AltNames}|";

        public string Labels { get; set; } = string.Empty;

        [NotMapped, Display(Name = "Description Labels")]
        public LabelSet DescriptionLabels
        {
            get => new LabelSet(Labels.Split(new char[] { LabelSet.STD_DELIM[0] }, StringSplitOptions.RemoveEmptyEntries));
            set => Labels = String.Join(LabelSet.STD_DELIM, value.Select(lbl => new string(lbl.Where(c => !LabelSet.DISALLOWED_LABEL_CHARS.Contains(c))
                                  .ToArray())));
        }

        [NotMapped, Display(Name = "Description Labels")]
        public List<string> DescriptionLabelsList
        {
            get => new List<string>(Labels.Split(new char[] { LabelSet.STD_DELIM[0] }, StringSplitOptions.RemoveEmptyEntries));
            set => Labels = String.Join(LabelSet.STD_DELIM, value.Select(lbl => new string(lbl.Where(c => !LabelSet.DISALLOWED_LABEL_CHARS.Contains(c))
                                  .ToArray())));
        }

        [NotMapped]
        public virtual List<Tip> Tips { get; set; } = new List<Tip>();

        [NotMapped]
        public virtual List<Relationship> Relationships { get; set; } = new List<Relationship>();

        public string GetImgName(string heroName)
        {
            string working = heroName.Replace(' ', '_');
            return $"img/hero/120px-{working}_icon.png";
        }

        public string GetImgName() => GetImgName(Name);
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