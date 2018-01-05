using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

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

    public class DeleteProfileViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class ReplaceProfileViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Profile To Copy")]
        public string ProfileToCopy { get; set; }
    }
}