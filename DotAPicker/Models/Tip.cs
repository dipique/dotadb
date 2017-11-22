using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotAPicker.Models
{
    [Table(nameof(Tip))]
    public class Tip: DotANote
    {

    }
}