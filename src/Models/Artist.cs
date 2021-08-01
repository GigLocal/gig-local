using System.Collections.Generic;

namespace GigLocal.Models
{
    public class Artist
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Website { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Gig> Gigs { get; set; }
    }
}
