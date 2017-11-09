﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Relationship>().HasRequired(f => f.Hero1)
            //                                   .WithOptional()
            //                                   .WillCascadeOnDelete(false);
            //modelBuilder.Entity<Relationship>().HasRequired(f => f.Hero2)
            //                                   .WithOptional()
            //                                   .WillCascadeOnDelete(false);
            //modelBuilder.Entity<Tip>().HasRequired(t => t.User)
            //                          .WithRequiredDependent()
            //                          .WillCascadeOnDelete(false);
        }

    }

    public class DotAInitializer : DropCreateDatabaseAlways<DotAContext> // DropCreateDatabaseIfModelChanges<DotAContext>
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
                new Hero { UserID = defaultID, Name = "Dazzle", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Drow Ranger", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Earthshaker", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Lich", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Lina", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Lion", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Mirana", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Morphling", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Necrophos", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Puck", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Pudge", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Razor", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Sand King", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Storm Spirit", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Tidehunter", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Vengeful Spirit", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Windranger", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Witch Doctor", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Slardar", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Enigma", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Faceless Void", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Tiny", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Venomancer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Clockwerk", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Dark Seer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Sniper", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Pugna", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Beastmaster", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Enchantress", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Leshrac", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Shadow Fiend", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Weaver", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Night Stalker", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Spectre", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Doom", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Bloodseeker", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Kunkka", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Riki", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Broodmother", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Jakiro", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Batrider", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Dragon Knight", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Warlock", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Lifestealer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Death Prophet", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Ursa", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Bounty Hunter", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Silencer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Clinkz", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Outworld Devourer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Bane", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Shadow Demon", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Lycan", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Phantom Lancer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Treant Protector", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Gyrocopter", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Chaos Knight", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Phantom Assassin", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Rubick", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Luna", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Io", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Undying", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Disruptor", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Nyx Assassin", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Keeper of the Light", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Visage", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Magnus", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Centaur Warrunner", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Slark", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Timbersaw", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Medusa", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Troll Warlord", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Skywrath Mage", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Ember Spirit", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Legion Commander", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Terrorblade", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Techies", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Oracle", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Winter Wyvern", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Underlord", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Monkey King", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Anti-Mage", AltNames = "Antimage AM", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Ogre Magi", AltNames = "om", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Spirit Breaker", AltNames = "sb barafu", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Sven", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Chen", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserID = defaultID, Name = "Crystal Maiden", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Bristleback", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Invoker", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserID = defaultID, Name = "Juggernaut", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Omniknight", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Viper", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Axe", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserID = defaultID, Name = "Brewmaster", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserID = defaultID, Name = "Elder Titan", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserID = defaultID, Name = "Naga Siren", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserID = defaultID, Name = "Meepo", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserID = defaultID, Name = "Wraith King", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Zeus", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Tinker", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserID = defaultID, Name = "Tusk", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserID = defaultID, Name = "Templar Assassin", AltNames = "ta, lanaya", Notes = "TA is a mid hero; she benefits a lot from early levels and needs them to get kills so she can get deso/blink and snowball. Absent early farm she can be very underwhelming.", Preference = HeroPreference.Favorite },
                new Hero { UserID = defaultID, Name = "Lone Druid", AltNames = "LD", Notes = "LD can be played as a bear-focused hero or as a sniper-style damage dealer.", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Ancient Apparition", AltNames = "aa", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Arc Warden", AltNames = "aw", Notes = "I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! ", Preference = HeroPreference.Hated },
                new Hero { UserID = defaultID, Name = "Queen of Pain", AltNames = "QoP", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserID = defaultID, Name = "Phoenix", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Earth Spirit", AltNames = "", Notes = "Preferred because I think I'll like him a lot more when I suck less", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Nature's Prophet", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Huskar", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Shadow Shaman", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserID = defaultID, Name = "Alchemist", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserID = defaultID, Name = "Abaddon", AltNames = "", Notes = "Abaddon is versatile and can be played as core or support. I prefer playing him as a support, maxing aphotic shield so my carry is unkillable.", Preference = HeroPreference.Preferred }
            };
            heroes.ForEach(u => db.Heroes.Add(u));
            db.SaveChanges();
            //heroes.ForEach(t => db.Entry(t).State = EntityState.Detached);

            var tips = new List<Tip>{
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Bloodseeker").ID, Type = TipType.Other, Text = "Use bloodrage on both you and a squishy enemy for max damage", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Bane").ID, Type = TipType.Strategy, Text = "Nightmare supports to keep them from counter-initiating, then use fiend's grip", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.AbilityBuild, Text = "Max psi blades first for maximum lane push, or max refraction for best defense. Either way these are the best two abilities.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.Other, Text = "Stack ancients starting early; as many stacks as possible. She can take them at level 8/9 with shrine, or earlier with DD rune.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.Other, Text = "TA does burst physical damage, so take out squishy heros in team fights first before they can react.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Axe").ID, Type = TipType.Other, Text = "Max counter helix. The pure damage is incredibly powerful early and can be used as a farming tool, defensively, and offensively.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Terrorblade").ID, Type = TipType.AbilityUse, Text = "Reflection is very good against 5-core right clicking lineups (especially squishy ones)", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Legion Commander").ID, Type = TipType.ItemBuild, Text = "Using press the attack the gain health is more gold efficient (via clarities) than tangos/salve, providing you get the full duration. Salve/clarity is a good option. In a lane with any degree of harass, this is a bad idea.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Shadow Demon").ID, Type = TipType.AbilityUse, Text = "Shadow poison stacks do an absurd amount of damage early.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.Counter, Text = "Abilities that reveal meld position: Defeaning blast, tornado, gust, flame break, charge of darkness", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.AbilityUse, Text = "Psionic trap: If you hit someone with a full charged trap (the trap charges over 4s to increase the slow % from 30 to 60) and before the slow ends you place and immediately activate a new trap then you refresh the duration but it keeps the maximum slow (60%) instead of resetting it to 30%. Works the other way around, too (giving a worse slow from a fully charged trap).", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.Counter, Text = "Orb of venom deals low enough damage to not proc her refraction block but deals just enough to trigger her blink's cooldown. perfect for preventing her blinking out while she has charges up", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Spectre").ID, Type = TipType.Counter, Text = "Silver edge to break passives", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Broodmother").ID, Type = TipType.Strategy, Text = "Pick into lineups without good AOE/cleave", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Morphling").ID, Type = TipType.Strategy, Text = "Pick into lineups with much lockdown", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.Other, Text = "TA stays invis during meld attack until the project attack, meaning you can blink away from the meld attack.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.AbilityUse, Text = "Use meld to disjoint projectile stuns", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = TipType.AbilityUse, Text = "Against Tinker, hold your meld until you see laser precast animation. Meld to cancel, then you get another attack or two to get the kill.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.Strategy, Text = "Take offlane. Scales well with items.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "Pause between fire spirits because they don't stack. With dps talent, these can do 2k+ dmg.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "Sunray allows crossing impassible terrain by toggling movement", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "Huge power spike at level 3 because fire spirits at level 2 more than double in damage", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.ItemBuild, Text = "Go Wand->Tranquils->Midas->Veil because he doesn't really want to farm", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.Other, Text = "Save wand charges to guarantee mana for ult if possible", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityBuild, Text = "go 2-2 first, then 2-2-1-1, max fire spirits and sunray prioritizing ult and XP talent", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.ItemBuild, Text = "After veil, buy an orchid. Really.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.ItemBuild, Text = "If an AS threat to the egg is an issue (troll, huskar, etc.), buy a halberd", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.Strategy, Text = "Counters channelled magic ults like CM as egg is spell immune", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "Use egg in the back half of team fights, not for initiation or when people can easily disengage. Pheonix punishes overcommitment and bad positioning.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "Team fight skill sequence: icarus dive to apply debuff. During the dive, use fire spirits and veil. Finally, ult. Might happen quickly because of ult duration.", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "If you are slowed below 250, sunray will actually increase your move speed", Patch = "7.07b" },
                new Tip { UserID = defaultID, HeroID = heroes.First(h => h.Name == "Phoenix").ID, Type = TipType.AbilityUse, Text = "For escapes, you can sunray, toggle movement on, and TP while you're actually off the map", Patch = "7.07b" }
            };
            tips.ForEach(u => db.Tips.Add(u));
            db.SaveChanges();
            //tips.ForEach(t => db.Entry(t).State = EntityState.Detached);

            var relationships = new List<Relationship>{
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Anti-Mage").ID, Hero2ID = heroes.First(h => h.Name == "Bloodseeker").ID, Type = RelationshipType.Counter, Description = "BS can use rupture to make it difficult for AM to flee", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Alchemist").ID, Hero2ID = heroes.First(h => h.Name == "Anti-Mage").ID, Type = RelationshipType.Synergy, Description = "Antimage benefits a lot from an aghs that doesn't take up an item slot", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Batrider").ID, Hero2ID = heroes.First(h => h.Name == "Enigma").ID, Type = RelationshipType.Other, Description = "BKB piercing lasso ends black hole", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Axe").ID, Hero2ID = heroes.First(h => h.Name == "Nature's Prophet").ID, Type = RelationshipType.Counter, Description = "Can use taunt to cancel TP", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Templar Assassin").ID, Hero2ID = heroes.First(h => h.Name == "Chaos Knight").ID, Type = RelationshipType.Counter, Description = "TA doesn't fair well against illusion heroes that can break through diffraction", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Templar Assassin").ID, Hero2ID = heroes.First(h => h.Name == "Phantom Lancer").ID, Type = RelationshipType.Counter, Description = "TA doesn't fair well against illusion heroes that can break through diffraction", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Templar Assassin").ID, Hero2ID = heroes.First(h => h.Name == "Enigma").ID, Type = RelationshipType.Counter, Description = "Eidelons can break through diffraction and black hole stops her from doing damage.", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Templar Assassin").ID, Hero2ID = heroes.First(h => h.Name == "Shadow Shaman").ID, Type = RelationshipType.Synergy, Description = "SS can disable a hero while TA does massive amounts of damage", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Templar Assassin").ID, Hero2ID = heroes.First(h => h.Name == "Enigma").ID, Type = RelationshipType.Synergy, Description = "Black hole can group people together to receive lots of damage from psi blades", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Sven").ID, Hero2ID = heroes.First(h => h.Name == "Enigma").ID, Type = RelationshipType.Synergy, Description = "Black hole can group people together to receive lots of damage from cleave", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Warlock").ID, Hero2ID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = RelationshipType.Synergy, Description = "Fatal bonds spreads TA's psi blade damage even farther", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Abaddon").ID, Hero2ID = heroes.First(h => h.Name == "Lina").ID, Type = RelationshipType.Counter, Description = "Lina can burst through abaddon without triggering ult if timed correctly", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Phantom Lancer").ID, Hero2ID = heroes.First(h => h.Name == "Wraith King").ID, Type = RelationshipType.Other, Description = "PL's mana burn makes it so WK doesn't have the mana to reincarnate", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Anti-Mage").ID, Hero2ID = heroes.First(h => h.Name == "Wraith King").ID, Type = RelationshipType.Counter, Description = "AM's mana burn makes it so WK doesn't have the mana to reincarnate", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Bloodseeker").ID, Hero2ID = heroes.First(h => h.Name == "Storm Spirit").ID, Type = RelationshipType.Counter, Description = "Rupture makes it difficult for SS to ball lightning away", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Bristleback").ID, Hero2ID = heroes.First(h => h.Name == "Riki").ID, Type = RelationshipType.Counter, Description = "Riki gives more damage from behind, bb takes reduced damage from behind", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Silencer").ID, Hero2ID = heroes.First(h => h.Name == "Legion Commander").ID, Type = RelationshipType.Synergy, Description = "Silencer can prevent LC from being disabled during duel", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Batrider").ID, Hero2ID = heroes.First(h => h.Name == "Legion Commander").ID, Type = RelationshipType.Synergy, Description = "Batrider can lasso enemies into a duel", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Faceless Void").ID, Hero2ID = heroes.First(h => h.Name == "Lina").ID, Type = RelationshipType.Synergy, Description = "Lina can do a lot of damage from outside the chronosphere", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Venomancer").ID, Hero2ID = heroes.First(h => h.Name == "Templar Assassin").ID, Type = RelationshipType.Counter, Description = "Counters during laning stage with DoT AND wards breaking through dispersion", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Phoenix").ID, Hero2ID = heroes.First(h => h.Name == "Slark").ID, Type = RelationshipType.Counter, Description = "Can escape tether with dive", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Phoenix").ID, Hero2ID = heroes.First(h => h.Name == "Disruptor").ID, Type = RelationshipType.Other, Description = "Can escape kinetic field with dive", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Meepo").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Counter, Description = "Meepo clones count toward killing egg", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Arc Warden").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Other, Description = "Tempest double counts toward killing egg", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Lone Druid").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Other, Description = "Bear counts toward killing egg", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Shadow Shaman").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Other, Description = "Can keep phoenix down and keep him from escaping or ulting", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Lion").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Other, Description = "Can disable phoenix and prevent him from escaping or ulting, and burst him down with ult", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Silencer").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Counter, Description = "Phoenix doesn't do shit if silenced", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Juggernaut").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Counter, Description = "High attack speed kills eggs, has spell immunity", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Lifestealer").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Counter, Description = "Has escapes from damage (ult and spell immunity), plus massive % based physical damage", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Ursa").ID, Hero2ID = heroes.First(h => h.Name == "Phoenix").ID, Type = RelationshipType.Counter, Description = "Rapid egg kills, lots of burst physical damage, ult can tank your burst magical damage", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Bloodseeker").ID, Hero2ID = heroes.First(h => h.Name == "Spirit Breaker").ID, Type = RelationshipType.Counter, Description = "Rupture prevents SB from charging away", Patch = "7.07b" },
                new Relationship { UserID = defaultID, Hero1ID = heroes.First(h => h.Name == "Lion").ID, Hero2ID = heroes.First(h => h.Name == "Chaos Knight").ID, Type = RelationshipType.Counter, Description = "Mana drain destroys illusions and talent places it on multiple targets. Q deals with illusions as well as Aghs for AOE ult.", Patch = "7.07b" }
            };
            relationships.ForEach(r => db.Relationships.Add(r));
            db.SaveChanges();
        }
    }
}