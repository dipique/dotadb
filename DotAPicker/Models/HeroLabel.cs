using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public class HeroLabel
    {
        public int ID { get; set; }

        public int HeroID { get; set; }
        public Hero Hero { get; set; }

        public RelationshipType Type { get; set; }

        //public string Name { get; set; }
        public string Value { get; set; }
    }
}