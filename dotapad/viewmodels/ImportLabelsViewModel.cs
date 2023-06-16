using System;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class ImportLabelsViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File to Upload")]
        public IFormFile? PostedFile { get; set; }
    }

}