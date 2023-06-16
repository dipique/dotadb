using System;
using System.Linq.Expressions;
using System.Reflection;

using DotAPicker.Models;

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

                //if it's not ascending, it's descending
                return items.OrderByDescending(selector);
            }
            set => items = value;
        }
        public SortDirections SortDirection { get; set; } = SortDirections.Ascending;
        public string SortField { get; set; } = string.Empty;

        public LambdaExpression BuildSelector()
        {
            var param = Expression.Parameter(typeof(T));
            var access = Expression.MakeMemberAccess(param, SortProperty);

            //usually, this access expression would be enough to create the lambda expression. HOWEVER,
            //for some stupid reason member access expressions DO NOT like enums, so we skirt around that limitation
            //by converting all the values to string
            var toStringMethod = typeof(object).GetMethods().Where(m => m.GetParameters().Length == 0)
                                                            .Single(m => m.Name == "ToString");
            var toString = Expression.Call(access, toStringMethod);

            //that done, we can assemble the lambda expression
            LambdaExpression lambda = Expression.Lambda<Func<T, object>>(toString, param);
            return lambda;   
        }

        private PropertyInfo SortProperty => typeof(T).GetProperty(SortField) ?? throw new Exception("Invalid sort field");

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
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public List<TableColumn<T>> Columns { get; set; } = new();
    }

    public class TableColumn<T>
    {
        private string headerText =  string.Empty;
        public string HeaderText
        {
            get => headerText ?? Name;
            set => headerText = value;
        }
        public string Name => Property?.DisplayName() ?? Property?.Name ?? string.Empty;
        public SortDirections SortDirection { get; set; } = SortDirections.None;
        public bool Display { get; set; } = true;
        public int MinWidth { get; set; } = -1;
        public PropertyInfo? Property { get; set; }
        public DropDownSettings? DropDownSettings { get; set; }
        public string ValueID { get; set; }
        public string HeaderID { get; set; }
        public bool Sortable { get; set; } = true;
        public string RawHTML { get; set; } //sometimes, instead of values, you might just want raw HTML inserted there

        public bool IsDropDown => DropDownSettings != null;

        public string GetValue(T item) => Property == null ? string.Empty : Property.GetValue(item)?.ToString() ?? string.Empty;
 
        public object? ObjectValue(T item)
        {
            if (Property == null) return null;
            return Property.GetValue(item);
        }

        public TableColumn(string propertyName, string valueID = "", bool display = true, int minWidth = -1, DropDownSettings? dropDownSettings = null, bool sortable = true, string headerID = "", string rawHTML = "", string header = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                Property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(p => p.Name == propertyName);
            }

            ValueID = valueID;
            Display = display;
            Sortable = sortable && display && rawHTML == null; //invisible fields can't be sortable, and rawHTML can't be sorted
            MinWidth = minWidth;
            DropDownSettings = dropDownSettings;
            HeaderID = headerID ?? propertyName;
            RawHTML = rawHTML ?? string.Empty;
            HeaderText = header;
        }

        private string? styleAttributes = null;
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
            List<string> styleAttributesList = new();
            if (MinWidth != -1)
            {
                styleAttributesList.Add($"min-width: {MinWidth}px;");
            }
            if (!Display)
            {
                styleAttributesList.Add("display:none;");
            }
            
            if (styleAttributesList.Count == 0) return string.Empty;

            //Create HTML string from attributes
            return $" style=\"{string.Join(" ", styleAttributesList)}\"";
        }
    }

    public class DropDownSettings
    {
        public bool RequiresSignIn { get; set; } = true;
        public string DropdownClass { get; set; } = string.Empty;
    }

}