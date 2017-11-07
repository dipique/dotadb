using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace DotAPicker.Utilities
{
    public static class Casting
    {
        /// <summary>
        /// Takes an object and converts it to its parent type, stripping off any unique information
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static TChild DownCast<TParent, TChild>(TParent parentObj)
        {
            string output = JsonConvert.SerializeObject(parentObj);
            return JsonConvert.DeserializeObject<TChild>(output);
        }

        public static TParent UpCast<TParent, TChild>(TChild childObj)
        {
            string output = JsonConvert.SerializeObject(childObj);
            return JsonConvert.DeserializeObject<TParent>(output);
        }
    }

    public static class EnumExt
    {
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            var memberInfo = enumValue.GetType()
                                      .GetMember(enumValue.ToString())
                                      .FirstOrDefault();
            if (memberInfo == null) return null;

            return (T)memberInfo.GetCustomAttributes(typeof(T), false)
                                .FirstOrDefault();
        }

        public static string GetDisplayName(this Enum enumValue) => GetAttribute<DisplayAttribute>(enumValue)?.Name ?? enumValue.ToString();
    }

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