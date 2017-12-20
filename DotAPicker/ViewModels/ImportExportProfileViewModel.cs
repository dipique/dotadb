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

    public class ImportProfileViewModel: ExportProfileViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File to Upload")]
        public HttpPostedFileBase PostedFile { get; set; }
    }

}