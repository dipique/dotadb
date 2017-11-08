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

        public int ID { get; set; }

        [Required]
        [StringLength(256)]
        [Index(IsUnique = true)]
        public string Username { get; set; }

        //Settings
        [Required]
        private string currentPatch = SettingValidators.DEFAULT_PATCH;

        [DisplayName("Current Patch")]
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
        public string LabelOptions { get; set; }

        public LabelSet Labels
        {
            get => (LabelSet)LabelOptions.Split(LBL_SEP).ToList();
            set => String.Join(LBL_SEP.ToString(), value);
        }

        public virtual List<Hero> Heroes { get; set; }
        public virtual List<Tip> Tips { get; set; }
        public virtual List<Relationship> Relationships { get; set; }
        //public List<Setting> Settings { get; set; }

        //public IEnumerable<string> GetLabels() => Settings.FirstOrDefault(s => s.UserID == ID && 
        //                                                                       s.Name == "Labels")
        //                                                 ?.Value?.Split('|') ?? new string[] { };
        //public string GetCurrentPatch() => Settings.FirstOrDefault(s => s.UserID == ID &&
        //                                                                s.Name == "CurrentPatch")
        //                                          ?.Value;
        

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