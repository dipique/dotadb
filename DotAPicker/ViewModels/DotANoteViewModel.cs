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
    }
}