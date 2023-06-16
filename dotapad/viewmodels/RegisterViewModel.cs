using System;
using System.ComponentModel.DataAnnotations;

using DotAPicker.Models;

namespace DotAPicker.ViewModels
{
    public class RegisterViewModel
    {
        //this will be required, but if it's empty we'll just use the e-mail
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@]{3,}$")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Profile Type")]
        public ProfileTypes ProfileType { get; set; } = ProfileTypes.ReadOnly;

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{3,}$")]
        [Compare(nameof(Password))]
        [Display(Name ="Re-Enter Password")]
        public string ComparePassword { get; set; } = string.Empty;

        [Display(Name = "Profile To Copy")]
        public string ProfileToCopy { get; set; } = User.DEFAULT_USER;

        public User ToUser()
        {
            //if no username, use e-mail
            if (string.IsNullOrWhiteSpace(Username))
                Username = Email;

            var user = new User {
                Name = Username,
                Email = Email,
                ProfileType = ProfileType
            };
            user.SetNewPassword(Password);
            return user;
        }
    }
}