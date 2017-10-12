using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection;
using System.ComponentModel.DataAnnotations;

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
}