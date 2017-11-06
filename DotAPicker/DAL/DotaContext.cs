using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotAPicker.Models;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DotAPicker.DAL
{
    public class DotAContext: DbContext
    {
        public DotAContext(): base(nameof(DotAContext))
        {

        }

        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<HeroLabel> HeroLabels { get; set; }
        public DbSet<User> Users { get; set; }
        

    }

    public class DotAInitializer : DropCreateDatabaseIfModelChanges<DotAContext>
    {
        protected override void Seed(DotAContext db)
        {
            var users = new List<User> {
                new User { Username = User.DEFAULT_USER },
            };
            users.ForEach(u => db.Users.Add(u));
            db.SaveChanges();

            var heroes = new List<Hero> {
                new Hero { UserID = 0, Name = "Abaddon", Notes = "test notes" }
            };
            users.ForEach(u => db.Users.Add(u));
            db.SaveChanges();

            var settings = new List<Setting> {
                new Setting { UserID = 0, Name = "CurrentPatch", Value = "7.07" },
                new Setting { UserID = 0, Name = "ShowDeprecatedTips", Value = "False" },
                new Setting { UserID = 0, Name = "ShowDeprecatedRelationships", Value = "False" },
                new Setting { UserID = 0, Name = "Labels", Value = "Pusher|Nuker|Support|Disabler|Pure Damage" }
            };
            settings.ForEach(s => db.Settings.Add(s));
            db.SaveChanges();
        }
    }
}