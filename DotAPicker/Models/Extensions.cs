using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Reflection;
using System.ComponentModel;

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
            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static)
                                   .Where(m => m.MemberType == MemberTypes.Field);
            return members.Select(val => new SelectListItem() {
                Text = val.DisplayName(),
                Value = val.Name,
                Selected = (strEnum == val.Name)
            });
        }

        public static IEnumerable<SelectListItem> GetSelectList(Enum enumVal)
        {
            var strEnum = enumVal.ToString();
            var members = enumVal.GetType().GetMembers(BindingFlags.Public | BindingFlags.Static)
                                           .Where(m => m.MemberType == MemberTypes.Field);
            return members.Select(val => new SelectListItem() {
                Text = val.DisplayName(),
                Value = val.Name,
                Selected = (strEnum == val.Name)
            });
        }

        public static string DisplayName(this MemberInfo member) => member.GetCustomAttribute<DisplayAttribute>()?.Name ?? member.Name;

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

            return members.Select(m => m.GetCustomAttribute<DisplayAttribute>()?.Name ?? m.Name);
        }
    }
}