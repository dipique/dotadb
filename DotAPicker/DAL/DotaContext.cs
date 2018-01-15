using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Web;

using DotAPicker.Models;

namespace DotAPicker.DAL
{
    public class DotAContext: DbContext
    {
        public DotAContext(): base(nameof(DotAContext))
        {
            Database.Log = s => Debug.WriteLine(s);
            Database.CommandTimeout = 1000; //this is required when dropping and creating a new database, otherwise it times out
            Configuration.UseDatabaseNullSemantics = false;
        }

        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        /// <summary>
        /// This override makes it so you can't save unless you're authorized on the current account. However for rebuilds it needs to be allowed.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges() => base.SaveChanges(); //throw new Exception("Don't use the SaveChanges method, it doesn't authenticate.");

        public int SaveChanges(bool authenticated) => authenticated ? base.SaveChanges() : UndoChanges();

        public int UndoChanges()
        {
            this.DiscardChanges();
            return -1;
        }

        //just so I can work with a boolean instead of an integer when I want to
        public bool SaveChangesB(bool authenticated) => SaveChanges(authenticated) > -1;
    }

    public static class DBContextExtensions
    {
        //check if an entity is attacedh
        public static bool IsAttached<T>(this DbContext context, T entity) where T : class
        {
            if (entity == null) return false;

            var oc = ((IObjectContextAdapter)context).ObjectContext;

            var type = ObjectContext.GetObjectType(entity.GetType());

            // Get key PropertyInfos
            var propertyInfos = oc.MetadataWorkspace
                  .GetItems(DataSpace.CSpace).OfType<EntityType>()
                  .Where(i => i.Name == type.Name)
                  .SelectMany(i => i.KeyProperties)
                  .Join(type.GetProperties(), ep => ep.Name, pi => pi.Name, (ep, pi) => pi);

            // Get key values
            var keyValues = propertyInfos.Select(pi => pi.GetValue(entity)).ToArray();

            // States to look for    
            var states = EntityState.Added | EntityState.Modified |
                         EntityState.Unchanged | EntityState.Deleted;

            // Check if there is an entity having these key values
            return oc.ObjectStateManager.GetObjectStateEntries(states)
                     .Select(ose => ose.Entity).OfType<T>()
                     .Any(t => propertyInfos.Select(i => i.GetValue(t))
                                            .SequenceEqual(keyValues));
        }

        public static void DiscardChanges(this DbContext context)
        {
            foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }
    }

    public class DotAInitializer : DropCreateDatabaseIfModelChanges<DotAContext> //DropCreateDatabaseAlways <DotAContext>
    {
        public void ReSeed(DotAContext db) => Seed(db);

