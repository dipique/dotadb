using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotAPicker.Models
{
    public class User
    {
        public const string DEFAULT_USER = "default";

        public int ID { get; set; }
        public string Username { get; set; }
    }
}