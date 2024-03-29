﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace DotAPicker.ViewModels
{
    public class PasswordResetViewModel
    {
        [Required]
        [Display(Name = "Username Or Email")]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@]{3,}$|^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$", ErrorMessage = "This doesn't look like a username or e-mail.")]
        public string UsernameOrEmail { get; set; }

        [Required]
        [Display(Name = "Password Reset Token")]
        public string PasswordResetToken { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Compare(nameof(NewPassword))]
        [Display(Name = "Re-Enter Password")]
        public string ComparePassword { get; set; }
    }
}