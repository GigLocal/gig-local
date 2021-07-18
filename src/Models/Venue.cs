using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GigLocal.Models
{
    public class Venue
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }

        public ICollection<Gig> Gigs { get; set; }
    }
}
