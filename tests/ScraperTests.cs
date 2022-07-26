using System.Net.Http;
using System.Threading.Tasks;

namespace GigLocal.Scraper.Tests;
public class SquarespaceScraperTests
{
    [Fact]
    public void SquarespaceScraperReturnsExpectedContent()
    {
        var scraper = new SquarespaceScraper("https://www.edinburghcastle.net.au");
        var html = File.ReadAllText(Path.Combine("html", "squarespace.html"));
        var gigs = scraper.Scrape(html).ToList();

        Assert.Equal(12, gigs.Count);
        Assert.Equal("Triana Whitt", gigs[0].EventTitle);
        Assert.Equal(new DateTime(2022, 7, 23, 18, 0, 0), gigs[0].StartDate);
        Assert.Equal(new DateTime(2022, 7, 23, 20, 0, 0), gigs[0].EndDate);
        Assert.Equal("https://images.squarespace-cdn.com/content/v1/568e056a1c1210854ed6005c/1656924335891-C2XF06JSFQWT6Z5IB1OR/Triana+Whit+-+Don+Bookings.jpg", gigs[0].ImageUrl);
        Assert.Equal("https://www.edinburghcastle.net.au/gig-guide/2022/7/23/triana-whitt", gigs[0].EventUrl);
        Assert.StartsWith("The Triana Whit Trio bring R&B, Latin, Jazz, Pop and Blues", gigs[0].Description);

        Assert.Equal("Hauptstimme", gigs[^1].EventTitle);
        Assert.Equal(new DateTime(2022, 7, 31, 16, 0, 0), gigs[^1].StartDate);
        Assert.Equal(new DateTime(2022, 7, 31, 18, 0, 0), gigs[^1].EndDate);
        Assert.Equal("https://images.squarespace-cdn.com/content/v1/568e056a1c1210854ed6005c/1656925128427-VLL8HWIGNBX9QT2FZAUC/Hauptstimme+Promo+Blank+A4+08+-+06+-+22+-+Don+Bookings.jpg", gigs[^1].ImageUrl);
        Assert.Equal("https://www.edinburghcastle.net.au/gig-guide/2022/7/31/hauptstimme", gigs[^1].EventUrl);
        Assert.StartsWith("Haupstimme is a chamber jazz duo featuring the unique combination", gigs[^1].Description);
    }

    [Fact]
    public async Task yes(){
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Cookie", "crumb=BXbAUEFdGHUTODVkMmY2NGViYmJjYWMzNzNkMDhiODdjMjkwNjM2");
        try {
            var resp = await httpClient.GetAsync("https://wesleyanne.com.au/events");

            var html = await resp.Content.ReadAsStringAsync();
        }
        catch (Exception e){
            Console.WriteLine(e);
        }

        // Assert.NotNull(html);
    }
}
