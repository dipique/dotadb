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

        private const char LBL_SEP = '|';
        private const string DISALLOWED_LABEL_CHARS = ":|"; //needed to support hero label storage format

        [DisplayName("Label Options")]
        public string LabelOptions { get; set; } = string.Empty;

        public LabelSet Labels
        {
            get => new LabelSet(LabelOptions.Split(new char[] { LBL_SEP }, StringSplitOptions.RemoveEmptyEntries));
            set => LabelOptions =  String.Join(LBL_SEP.ToString(), value.Select(lbl => new string(lbl.Where(c => !DISALLOWED_LABEL_CHARS.Contains(c))
                                                                                      .ToArray())));
        }

        public virtual List<Hero> Heroes { get; set; } = new List<Hero>();
        public virtual List<Tip> Tips { get; set; } = new List<Tip>();
        public virtual List<Relationship> Relationships { get; set; } = new List<Relationship>();
    }

    public class LabelSet : List<string>
    {
        public const string STD_DELIM = "|";

        public LabelSet(IEnumerable<string> elements) : base(elements)
        {

        }

        public LabelSet(string elements, params string[] delims): base(elements.Split(delims, StringSplitOptions.RemoveEmptyEntries))
        {
        }

        public LabelSet(string elements) : this(elements, STD_DELIM)
        {
        }
    }
}