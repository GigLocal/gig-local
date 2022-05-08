namespace GigLocal.Models;

public class Venue
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Address { get; set; }

    public string Suburb { get; set; }

    public string State { get; set; }

    public int Postcode { get; set; }

    public string Website { get; set; }

    public string ImageUrl { get; set; }

    public string TimeZone { get; set; }

    public ICollection<Gig> Gigs { get; set; }
}