        protected override void Seed(DotAContext db)
        {
            var users = new List<User> {
                new User { Name = User.DEFAULT_USER,
                           Email = "default@user.com",
                           CurrentPatch = "7.07c",
                           ShowDeprecatedRelationships = false,
                           ShowDeprecatedTips = false,
                           LabelOptions = "Pusher|Nuker|Support|Disabler|Pure Damage|Agility|DoT|Strength|Intelligence|Carry|Melee|Ranged",
                           ProfileType = ProfileTypes.ReadOnly
                },
            };
            users.First().SetNewPassword("password");

            users.ForEach(u => db.Users.Add(u));
            db.SaveChanges(true);
            var defaultID = db.Users.First().Id;

            var heroes = new List<Hero> {
                new Hero { UserId = defaultID, Name = "Dazzle", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Drow Ranger", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Earthshaker", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Lich", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Lina", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Lion", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Mirana", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Morphling", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Necrophos", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Puck", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Pudge", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Razor", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Sand King", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Storm Spirit", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Tidehunter", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Vengeful Spirit", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Windranger", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Witch Doctor", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Slardar", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Enigma", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Faceless Void", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Tiny", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Venomancer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Clockwerk", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Dark Seer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Sniper", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Pugna", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Beastmaster", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Enchantress", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Leshrac", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Shadow Fiend", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Weaver", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Night Stalker", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Spectre", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Doom", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Bloodseeker", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Kunkka", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Riki", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Broodmother", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Jakiro", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Batrider", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Dragon Knight", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Warlock", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Lifestealer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Death Prophet", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Ursa", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Bounty Hunter", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Silencer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Clinkz", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Outworld Devourer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Bane", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Shadow Demon", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Lycan", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Phantom Lancer", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Treant Protector", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Gyrocopter", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Chaos Knight", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Phantom Assassin", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Rubick", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Luna", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Io", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Undying", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Disruptor", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Nyx Assassin", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Keeper of the Light", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Visage", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Magnus", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Centaur Warrunner", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Slark", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Timbersaw", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Medusa", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Troll Warlord", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Skywrath Mage", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Ember Spirit", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Legion Commander", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Terrorblade", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Techies", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Oracle", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Winter Wyvern", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Underlord", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Monkey King", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Anti-Mage", AltNames = "Antimage AM", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Ogre Magi", AltNames = "om", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Pangolier", AltNames = "", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Dark Willow", AltNames = "dw", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Spirit Breaker", AltNames = "sb barafu", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Sven", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Chen", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserId = defaultID, Name = "Crystal Maiden", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Bristleback", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Invoker", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserId = defaultID, Name = "Juggernaut", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Omniknight", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Viper", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Axe", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserId = defaultID, Name = "Brewmaster", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserId = defaultID, Name = "Elder Titan", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserId = defaultID, Name = "Naga Siren", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserId = defaultID, Name = "Meepo", AltNames = "", Notes = "", Preference = HeroPreference.Hated },
                new Hero { UserId = defaultID, Name = "Wraith King", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Zeus", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Tinker", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserId = defaultID, Name = "Tusk", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserId = defaultID, Name = "Templar Assassin", AltNames = "ta, lanaya", Notes = "TA is a mid hero; she benefits a lot from early levels and needs them to get kills so she can get deso/blink and snowball. Absent early farm she can be very underwhelming.", Preference = HeroPreference.Favorite },
                new Hero { UserId = defaultID, Name = "Lone Druid", AltNames = "LD", Notes = "LD can be played as a bear-focused hero or as a sniper-style damage dealer.", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Ancient Apparition", AltNames = "aa", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Arc Warden", AltNames = "aw", Notes = "I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! I have notes here! I'm curious what this looks like when it's really long! ", Preference = HeroPreference.Hated },
                new Hero { UserId = defaultID, Name = "Queen of Pain", AltNames = "QoP", Notes = "", Preference = HeroPreference.Indifferent },
                new Hero { UserId = defaultID, Name = "Phoenix", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Earth Spirit", AltNames = "", Notes = "Preferred because I think I'll like him a lot more when I suck less", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Nature's Prophet", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Huskar", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Shadow Shaman", AltNames = "", Notes = "", Preference = HeroPreference.Preferred },
                new Hero { UserId = defaultID, Name = "Alchemist", AltNames = "", Notes = "", Preference = HeroPreference.Disliked },
                new Hero { UserId = defaultID, Name = "Abaddon", AltNames = "", Notes = "Abaddon is versatile and can be played as core or support. I prefer playing him as a support, maxing aphotic shield so my carry is unkillable.", Preference = HeroPreference.Preferred }
            };
            heroes.ForEach(u => db.Heroes.Add(u));
            db.SaveChanges(true);
            //heroes.ForEach(t => db.Entry(t).State = EntityState.Detached);

            var tips = new List<Tip>{
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Bloodseeker").Id, Type = TipType.Other, Text = "Use bloodrage on both you and a squishy enemy for max damage", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Bane").Id, Type = TipType.Strategy, Text = "Nightmare supports to keep them from counter-initiating, then use fiend's grip", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.AbilityBuild, Text = "Max psi blades first for maximum lane push, or max refraction for best defense. Either way these are the best two abilities.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Other, Text = "Stack ancients starting early; as many stacks as possible. She can take them at level 8/9 with shrine, or earlier with DD rune.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Other, Text = "TA does burst physical damage, so take out squishy heros in team fights first before they can react.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Axe").Id, Type = TipType.Other, Text = "Max counter helix. The pure damage is incredibly powerful early and can be used as a farming tool, defensively, and offensively.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Terrorblade").Id, Type = TipType.AbilityUse, Text = "Reflection is very good against 5-core right clicking lineups (especially squishy ones)", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Legion Commander").Id, Type = TipType.ItemBuild, Text = "Using press the attack the gain health is more gold efficient (via clarities) than tangos/salve, providing you get the full duration. Salve/clarity is a good option. In a lane with any degree of harass, this is a bad idea.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Shadow Demon").Id, Type = TipType.AbilityUse, Text = "Shadow poison stacks do an absurd amount of damage early.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Counter, Text = "Abilities that reveal meld position: Defeaning blast, tornado, gust, flame break, charge of darkness", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.AbilityUse, Text = "Psionic trap: If you hit someone with a full charged trap (the trap charges over 4s to increase the slow % from 30 to 60) and before the slow ends you place and immediately activate a new trap then you refresh the duration but it keeps the maximum slow (60%) instead of resetting it to 30%. Works the other way around, too (giving a worse slow from a fully charged trap).", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Counter, Text = "Orb of venom deals low enough damage to not proc her refraction block but deals just enough to trigger her blink's cooldown. perfect for preventing her blinking out while she has charges up", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Spectre").Id, Type = TipType.Counter, Text = "Silver edge to break passives", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Broodmother").Id, Type = TipType.Strategy, Text = "Pick into lineups without good AOE/cleave", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Morphling").Id, Type = TipType.Strategy, Text = "Pick into lineups with much lockdown", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Other, Text = "TA stays invis during meld attack until the project attack, meaning you can blink away from the meld attack.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.AbilityUse, Text = "Use meld to disjoint projectile stuns", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.AbilityUse, Text = "Against Tinker, hold your meld until you see laser precast animation. Meld to cancel, then you get another attack or two to get the kill.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Strategy, Text = "Take offlane. Scales well with items.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "Pause between fire spirits because they don't stack. With dps talent, these can do 2k+ dmg.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "Sunray allows crossing impassible terrain by toggling movement", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "Huge power spike at level 3 because fire spirits at level 2 more than double in damage", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.ItemBuild, Text = "Go Wand->Tranquils->Midas->Veil because he doesn't really want to farm", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Other, Text = "Save wand charges to guarantee mana for ult if possible", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityBuild, Text = "go 2-2 first, then 2-2-1-1, max fire spirits and sunray prioritizing ult and XP talent", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.ItemBuild, Text = "After veil, buy an orchid. Really.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.ItemBuild, Text = "If an AS threat to the egg is an issue (troll, huskar, etc.), buy a halberd", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Strategy, Text = "Counters channelled magic ults like CM as egg is spell immune", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "Use egg in the back half of team fights, not for initiation or when people can easily disengage. Pheonix punishes overcommitment and bad positioning.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "Team fight skill sequence: icarus dive to apply debuff. During the dive, use fire spirits and veil. Finally, ult. Might happen quickly because of ult duration.", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "If you are slowed below 250, sunray will actually increase your move speed", Patch = "7.07b" },
                new Tip { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.AbilityUse, Text = "For escapes, you can sunray, toggle movement on, and TP while you're actually off the map", Patch = "7.07b" }
            };
            tips.ForEach(u => db.Tips.Add(u));
            db.SaveChanges(true);
            //tips.ForEach(t => db.Entry(t).State = EntityState.Detached);

