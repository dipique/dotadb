using System;
using System.IO;
using System.Linq;
using System.Web;

using DotAPicker.Models;

namespace DotAPicker.ViewModels
{
    public class HeroViewModel
    {
        public bool SignedIn { get; set; } = false;

        public static string GetImgName(string heroName)
        {
            string working = heroName.Replace(' ', '_');
            return $"img/hero/120px-{working}_icon.png";
        }

        public string GetImgName() => GetImgName(Hero?.Name);
        public string GetImgFilename() => GetImgFilename(Hero?.Name);
        public static string GetImgFilename(string heroName) => HttpContext.Current.Server.MapPath($"~/{GetImgName(heroName)}");

        private const string MISSING_IMG_STRING = "Missing";
        public static string MissingImgName => GetImgName(MISSING_IMG_STRING);

        public bool MissingImage => !File.Exists(GetImgFilename());

        public Hero Hero { get; set; }

        public HeroViewModel() { }
        public HeroViewModel(Hero hero, bool signedIn = false)
        {
            Hero = hero;
            SignedIn = signedIn;
        }
    }
}