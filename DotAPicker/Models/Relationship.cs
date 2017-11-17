using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    /// <summary>
    /// A relationship is a tip that discusses how one hero or label effects another (rather than
    /// a discussion about a single hero/label)
    /// </summary>
    public class Relationship: Tip
    {
        private int? heroObjectID = null;
        public int? HeroObjectID
        {
            get => heroObjectID;
            set
            {
                if (value >= 0) //null returns false here
                {
                    LabelObject = string.Empty;
                }
                heroObjectID = value;
            }
        }
        public Hero HeroObject { get; set; }

        private string labelObject = string.Empty;
        public string LabelObject
        {
            get => labelObject;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    heroObjectID = null;
                }
                labelObject = value;
            }
        }

        /// <summary>
        /// Single access property to the second target of the relationship. Heros are represented by their integer IDs
        /// </summary>
        [Required]
        [Display(Name = "Object")]
        public string ObjectEntity
        {
            get => heroObjectID?.ToString() ?? LabelObject;
            set
            {
                if (int.TryParse(value, out int heroIDVal))
                {
                    HeroObjectID = heroIDVal;
                }
                else
                {
                    LabelObject = value;

                    //Make sure we can clear the value; it's not a valid state, but it needs to be possible at least in memory
                    if (value == null)
                    {
                        HeroObjectID = null;
                    }
                }
            }
        }

        public string ObjectName => HeroObject?.Name ?? LabelObject;

        public bool IncludesHero(int ID, string lbl = null) => HeroSubjectID == ID ||
                                                        HeroObjectID == ID || 
                                                        (!string.IsNullOrEmpty(lbl) && (LabelSubject == lbl || 
                                                                                        LabelObject == lbl));
        public override string NameSet => $"{base.NameSet}|{HeroObject?.Name}|{HeroObject?.AltNames}|{LabelObject}";

    }
}