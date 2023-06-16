using System;
using System.ComponentModel.DataAnnotations;

namespace DotAPicker.ViewModels
{
    public class UploadHeroImageViewModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "PNG to Upload")]
        public IFormFile? PostedFile { get; set; }

        [Required]
        public int HeroID { get; set; } = -1;

        public UploadHeroImageViewModel(int heroID)
        {
            HeroID = heroID;
        }

        public UploadHeroImageViewModel() { }
    }

}