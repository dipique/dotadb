using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using DotAPicker.Models;

namespace DotAPicker.ViewModels
{
    public class ProfileSettingsViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public ProfileTypes ProfileType { get; set; } = ProfileTypes.Private;
    }
}