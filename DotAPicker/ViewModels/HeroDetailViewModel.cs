using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using DotAPicker.ViewModels;
using DotAPicker.Utilities;

namespace DotAPicker.Models
{
    public class HeroDetailViewModel
    {
        public Hero Hero { get; set; }
        public IEnumerable<Tip> Tips { get; set; }
        public IEnumerable<RelationshipViewModel> Relationships { get; set; }
        //public string HeroPortrait => GetPortrait(Hero.Name); //we don't preload so that the file isn't access if it's never used

        public HeroDetailViewModel() { }

        public HeroDetailViewModel(int heroID, DotADB db)
        {
            Hero = db.Heroes.FirstOrDefault(h => h.ID == heroID);
            FillTipsAndRelationships(db);
        }

        public HeroDetailViewModel(Hero hero, DotADB db)
        {
            Hero = hero;
            FillTipsAndRelationships(db);
        }

        /// <summary>
        /// I just didn't know what the hell else to call it
        /// </summary>
        /// <param name="db"></param>
        public void FillTipsAndRelationships(DotADB db)
        {
            Tips = db.Tips.Where(t => t.HeroID == Hero.ID);
            if (!db.Settings.ShowDeprecatedTips)
            {
                Tips = Tips.Where(t => !t.Deprecated);
            }

            Relationships = db.Relationships.Where(r => r.IncludesHero(Hero.ID))
                                            .Select(r => Casting.DownCast<Relationship, RelationshipViewModel>(r)
                                                                .FillRelationships(db));

            if (!db.Settings.ShowDeprecatedRelationships)
            {
                Relationships = Relationships.Where(r => !r.Deprecated);
            }
        }

        //public string GetPortrait(int heroID, DotADB db)
        //{
        //    string heroName = db.Heroes.First(h => h.ID == heroID).Name;
        //    return GetPortrait(heroName);
        //}

        //public string GetPortrait(string heroName)
        //{
        //    var imgPath = GetImgName(heroName);
        //    var base64 = Convert.ToBase64String(File.ReadAllBytes(imgPath));
        //    var imgSrc = String.Format($"data:image/png;base64,{base64}");
        //    return imgSrc;
        //}

        public string GetImgName(string heroName)
        {
            string working = heroName.Replace(' ', '_');
            return $"img/hero/120px-{working}_icon.png";
        }

        public string GetImgName() => GetImgName(Hero.Name);
    }
}