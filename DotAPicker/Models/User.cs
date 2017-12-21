using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Security.Principal;

using DotAPicker.Utilities;

namespace DotAPicker.Models
{
    public class User: IIdentity
    {
        public const string DEFAULT_USER = "default";
        public const string STD_DELIM = "|";
        public const string DISALLOWED_LABEL_CHARS = ":|";

        private const int PWD_ITERATIONS = 1000;
        private const int PWD_SALT_BYTES = 16;
        private const int PWD_HASH_BYTES = 20;

        public int Id { get; set; }

        #region IIdentify Requirements

        [Required]
        [StringLength(256)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public string AuthenticationType => ProfileType.ToString();

        [NotMapped]
        public bool IsAuthenticated { get; set; } = false;

        #endregion

        [Required]
        public string Email { get; set; }

        //This is actually a hash of the password. Use the "SetNewPassword" and "MatchingPassword" methods instead
        //of directly setting this property.
        public string Password { get; set; }

        public void SetNewPassword(string plainText)
        {
            //generate long random salt
            byte[] salt = new byte[PWD_SALT_BYTES];
            RandomNumberGenerator.Create().GetBytes(salt);

            //get hash value
            var pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, PWD_ITERATIONS);
            byte[] hash = pbkdf2.GetBytes(PWD_HASH_BYTES);

            //combine salt and hash
            byte[] hashBytes = new byte[PWD_HASH_BYTES + PWD_SALT_BYTES];
            Array.Copy(salt, 0, hashBytes, 0, PWD_SALT_BYTES);
            Array.Copy(hash, 0, hashBytes, PWD_SALT_BYTES, PWD_HASH_BYTES);

            //store as string
            Password = Convert.ToBase64String(hashBytes);
        }

        public bool MatchingPassword(string plainText)
        {            
            //retrieved saved values
            byte[] hashBytes = Convert.FromBase64String(Password);            
            byte[] salt = new byte[PWD_SALT_BYTES];
            Array.Copy(hashBytes, 0, salt, 0, PWD_SALT_BYTES);
            
            //compute the hash on the provided password
            var pbkdf2 = new Rfc2898DeriveBytes(plainText ?? string.Empty, salt, PWD_ITERATIONS);
            byte[] hash = pbkdf2.GetBytes(PWD_HASH_BYTES);

            //return whether the passwords are equal
            return hash.SequenceEqual(hashBytes.Skip(PWD_SALT_BYTES));
        }

        //Settings
        [Required]
        private string currentPatch = SettingValidators.DEFAULT_PATCH;

        [DisplayName("Current Patch")]
        [RegularExpression(SettingValidators.PATCH_REGEX, ErrorMessage = "Invalid patch name")]
        public string CurrentPatch
        {
            get => currentPatch;
            set
            {
                if (SettingValidators.ValidatePatchNumber(value))
                {
                    currentPatch = value;
                }
            }
        }

        [DisplayName("Profile Type")]
        public ProfileTypes ProfileType { get; set; } = ProfileTypes.Private;

        [DisplayName("Show Deprecated Tips")]
        public bool ShowDeprecatedTips { get; set; } = false;

        [DisplayName("Show Deprecated Relationships")]
        public bool ShowDeprecatedRelationships { get; set; } = false;

        [DisplayName("Label Options")]
        public string LabelOptions
        {
            get => string.Join(STD_DELIM, Labels);
            set => Labels = value.Split(STD_DELIM[0]).ToList();
        }

        [NotMapped, Display(Name = "Description Labels")]
        public List<string> Labels { get; set; } = new List<string>();

        public virtual List<Hero> Heroes { get; set; } = new List<Hero>();
        public virtual List<Tip> Tips { get; set; } = new List<Tip>();
        public virtual List<Relationship> Relationships { get; set; } = new List<Relationship>();
    }

    public class Principal: IPrincipal
    {
        public Principal(User user)
        {
            Identity = user;
        }

        public Principal() { }

        public IIdentity Identity { get; set; }
        public bool IsInRole(string roleString = null)
        {
            if (Identity.IsAuthenticated) return true;
            if (Identity.AuthenticationType == ProfileTypes.Public.ToString()) return true;
            return false;           
        }
    }

    public enum ProfileTypes
    {
        Private,
        ReadOnly,
        Public
    }
}