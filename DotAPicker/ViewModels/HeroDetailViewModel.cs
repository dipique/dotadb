using System;
using System.Collections.Generic;
using System.Linq;

using DotAPicker.ViewModels;
using DotAPicker.Utilities;

namespace DotAPicker.Models
{
    public class HeroDetailViewModel
    {
        public Hero Hero { get; set; }
        public IEnumerable<Tip> Tips { get; set; }
        public IEnumerable<RelationshipViewModel> Relationships { get; set; }

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
            Relationships = db.Relationships.Where(r => r.IncludesHero(Hero.ID))
                                            .Select(r => Casting.DownCast<Relationship, RelationshipViewModel>(r)
                                                                .FillRelationships(db));
        }
    }
}