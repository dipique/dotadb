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

        public static IEnumerable<string> GetEnumOptions(Type enumType, Type affiliationType = null)
        {
            //since we cant do a generic type constraint
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }

            var members = enumType.GetMembers(BindingFlags.Public | BindingFlags.Static)
                                  .Where(m => m.MemberType == MemberTypes.Field);

            //only filter by affiliation if an affiliation type was provided.
            if (affiliationType != null)
            {
                members = members.Where(m => (m.GetCustomAttribute<ObjectAffiliation>()?.TypeName ?? affiliationType.Name) == affiliationType.Name);
            }

            return members.Select(m => m.Name);
        }
    }
}