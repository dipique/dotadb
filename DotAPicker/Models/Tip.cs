using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public class Tip: PatchRelative
    {
        private int? heroSubjectID = null;
        public int? HeroSubjectID
        {
            get => heroSubjectID;
            set
            {
                if (value >= 0) //null returns false here
                {
                    LabelSubject = string.Empty;
                }
                heroSubjectID = value;
            }
        }
        public Hero HeroSubject { get; set; }

        private string labelSubject = string.Empty;
        public string LabelSubject
        {
            get => labelSubject;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    heroSubjectID = null;
                }
                labelSubject = value;
            }
        }

        /// <summary>
        /// Single access property to the first target of the tip. Heros are represented by their integer IDs
        /// </summary>
        [Required]
        [Display(Name = "Subject")]
        public string SubjectEntity
        {
            get => heroSubjectID?.ToString() ?? LabelSubject;
            set
            {
                if (int.TryParse(value, out int heroIDVal))
                {
                    HeroSubjectID = heroIDVal;
                }
                else
                {
                    LabelSubject = value;

                    //Make sure we can clear the value; it's not a valid state, but it needs to be possible at least in memory
                    if (value == null)
                    {
                        HeroSubjectID = null;
                    }
                }
            }
        }
        public string SubjectName => HeroSubject?.Name ?? LabelSubject;

        public virtual string NameSet => $"{HeroSubject?.Name}|{HeroSubject?.AltNames}|{LabelSubject}";

        public TipType Type { get; set; } = TipType.Counter;
        
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }

    public enum TipType
    {
        Counter,

        [ObjectAffiliation(nameof(Tip))]
        Strategy,

        [ObjectAffiliation(nameof(Tip))]
        [Display(Name = "Item Build")]
        ItemBuild,

        [ObjectAffiliation(nameof(Tip))]
        [Display(Name = "Ability Build")]
        AbilityBuild,

        [ObjectAffiliation(nameof(Tip))]
        [Display(Name = "Abilty Use")]
        AbilityUse,

        Synergy,

        Other
    }

    public class ObjectAffiliation: Attribute
    {
        public string TypeName { get; set; }
        public ObjectAffiliation(string typeName) => TypeName = typeName;
    }
}