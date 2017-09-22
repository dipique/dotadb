using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public abstract class PatchRelative
    {
        public string Patch { get; set; }
        public bool Deprecated { get; set; } = false;
    }
}