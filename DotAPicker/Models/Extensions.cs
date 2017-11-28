using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Reflection;

namespace DotAPicker.Models
{
    public static class Extensions
    {
        public static IEnumerable<SelectListItem> AsSelectList<T>(this T enumVal) where T: struct
        {
            //since we cant do a generic type constraint
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }

            var strEnum = enumVal.ToString();
            return Enum.GetNames(typeof(T))
                       .Select(val => new SelectListItem() {
                           Text = val,
                           Value = val,
                           Selected = (strEnum == val)
                       });
        }

        public static IEnumerable<string> GetEnumOptions(Type enumType, Type affiliationType)
        {
            //since we cant do a generic type constraint
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }

            string sAfType = affiliationType.Name;
            return enumType.GetMembers(BindingFlags.Public|BindingFlags.Static)
                           .Where(m => m.MemberType == MemberTypes.Field)
                           .Where(m => (m.GetCustomAttribute<ObjectAffiliation>()?.TypeName ?? sAfType) == sAfType)
                           .Select(m => m.Name);
        }
    }
}