namespace GigLocal.Models;

public class Gig
{
    public int ID { get; set; }

    public int ArtistID { get; set; }

    public int VenueID { get; set; }

    public DateTime Date { get; set; }

    public Decimal TicketPrice { get; set; }

    public string TicketWebsite { get; set; }

    public Artist Artist { get; set; }

    public Venue Venue { get; set; }
}
