using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class UploadHeroImageViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "PNG to Upload")]
        public HttpPostedFileBase PostedFile { get; set; }

        [Required]
        public int HeroID { get; set; } = -1;

        public UploadHeroImageViewModel(int heroID)
        {
            HeroID = heroID;
        }

        public UploadHeroImageViewModel() { }
    }

}