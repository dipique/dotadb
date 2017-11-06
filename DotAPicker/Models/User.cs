using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;

using DotAPicker.Utilities;

namespace DotAPicker.Models
{
    public class User
    {
        public const string DEFAULT_USER = "default";

        public int ID { get; set; }
        public string Username { get; set; }

        //Settings
        private string currentPatch = DotAPatch.DEFAULT_PATCH;

        [DisplayName("Current Patch")]
        public string CurrentPatch
        {
            get => currentPatch;
            set
            {
                if (DotAPatch.ValidatePatchNumber(value))
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

        public List<Hero> Heroes { get; set; }
        public List<Tip> Tips { get; set; }
        public List<Relationship> Relationships { get; set; }
        public List<Setting> Settings { get; set; }

        public IEnumerable<string> GetLabels() => Settings.FirstOrDefault(s => s.UserID == ID && 
                                                                               s.Name == "Labels")
                                                         ?.Value?.Split('|') ?? new string[] { };
        public string GetCurrentPatch() => Settings.FirstOrDefault(s => s.UserID == ID &&
                                                                        s.Name == "CurrentPatch")
                                                  ?.Value;
        

    }

    public class LabelSet : List<string>
    {
    }
}