namespace GigLocal.Models;

public class Gig
{
    public int ID { get; set; }

    public string ArtistName { get; set; }

    public string Description { get; set; }

    public string EventUrl { get; set; }

    public string ImageUrl { get; set; }

    public int ArtistID { get; set; }

    public int VenueID { get; set; }

    public DateTime Date { get; set; }

    public Artist Artist { get; set; }

    public Venue Venue { get; set; }
}
