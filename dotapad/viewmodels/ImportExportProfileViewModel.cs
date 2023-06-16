using System;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class ExportProfileViewModel
    {
        [Display(Name = "Include Notes")]
        public bool IncludeNotes { get; set; } = false; //this has to default to false, because false checkboxes don't return any value at all
    }

    public class ImportProfileViewModel: ExportProfileViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File to Upload")]
        public IFormFile? PostedFile { get; set; }
    }

}