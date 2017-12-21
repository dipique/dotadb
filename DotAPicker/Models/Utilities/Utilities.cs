using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DotAPicker.Utilities
{
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

        public static T Parse<T>(string input) where T : struct
        {
            //since we cant do a generic type constraint
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }
            if (!string.IsNullOrEmpty(input))
            {
                if (Enum.GetNames(typeof(T)).Any(
                      e => e.Trim().ToUpperInvariant() == input.Trim().ToUpperInvariant()))
                {
                    return (T)Enum.Parse(typeof(T), input, true);
                }
            }
            return default(T);
        }
    }
}