using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotAPicker.Models
{
    /// <summary>
    /// A relationship is a tip that discusses how one hero or label effects another (rather than
    /// a discussion about a single hero/label)
    /// </summary>
    [Table(nameof(Relationship))]
    public class Relationship: DotANote
    {

        private int? heroObjectId = null;
        public int? HeroObjectId
        {
            get => heroObjectId;
            set
            {
                if (value >= 0) //null returns false here
                {
                    LabelObject = string.Empty;
                }
                heroObjectId = value;
            }
        }

        public virtual Hero HeroObject { get; set; }

        private string labelObject = string.Empty;
        public string LabelObject
        {
            get => labelObject;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    heroObjectId = null;
                }
                labelObject = value;
            }
        }

        /// <summary>
        /// Single access property to the second target of the relationship. Heros are represented by their integer IDs
        /// </summary>
        [NotMapped]
        [Display(Name = "Object")]
        public string ObjectEntity
        {
            get => heroObjectId?.ToString() ?? LabelObject;
            set
            {
                if (int.TryParse(value, out int heroIDVal))
                {
                    HeroObjectId = heroIDVal;
                }
                else
                {
                    LabelObject = value;

                    //Make sure we can clear the value; it's not a valid state, but it needs to be possible at least in memory
                    if (value == null)
                    {
                        HeroObjectId = null;
                    }
                }
            }
        }

        public string ObjectName => HeroObject?.Name ?? LabelObject;

        public bool IncludesHero(int ID, string lbl = null) => HeroSubjectId == ID ||
                                                        HeroObjectId == ID || 
                                                        (!string.IsNullOrEmpty(lbl) && (LabelSubject == lbl || 
                                                                                        LabelObject == lbl));
        public override string NameSet => $"{base.NameSet}|{HeroObject?.Name}|{HeroObject?.AltNames}|{LabelObject}";
    }
}