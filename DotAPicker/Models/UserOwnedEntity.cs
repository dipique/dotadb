using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotAPicker.Models
{
    public abstract class UserOwnedEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public virtual User User { get; set; }
    }
}