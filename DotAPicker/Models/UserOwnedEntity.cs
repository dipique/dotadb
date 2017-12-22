using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DotAPicker.Utilities;

namespace DotAPicker.Models
{
    public abstract class UserOwnedEntity : ISortable
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}