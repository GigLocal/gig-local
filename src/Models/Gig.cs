namespace GigLocal.Models;

public class Gig
{
    public int ID { get; set; }

    public string ArtistName { get; set; }

    public string Description { get; set; }

    public string EventUrl { get; set; }

    public string ImageUrl { get; set; }

    public bool Approved { get; set; }

    public int VenueID { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Venue Venue { get; set; }
}
