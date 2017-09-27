using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace DotAPicker.Utilities
{
    public static class Casting
    {
        /// <summary>
        /// Takes an object and converts it to its parent type, stripping off any unique information
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static TChild DownCast<TParent, TChild>(TParent parentObj)
        {
            string output = JsonConvert.SerializeObject(parentObj);
            return JsonConvert.DeserializeObject<TChild>(output);
        }

        public static TParent UpCast<TParent, TChild>(TChild childObj)
        {
            string output = JsonConvert.SerializeObject(childObj);
            return JsonConvert.DeserializeObject<TParent>(output);
        }
    }
}