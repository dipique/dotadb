using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public class Hero
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        [DisplayName("Alt. Names")]
        public string AltNames { get; set; } //can be searched as well so that, for example, BS can find Bloodseeker

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public HeroPreference Preference { get; set; } = HeroPreference.Indifferent;

        public List<HeroLabel> Labels = new List<HeroLabel>();

        #region Labels accessible as Counters and Synergies

        public LabelSet Counters
        {
            get => GetLabelsByType(RelationshipType.Counter);
            set => UpdateLabels(RelationshipType.Counter, value);
        }
        public LabelSet Synergies
        {
            get => GetLabelsByType(RelationshipType.Synergy);
            set => UpdateLabels(RelationshipType.Synergy, value);
        }

        public void UpdateLabels(RelationshipType type, LabelSet labels)
        {
            Labels.AddRange(labels.Where(v => !Labels.Any(l => l.Type == type &&
                                                               l.Value.Equals(v, StringComparison.CurrentCultureIgnoreCase)))
                                 .Select(v => new HeroLabel
                                 {
                                     HeroID = ID,
                                     Type = type,
                                     Value = v
                                 }));
            Labels.RemoveAll(l => l.Type == type && !labels.Any(v => v.Equals(l.Value, StringComparison.CurrentCultureIgnoreCase)));
        }
        public LabelSet GetLabelsByType(RelationshipType type) => (LabelSet)Labels.Where(l => l.Type == type)
                                                                                  .Select(l => l.Value).ToList();

        #endregion

        public virtual List<Tip> Tips { get; set; }
        public virtual List<Relationship> Relationships { get; set; }

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