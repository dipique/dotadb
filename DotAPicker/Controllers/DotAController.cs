using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotAPicker.Models;

namespace DotAPicker.Controllers
{
    public class DotAController : Controller
    {
        internal const string dataInd = "data";
        internal DotADB db
        {
            get
            {
                if ((DotADB)(Session[dataInd]) == null)
                    Session[dataInd] = DotADB.Load();

                return (DotADB)Session[dataInd];
            }
            set
            {
                Session[dataInd] = value;
            }
        }
    }
}