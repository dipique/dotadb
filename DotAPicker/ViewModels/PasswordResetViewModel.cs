using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace DotAPicker.ViewModels
{
    public class PasswordResetViewModel
    {
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