using System.Collections.Generic;

namespace DotAPicker.Models
{
    public class HeroDetailViewModel
    {
        public Hero Hero { get; set; }
        public IEnumerable<Tip> Tips { get; set; }
        public IEnumerable<Relationship> Relationships { get; set; }
    }
}