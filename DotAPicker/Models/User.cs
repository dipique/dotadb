using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

using DotAPicker.Utilities;

namespace DotAPicker.Models
{
    public class User
    {
        public const string DEFAULT_USER = "default";
        public const string STD_DELIM = "|";
        public const string DISALLOWED_LABEL_CHARS = ":|";

        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        [Index(IsUnique = true)]
        public string Username { get; set; }

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
}