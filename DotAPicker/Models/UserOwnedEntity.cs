using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public abstract class UserOwnedEntity
    {
        public int Id { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }
}