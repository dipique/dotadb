using System;

using DotAPicker.Models;

namespace DotAPicker.ViewModels
{
    public class HeroViewModel
    {
        public bool SignedIn { get; set; } = false;

        public static string GetImgName(string heroName)
        {
            string working = string.IsNullOrEmpty(heroName) ? MISSING_IMG_STRING : heroName.Replace(' ', '_');
            return $"img/hero/120px-{working}_icon";
        }

        public string GetImgName() => GetImgName(Hero?.Name ?? string.Empty);
        public string GetImgFilename() => GetImgFilename(Hero?.Name ?? string.Empty);
        public static string GetImgFilename(string heroName) => Path.Combine(AppContext.BaseDirectory, "img", GetImgName(heroName), ".png");

        private const string MISSING_IMG_STRING = "Missing";
        public static string MissingImgName => GetImgName(MISSING_IMG_STRING);

        public bool MissingImage => !File.Exists(GetImgFilename());

        public Hero Hero { get; set; }

        public HeroViewModel() { Hero = new Hero(); }
        public HeroViewModel(Hero hero, bool signedIn = false)
        {
            Hero = hero;
            SignedIn = signedIn;
        }
    }
}