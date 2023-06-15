using DotAPicker.Models;
using System;
using System.Collections.Generic;

namespace DotAPicker.ViewModels
{
    public class DotANoteViewModel<T> where T:DotANote
    {
        public DotANoteViewModel(List<T> notes, string currentPatch)
        {
            Notes = notes;
            CurrentPatch = currentPatch;
        }

        public List<T> Notes { get; set; }
        public string CurrentPatch { get; set; }
        public string TypeName => typeof(T).Name;
        public Type Type => typeof(T);

        public string GetName(bool plural = false, bool lowerCase = false)
        {
            var n = TypeName;
            if (plural)
                n = $"{n}s";
            if (lowerCase)
                n = n.ToLowerInvariant();
            return n;
        }
    }
}