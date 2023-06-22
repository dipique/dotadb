using System;
using System.ComponentModel.DataAnnotations;
using DotAPicker.Utilities;
using Newtonsoft.Json;

namespace DotAPicker.Models
{
    public abstract class UserOwnedEntity : ISortable
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}