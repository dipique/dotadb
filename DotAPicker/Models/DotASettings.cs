using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.Models
{
    public class DotASettings
    {
        public const string PATCH_REGEX = @"^(\d+\.\d{2}([a-zA-Z])?)$";
        private string currentPatch = "7.06"; //default patch
        [DisplayName("Current Patch")]
        public string CurrentPatch
        {
            get => currentPatch;
            set
            {                
                if (ValidatePatchNumber(value))
                {
                    currentPatch = value;
                }
            }
        }

        /// <summary>
        /// Patch numbers are either a decimal value, or a decimal value followed by a letter.
        /// Valid: 7.00, 7.07, 7.07a, 7.07A
        /// Invalid: 7, 7.0, 7.00ff, 7f, 700
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public bool ValidatePatchNumber(string patch) => new Regex(PATCH_REGEX).IsMatch(patch);

        [DisplayName("Show Deprecated Tips")]
        public bool ShowDeprecatedTips { get; set; } = false;

        [DisplayName("Show Deprecated Relationships")]
        public bool ShowDeprecatedRelationships { get; set; } = false;
        
    }
}