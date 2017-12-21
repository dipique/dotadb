using System;
using System.Linq;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DotAPicker.Utilities
{
    public static class SettingValidators
    {
        /// <summary>
        /// Patch numbers are either a decimal value, or a decimal value followed by a letter.
        /// Valid: 7.00, 7.07, 7.07a, 7.07A
        /// Invalid: 7, 7.0, 7.00ff, 7f, 700
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        [SettingValidator("CurrentPatch")]
        public static bool ValidatePatchNumber(string patch) => new Regex(PATCH_REGEX).IsMatch(patch);
        public const string PATCH_REGEX = @"^(\d+\.\d{2}([a-zA-Z])?)$";

        public const string DEFAULT_PATCH = "7.07";
    }

    /// <summary>
    /// This indicates a method used for validating Setting objects; must accept string and return bool
    /// </summary>
    public class SettingValidator : Attribute
    {
        public string SettingName { get; set; }
        public SettingValidator(string settingName)
        {
            SettingName = settingName;
        }
    }
}