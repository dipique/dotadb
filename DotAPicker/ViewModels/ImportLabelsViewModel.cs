using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class ImportLabelsViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File to Upload")]
        public HttpPostedFileBase PostedFile { get; set; }
    }

}