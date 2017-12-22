using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

using DotAPicker.Models;
using DotAPicker.Utilities;

namespace DotAPicker.ViewModels
{
    public class TableViewModel<T> where T: UserOwnedEntity
    {
        private IEnumerable<T> items = new List<T>();
        public IEnumerable<T> Items
        {
            get
            {
                if (SortProperty == null) return items;

                var selector = (Func<T, object>)BuildSelector().Compile();

                if (SortDirection == SortDirections.Ascending)
                {
                    return items.OrderBy(selector);
                }

                //if it's not ascending, it's descening
                return items.OrderByDescending(selector);
            }
            set => items = value;
        }
        public SortDirections SortDirection { get; set; } = SortDirections.Ascending;
        public string SortField { get; set; }

        public LambdaExpression BuildSelector()
        {
            ParameterExpression param = Expression.Parameter(typeof(T));
            MemberExpression access = Expression.MakeMemberAccess(param, SortProperty);
            LambdaExpression lambda = Expression.Lambda<Func<T, object>>(access, param);
            return lambda;   
        }

        private PropertyInfo SortProperty => typeof(T).GetProperty(SortField);

        private int selectedItemId = -1;
        public int SelectedItemId
        {
            get
            {
                if (selectedItemId != -1) return selectedItemId;

                if (!items.Any()) return -1;
                return Items.First().Id;
            }
            set => selectedItemId = value;
        }
    }

    public enum SortDirections
    {
        Ascending,
        Descending,
        None
    }

    public class TableConstructorViewModel<T>
    {
        public bool SignedIn { get; set; }
        public IEnumerable<T> Items { get; set; }
        public List<TableColumn<T>> Columns { get; set; }
    }

    public class TableColumn<T>
    {
        public string Name => Property?.DisplayName() ?? Property?.Name;
        public SortDirections SortDirection { get; set; } = SortDirections.None;
        public bool Display { get; set; } = true;
        public int MinWidth { get; set; } = -1;
        public PropertyInfo Property { get; set; }
        public DropDownSettings DropDownSettings { get; set; }
        public string ValueID { get; set; }

        public bool IsDropDown => DropDownSettings != null;

        public string GetValue(T item)
        {
            if (Property == null) return string.Empty;

            return Property.GetValue(item).ToString();
        }

        public object ObjectValue(T item)
        {
            if (Property == null) return null;
            return Property.GetValue(item);
        }

        public TableColumn(string propertyName, string valueID = null, bool display = true, int minWidth = -1, DropDownSettings dropDownSettings = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                Property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(p => p.Name == propertyName);
            }

            ValueID = valueID;
            Display = display;
            MinWidth = minWidth;
            DropDownSettings = dropDownSettings;
        }

        private string styleAttributes = null;
        public string StyleAttributes
        {
            get
            {
                if (styleAttributes == null) styleAttributes = GetStyleAttributeString();
                return styleAttributes;
            }
            set => styleAttributes = value;
        }

        public string IDString
        {
            get
            {
                if (ValueID == null) return string.Empty;

                return $" id=\"{ValueID}\"";
            }
        }

        private string GetStyleAttributeString()
        {
            //Assemble style attributes from the column metadata
            List<string> styleAttributesList = new List<string>();
            if (MinWidth != -1)
            {
                styleAttributesList.Add($"min-width: {MinWidth}px;");
            }
            if (!Display)
            {
                styleAttributesList.Add("display:none;");
            }
            
            if (styleAttributesList.Count() == 0) return string.Empty;

            //Create HTML string from attributes
            return $" style=\"{string.Join(" ", styleAttributesList)}\"";
        }
    }

    public class DropDownSettings
    {
        public bool RequiresSignIn { get; set; } = true;
        public string DropdownClass { get; set; }
    }

}