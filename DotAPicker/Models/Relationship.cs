using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public class Relationship: PatchRelative
    {
        public int Hero1ID { get; set; }      
        public Hero Hero1 { get; set; }

        public int Hero2ID { get; set; }
        public Hero Hero2 { get; set; }

        public bool IncludesHero(int ID) => Hero1ID == ID || Hero2ID == ID;
        public string AltNameSet => $"{Hero1?.AltNames}|{Hero2?.AltNames}";

        /// <summary>
        /// always 1->2, so if the type is "counter", it means "Hero1 counters Hero2"
        /// </summary>
        public RelationshipType Type { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public enum RelationshipType
    {
        Other,
        Counter,
        Synergy
    }
}