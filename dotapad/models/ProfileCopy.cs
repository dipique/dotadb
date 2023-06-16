using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DotAPicker.Models
{
    /// <summary>
    /// Class to hold all data from a user for load or export
    /// </summary>
    public class ProfileCopy: CopyObject<User>
    {
        public List<string?>? Labels { get; set; }
        public string? CurrentPatch { get; set; }
        public bool ShowDeprecatedTips { get; set; }
        public bool ShowDeprecatedRelationships { get; set; }
        public ProfileCopy(User user, bool includeNotes = true, bool includeHeroPreference = false): base(user)
        {
            Heroes.AddRange(user.Heroes.Select(h => new _Hero(h, includeHeroPreference)));
            if (includeNotes)
            {
                Tips.AddRange(user.Tips.Select(t => new _Tip(t)));
                Relationships.AddRange(user.Relationships.Select(r => new _Relationship(r)));
            }
        }

        public ProfileCopy() { } //needed so it can be serializable

        [NotFromCopy] public List<_Hero> Heroes { get; set; } = new List<_Hero>();
        [NotFromCopy] public List<_Tip> Tips { get; set; } = new List<_Tip>();
        [NotFromCopy] public List<_Relationship> Relationships { get; set; } = new List<_Relationship>();

        public string ToXML()
        {
            var serializer = new XmlSerializer(typeof(ProfileCopy));
            var textWriter = new StringWriter();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();            
        }
    }

    public class _Hero: CopyObject<Hero>
    {
        public string? Name { get; set; }
        public string? AltNames { get; set; }
        public string? Notes { get; set; }
        public HeroPreference Preference { get; set; }
        public List<string?> DescriptionLabels { get; set; } = new();

        public _Hero(Hero hero, bool includeHeroPreference = false) : base(hero) { if (!includeHeroPreference) Preference = HeroPreference.Indifferent; }
        public _Hero() { }
    }

    public abstract class _DotANote: CopyObject<DotANote>
    {
        [NotFromCopy]
        public string? Subject { get; set; }
        public NoteType Type { get; set; }
        public string? Text { get; set; }
        public string? Patch { get; set; }
        public bool Deprecated { get; set; }
        public string? Source { get; set; }     
        
        public _DotANote(DotANote obj): base(obj)
        {
            Subject = obj.HeroSubjectId == null ? obj.LabelSubject : obj.HeroSubject.Name;
        }

        public _DotANote() { }

        public virtual void CopyTo(DotANote obj, Dictionary<string, int> heroList)
        {
            base.CopyTo(obj);
            if (heroList.TryGetValue(Subject ?? "", out int heroSubjectID))
            {
                obj.HeroSubjectId = heroSubjectID;
            }
            else
            {
                obj.LabelSubject = Subject;
            }

        }
    }

    public class _Tip: _DotANote
    {
        public _Tip(Tip obj): base(obj) { }
        public _Tip() { }
    }

    public class _Relationship: _DotANote
    {
        public string? Object { get; set; }

        public _Relationship(Relationship obj): base(obj)
        {
            Object = obj.HeroObjectId == null ? obj.LabelObject : obj.HeroObject.Name;
        }

        public _Relationship() { }

        public void CopyTo(Dictionary<string, int> heroList, Relationship obj) //parameters in reverse order to eliminate ambiguity with dotanote method
        {
            base.CopyTo(obj, heroList);
            if (heroList.TryGetValue(Object ?? string.Empty, out int heroObjectID))
            {
                obj.HeroObjectId = heroObjectID;
            }
            else
            {
                obj.LabelObject = Object ?? string.Empty;
            }
        }
    }

    public abstract class CopyObject<T>
    {
        /// <summary>
        /// Copy one object to another by property names
        /// </summary>
        /// <param name="obj"></param>
        public void CopyFrom(T obj)
        {
            var destProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .Where(p => p.GetCustomAttribute<NotFromCopyAttribute>() == null))
            {
                var destProp = destProps.FirstOrDefault(p => p.Name == prop.Name && 
                                                             p.PropertyType == prop.PropertyType);
                if (destProp == null) continue;
                prop.SetValue(this, destProp.GetValue(obj));
            }
        }

        public CopyObject(T obj)
        {
            CopyFrom(obj);
        }

        public CopyObject() { } // needed so it can be serializable

        public virtual void CopyTo(T obj)
        {
            var destProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .Where(p => p.GetCustomAttribute<NotFromCopyAttribute>() == null))
            {
                var destProp = destProps.FirstOrDefault(p => p.Name == prop.Name &&
                                                             p.PropertyType == prop.PropertyType);
                if (destProp == null) continue;
                destProp.SetValue(obj, prop.GetValue(this));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotFromCopyAttribute: Attribute
    {
        public NotFromCopyAttribute() { }
    }
}