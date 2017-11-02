using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public class Setting
    {
        public int ID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}