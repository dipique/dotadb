using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotAPicker.Models
{
    [Table(nameof(Tip))]
    public class Tip: UserOwnedEntity
    {
        private int? heroSubjectId = null;
        public int? HeroSubjectId
        {
            get => heroSubjectId;
            set
            {
                if (value >= 0) //null returns false here
                {
                    LabelSubject = string.Empty;
                }
                heroSubjectId = value;
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
                    heroSubjectId = null;
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
            get => heroSubjectId?.ToString() ?? LabelSubject;
            set
            {
                if (int.TryParse(value, out int heroIDVal))
                {
                    HeroSubjectId = heroIDVal;
                }
                else
                {
                    LabelSubject = value;

                    //Make sure we can clear the value; it's not a valid state, but it needs to be possible at least in memory
                    if (value == null)
                    {
                        HeroSubjectId = null;
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

        [Required]
        public string Patch { get; set; }
        public bool Deprecated { get; set; } = false;

        public string Source { get; set; } //where you found the tip
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