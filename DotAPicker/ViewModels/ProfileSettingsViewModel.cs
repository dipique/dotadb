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
        public ProfileTypes ProfileType { get; set; } = ProfileTypes.Private;
    }
}