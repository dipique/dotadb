using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name="Username Or Email")]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@]{3,}$")]
        public string UsernameOrEmail { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        public string Password { get; set; }
    }
}