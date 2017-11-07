using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
            Database.Log = s => Debug.WriteLine(s);
        }

        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        //public DbSet<Setting> Settings { get; set; }
        public DbSet<HeroLabel> HeroLabels { get; set; }
        public DbSet<User> Users { get; set; }
        
        ///// <summary>
        ///// Updates the settings objects from a list of settings. These are updates as lists and not as individual objects.
        ///// TODO: Return errors if found
        ///// </summary>
        ///// <param name="currentUserID"></param>
        ///// <param name="newSettings"></param>
        //public void UpdateSettings(int currentUserID, List<Setting> newSettings)
        //{
        //    var currentSettings = Users.Find(currentUserID).Settings;
        //    var saveNeeded = false;

        //    //loop through all settings, updating as appropriate
        //    foreach (Setting oldVal in currentSettings)
        //    {
        //        //get new value for comparison
        //        var newVal = newSettings.FirstOrDefault(s => s.ID == oldVal.ID);
        //        if (newVal == null) continue;

        //        //if it's the same, move on
        //        if (newVal.Value == oldVal.Value) continue;

        //        //validate the setting
        //        var validationMethod = typeof(SettingValidator).GetMethods()
        //                                                       .FirstOrDefault(m => m.GetCustomAttribute<SettingValidator>()?.SettingName == oldVal.Name);
        //        if (validationMethod != null)
        //        {
        //            var success = (bool)validationMethod.Invoke(null, new object[] { newVal.Value });
        //            if (!success) continue; //failed validation, don't make update (TODO: send error messages to screen)
        //        }

        //        //make the update
        //        oldVal.Value = newVal.Value;
        //        Entry(oldVal).State = EntityState.Modified;
        //        saveNeeded = true;
        //    }

        //    //save if needed
        //    if (saveNeeded) SaveChanges();
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Relationship>().HasRequired(f => f.Hero1)
                                               .WithOptional()
                                               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Relationship>().HasRequired(f => f.Hero2)
                                               .WithOptional()
                                               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Tip>().HasRequired(t => t.User)
                                      .WithRequiredDependent()
                                      .WillCascadeOnDelete(false);
            //modelBuilder.Entity<Setting>().HasRequired(s => s.User)
            //                              .WithRequiredDependent()
            //                              .WillCascadeOnDelete(false);
        }

    }

    public class DotAInitializer : DropCreateDatabaseIfModelChanges<DotAContext>
    {
        protected override void Seed(DotAContext db)
        {
            var users = new List<User> {
                new User { Username = User.DEFAULT_USER,
                           CurrentPatch = "7.07b",
                           ShowDeprecatedRelationships = false,
                           ShowDeprecatedTips = false,
                           LabelOptions = "Pusher|Nuker|Support|Disabler|Pure Damage"
                },
            };
            users.ForEach(u => db.Users.Add(u));
            db.SaveChanges();
            var defaultID = db.Users.First().ID;

            var heroes = new List<Hero> {
                new Hero { UserID = defaultID, Name = "Abaddon", Notes = "test notes" }
            };
            heroes.ForEach(u => db.Heroes.Add(u));
            db.SaveChanges();

            //var settings = new List<Setting> {
            //    new Setting { UserID = defaultID, Name = "CurrentPatch", Value = "7.07b" },
            //    new Setting { UserID = defaultID, Name = "ShowDeprecatedTips", Value = "False" },
            //    new Setting { UserID = defaultID, Name = "ShowDeprecatedRelationships", Value = "False" },
            //    new Setting { UserID = defaultID, Name = "Labels", Value = "Pusher|Nuker|Support|Disabler|Pure Damage" }
            //};
            //settings.ForEach(s => db.Settings.Add(s));
            //db.SaveChanges();
        }
    }
}