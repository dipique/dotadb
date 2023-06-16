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
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@]{3,}$|^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$", ErrorMessage = "This doesn't look like a username or e-mail.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        public string Password { get; set; } = string.Empty;
    }
}