            var relationships = new List<Relationship>{
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Anti-Mage").Id, HeroObjectId = heroes.First(h => h.Name == "Bloodseeker").Id, Type = TipType.Counter, Text = "BS can use rupture to make it difficult for AM to flee", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Alchemist").Id, HeroObjectId = heroes.First(h => h.Name == "Anti-Mage").Id, Type = TipType.Synergy, Text = "Antimage benefits a lot from an aghs that doesn't take up an item slot", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Batrider").Id, HeroObjectId = heroes.First(h => h.Name == "Enigma").Id, Type = TipType.Other, Text = "BKB piercing lasso ends black hole", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Axe").Id, HeroObjectId = heroes.First(h => h.Name == "Nature's Prophet").Id, Type = TipType.Counter, Text = "Can use taunt to cancel TP", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, HeroObjectId = heroes.First(h => h.Name == "Chaos Knight").Id, Type = TipType.Counter, Text = "TA doesn't fair well against illusion heroes that can break through diffraction", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, HeroObjectId = heroes.First(h => h.Name == "Phantom Lancer").Id, Type = TipType.Counter, Text = "TA doesn't fair well against illusion heroes that can break through diffraction", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, HeroObjectId = heroes.First(h => h.Name == "Enigma").Id, Type = TipType.Counter, Text = "Eidelons can break through diffraction and black hole stops her from doing damage.", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, HeroObjectId = heroes.First(h => h.Name == "Shadow Shaman").Id, Type = TipType.Synergy, Text = "SS can disable a hero while TA does massive amounts of damage", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Templar Assassin").Id, HeroObjectId = heroes.First(h => h.Name == "Enigma").Id, Type = TipType.Synergy, Text = "Black hole can group people together to receive lots of damage from psi blades", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Sven").Id, HeroObjectId = heroes.First(h => h.Name == "Enigma").Id, Type = TipType.Synergy, Text = "Black hole can group people together to receive lots of damage from cleave", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Warlock").Id, HeroObjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Synergy, Text = "Fatal bonds spreads TA's psi blade damage even farther", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Abaddon").Id, HeroObjectId = heroes.First(h => h.Name == "Lina").Id, Type = TipType.Counter, Text = "Lina can burst through abaddon without triggering ult if timed correctly", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phantom Lancer").Id, HeroObjectId = heroes.First(h => h.Name == "Wraith King").Id, Type = TipType.Other, Text = "PL's mana burn makes it so WK doesn't have the mana to reincarnate", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Anti-Mage").Id, HeroObjectId = heroes.First(h => h.Name == "Wraith King").Id, Type = TipType.Counter, Text = "AM's mana burn makes it so WK doesn't have the mana to reincarnate", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Bloodseeker").Id, HeroObjectId = heroes.First(h => h.Name == "Storm Spirit").Id, Type = TipType.Counter, Text = "Rupture makes it difficult for SS to ball lightning away", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Bristleback").Id, HeroObjectId = heroes.First(h => h.Name == "Riki").Id, Type = TipType.Counter, Text = "Riki gives more damage from behind, bb takes reduced damage from behind", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Silencer").Id, HeroObjectId = heroes.First(h => h.Name == "Legion Commander").Id, Type = TipType.Synergy, Text = "Silencer can prevent LC from being disabled during duel", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Batrider").Id, HeroObjectId = heroes.First(h => h.Name == "Legion Commander").Id, Type = TipType.Synergy, Text = "Batrider can lasso enemies into a duel", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Faceless Void").Id, HeroObjectId = heroes.First(h => h.Name == "Lina").Id, Type = TipType.Synergy, Text = "Lina can do a lot of damage from outside the chronosphere", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Venomancer").Id, HeroObjectId = heroes.First(h => h.Name == "Templar Assassin").Id, Type = TipType.Counter, Text = "Counters during laning stage with DoT AND wards breaking through dispersion", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, HeroObjectId = heroes.First(h => h.Name == "Slark").Id, Type = TipType.Counter, Text = "Can escape tether with dive", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Phoenix").Id, HeroObjectId = heroes.First(h => h.Name == "Disruptor").Id, Type = TipType.Other, Text = "Can escape kinetic field with dive", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Meepo").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Counter, Text = "Meepo clones count toward killing egg", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Arc Warden").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Other, Text = "Tempest double counts toward killing egg", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Lone Druid").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Other, Text = "Bear counts toward killing egg", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Shadow Shaman").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Other, Text = "Can keep phoenix down and keep him from escaping or ulting", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Lion").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Other, Text = "Can disable phoenix and prevent him from escaping or ulting, and burst him down with ult", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Silencer").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Counter, Text = "Phoenix doesn't do shit if silenced", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Juggernaut").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Counter, Text = "High attack speed kills eggs, has spell immunity", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Lifestealer").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Counter, Text = "Has escapes from damage (ult and spell immunity), plus massive % based physical damage", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Ursa").Id, HeroObjectId = heroes.First(h => h.Name == "Phoenix").Id, Type = TipType.Counter, Text = "Rapid egg kills, lots of burst physical damage, ult can tank your burst magical damage", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Bloodseeker").Id, HeroObjectId = heroes.First(h => h.Name == "Spirit Breaker").Id, Type = TipType.Counter, Text = "Rupture prevents SB from charging away", Patch = "7.07b" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Dark Willow").Id, HeroObjectId = heroes.First(h => h.Name == "Chaos Knight").Id, Type = TipType.Counter, Text = "Terrorize can make all illusions run away from their burst target, and brambles can entrap them.", Patch = "7.07c" },
                new Relationship { UserId = defaultID, HeroSubjectId = heroes.First(h => h.Name == "Lion").Id, HeroObjectId = heroes.First(h => h.Name == "Chaos Knight").Id, Type = TipType.Counter, Text = "Mana drain destroys illusions and talent places it on multiple targets. Q deals with illusions as well as Aghs for AOE ult.", Patch = "7.07b" }
            };
            relationships.ForEach(r => db.Relationships.Add(r));
            db.SaveChanges(true);
        }
    }

    public static class DotADBTools
    {
        public static bool CopyUser(this DotAContext db, User src, User dest, bool authenticated, bool includeNotes = true)
        {
            if (!authenticated) return false;
            //create a data-only copy of the user profile (not EF associations)
            var userCopy = new ProfileCopy(src);

            return ImportProfile(db, userCopy, dest, authenticated, includeNotes);
        }

        public static bool ImportProfile(this DotAContext db, ProfileCopy src, User dest, bool authenticated, bool includeNotes = true)
        {
            if (!authenticated) return false;

            //validate empty destination profile
            if (dest.Heroes.Any() ||
                dest.Tips.Any() ||
                dest.Relationships.Any()) return false;

            //update user (header-level) data
            src.CopyTo(dest);
            db.SaveChanges(true);

            //Add heroes
            var destUserId = dest.Id;
            src.Heroes.ForEach(hCopy => {
                var hero = new Hero { UserId = destUserId };
                hCopy.CopyTo(hero);
                db.Heroes.Add(hero);
            });
            db.SaveChanges(true);

            if (includeNotes)
            {
                //create hero/ID cross-reference
                var heroList = db.Heroes.Where(h => h.UserId == destUserId).ToDictionary(h => h.Name, h => h.Id);

                //add tips
                src.Tips.ForEach(tCopy => {
                    var tip = new Tip { UserId = destUserId };
                    tCopy.CopyTo(tip, heroList);
                    db.Tips.Add(tip);
                });
                db.SaveChanges(true);

                //add relationships
                src.Relationships.ForEach(rCopy => {
                    var relationship = new Relationship { UserId = destUserId };
                    rCopy.CopyTo(heroList, relationship);
                    db.Relationships.Add(relationship);
                });
                db.SaveChanges(true);
            }

            return true;
        }

        public static bool ClearUserData(this DotAContext db, User user)
        {
            if (!user.IsAuthenticated) return false;

            foreach(var item in db.Tips.Where(i => i.UserId == user.Id))
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            db.SaveChanges(true);

            foreach (var item in db.Relationships.Where(i => i.UserId == user.Id))
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            db.SaveChanges(true);

            foreach (var item in db.Heroes.Where(i => i.UserId == user.Id))
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            db.SaveChanges(true);

            return true;
        }

        public static bool DeleteUser(this DotAContext db, User user)
        {
            if (!user.IsAuthenticated) return false;

            db.ClearUserData(user);
            db.Entry(user).State = EntityState.Deleted;
            db.SaveChanges(true);

            return true;
        }
    }
}