using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotAPicker.Models
{
    public abstract class DotANote: UserOwnedEntity
    {
        /// <summary>
        /// The "HeroSubject" of a tip is the hero this tip applies to.
        /// </summary>
        public virtual Hero HeroSubject { get; set; }
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
        /// Single access property to the first target of the tip. Heroes are represented by their integer IDs
        /// </summary>
        [Display(Name = "Subject")]
        [NotMapped]
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

        /// <summary>
        /// As the game changes, tips may go out of date; tracking the
        /// applicable patch assists with this, and users can flag a
        /// specific tip as out of date ("deprecated").
        /// </summary>
        [Required]
        public string Patch { get; set; }
        public bool Deprecated { get; set; } = false;

        /// <summary>
        /// Where the tip was found, possibly a URL. Can give context to
        /// a tip that isn't very intuitive.
        /// </summary>
        public string Source { get; set; }
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

    public class ObjectAffiliation : Attribute
    {
        public string TypeName { get; set; }
        public ObjectAffiliation(string typeName) => TypeName = typeName;
    }
}