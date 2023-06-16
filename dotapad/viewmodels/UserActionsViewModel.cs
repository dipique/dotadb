using System;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Compare(nameof(NewPassword))]
        [Display(Name = "Re-Enter Password")]
        public string ComparePassword { get; set; } = string.Empty;
    }

    public class DeleteProfileViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }

    public class ReplaceProfileViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Profile To Copy")]
        public string ProfileToCopy { get; set; } = string.Empty;
    }
}