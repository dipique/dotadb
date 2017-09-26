using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public abstract class PatchRelative
    {
        public int ID { get; set; }

        public string Patch { get; set; }
        public bool Deprecated { get; set; } = false;

        public string Source { get; set; } //where you found the tip
    }
}