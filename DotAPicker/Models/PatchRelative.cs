using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public abstract class PatchRelative
    {
        public int ID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        [Required]
        public string Patch { get; set; }
        public bool Deprecated { get; set; } = false;

        public string Source { get; set; } //where you found the tip
    }
}