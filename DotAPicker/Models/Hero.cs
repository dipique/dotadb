using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DotAPicker.Utilities;

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

        /// <summary>
        /// Stored in the form: Type1:Label1|Type2:Label2|Type3:Label3
        /// Types are the text of "RelationshipType" enums
        /// </summary>
        public string Labels { get; set; } = string.Empty;

        #region Labels accessible as Counters and Synergies

        private const char LBL_SEP = '|';
        private const char TYP_SEP = ':';

        public (RelationshipType Type, string Value)[] ParseLabels => string.IsNullOrEmpty(Labels) ? new(RelationshipType Type, string Value)[0] :
            Labels.Split(LBL_SEP)
                  .Select(l => l.Split(TYP_SEP))
                  .Select(l => (EnumExt.Parse<RelationshipType>(l[0]), l[1]))
                  .ToArray();        

        [NotMapped] public LabelSet Counters
        {
            get => GetLabelsByType(RelationshipType.Counter);
            set => UpdateLabels(RelationshipType.Counter, value);
        }

        [NotMapped] public LabelSet Synergies
        {
            get => GetLabelsByType(RelationshipType.Synergy);
            set => UpdateLabels(RelationshipType.Synergy, value);
        }

        public void UpdateLabels(RelationshipType type, LabelSet labels) =>
            Labels = string.Join(LBL_SEP.ToString(), 
                ParseLabels.Where(l => l.Type != type)                      //get all the existing labels that aren't of this type
                           .Concat(labels.Select(l => (type, l)))           //add the updated labels of the current type
                           .Select(l => $"{l.Item1}{TYP_SEP}{l.Item2}"));   //and convert them all into the string equivalent

        public LabelSet GetLabelsByType(RelationshipType type) => 
           new LabelSet(ParseLabels.Where(l => l.Type == type)
                                   .Select(l => l.Value));

        #endregion

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