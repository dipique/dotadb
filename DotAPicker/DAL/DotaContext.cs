using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;

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

    public class DotAInitializer : CreateDatabaseIfNotExists<DotAContext> //DropCreateDatabaseIfModelChanges<DotAContext> //DropCreateDatabaseAlways <DotAContext>
    {
        public void ReSeed(DotAContext db) => Seed(db);
        protected override void Seed(DotAContext db) => SeedData.Seed(db);
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