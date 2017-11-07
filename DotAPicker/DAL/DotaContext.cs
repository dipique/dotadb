using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

using DotAPicker.Models;
using DotAPicker.Utilities;



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
        
        /// <summary>
        /// TODO: Return errors if found
        /// </summary>
        /// <param name="currentUserID"></param>
        /// <param name="newSettings"></param>
        public void UpdateSettings(int currentUserID, List<Setting> newSettings)
        {
            var currentSettings = Users.Find(currentUserID).Settings;
            var saveNeeded = false;

            //loop through all settings, updating as appropriate
            foreach (Setting oldVal in currentSettings)
            {
                //get new value for comparison
                var newVal = newSettings.FirstOrDefault(s => s.ID == oldVal.ID);
                if (newVal == null) continue;

                //if it's the same, move on
                if (newVal.Value == oldVal.Value) continue;

                //validate the setting
                var validationMethod = typeof(SettingValidator).GetMethods()
                                                               .FirstOrDefault(m => m.GetCustomAttribute<SettingValidator>()?.SettingName == oldVal.Name);
                if (validationMethod != null)
                {
                    var success = (bool)validationMethod.Invoke(null, new object[] { newVal.Value });
                    if (!success) continue; //failed validation, don't make update (TODO: send error messages to screen)
                }

                //make the update
                oldVal.Value = newVal.Value;
                Entry(oldVal).State = EntityState.Modified;
                saveNeeded = true;
            }

            //save if needed
            if (saveNeeded) SaveChanges();
        }

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
                new Setting { UserID = 0, Name = "CurrentPatch", Value = "7.07b" },
                new Setting { UserID = 0, Name = "ShowDeprecatedTips", Value = "False" },
                new Setting { UserID = 0, Name = "ShowDeprecatedRelationships", Value = "False" },
                new Setting { UserID = 0, Name = "Labels", Value = "Pusher|Nuker|Support|Disabler|Pure Damage" }
            };
            settings.ForEach(s => db.Settings.Add(s));
            db.SaveChanges();
        }
    }
}