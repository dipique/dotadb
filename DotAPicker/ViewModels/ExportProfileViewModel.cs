using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class ExportProfileViewModel
    {
        [Display(Name = "Include Notes")]
        public bool IncludeNotes { get; set; } = true;
    }
}