using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public class Relationship: PatchRelative
    {
        public string Hero1 { get; set; }
        public string Hero2 { get; set; }

        /// <summary>
        /// always 1->2, so if the type is "counter", it means "Hero1 counters Hero2"
        /// </summary>
        public RelationshipType Type { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
    }

    public enum RelationshipType
    {
        Other,
        Counter,
        Synergy,
        OpportunityWhenAlly,
        OpportunityWhenEnemy
    }
}