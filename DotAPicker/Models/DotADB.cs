using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace DotAPicker.Models
{
    public class DotASession
    {
        private const string DEFAULT_USER = "default";
        public string CurrentPatch => Settings.CurrentPatch;
        

        public List<Hero> Heroes { get; set; } = new List<Hero>();
        public List<Tip> Tips { get; set; } = new List<Tip>();
        public List<Relationship> Relationships { get; set; } = new List<Relationship>();
        public DotASettings Settings { get; set; } = new DotASettings();

        ///// <summary>
        ///// Defaults to the path in the web config
        ///// </summary>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //public static DotADB Load(string filename = null)
        //{
        //    filename = filename ?? ConfigurationManager.AppSettings[DB_PATH_SETTING];

        //    //first run logic
        //    if (!File.Exists(filename)) new DotADB().Save();

        //    var s = new XmlSerializer(typeof(DotADB));
        //    var fs = new FileStream(filename, FileMode.Open); //todo: what if file can't be accessed, is invalid, etc.
        //    var retVal = (DotADB)s.Deserialize(fs);
        //    fs.Close();

        //    //if (retVal.Settings.Labels.Count() == 0) retVal.Settings.Labels = new List<string>() { "Support", "Nuker", "Disabler", "Pusher" };

        //    return retVal;
        //}

        ///// <summary>
        ///// Defaults to the path in the web config
        ///// </summary>
        ///// <param name="filename"></param>
        //public void Save(string filename = null)
        //{
        //    filename = filename ?? ConfigurationManager.AppSettings[DB_PATH_SETTING];

        //    var s = new XmlSerializer(typeof(DotADB));
        //    if (File.Exists(filename)) File.Delete(filename); //todo: something more robust
        //    var fs = new FileStream(filename, FileMode.OpenOrCreate);
        //    s.Serialize(fs, this);
        //    fs.Close();
        //}
    }
}