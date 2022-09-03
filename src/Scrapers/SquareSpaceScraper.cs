namespace GigLocal.Scrapers;
public class SquarespaceScraper
{
    private readonly string _baseUrl;

    public SquarespaceScraper(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public IEnumerable<ScrapedGig> Scrape(string squarespaceHtmlEventPage)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(squarespaceHtmlEventPage);

        HtmlNodeCollection articleNodes = GetArticleNodes(doc);

        foreach (HtmlNode articleNode in articleNodes)
        {
            HtmlNode infoNode = GetInfoNode(articleNode);
            string eventTitle = GetEventTitle(infoNode);
            var eventTitleLower = eventTitle.ToLower();
            if (eventTitleLower.Contains("gig guide") || eventTitleLower.Contains("trivia"))
            {
                continue;
            }
            DateTime startDateTime = GetDateTime(infoNode, "start");
            DateTime endDateTime = GetDateTime(infoNode, "end");
            string imageUrl = GetImageUrl(articleNode);
            string eventUrl = GetEventUrl(infoNode);
            string description = GetDescription(infoNode);
            yield return new ScrapedGig(
                eventTitle,
                startDateTime,
                endDateTime,
                imageUrl,
                eventUrl,
                description);
        }
    }

    protected HtmlNodeCollection GetArticleNodes(HtmlDocument doc)
    {
        return doc.DocumentNode.SelectNodes("//article[contains(@class, 'eventlist-event--upcoming')]");
    }

    protected HtmlNode GetInfoNode(HtmlNode articleNode)
    {
        return GetChildNode(articleNode, "eventlist-column-info");
    }

    protected DateTime GetDateTime(HtmlNode infoNode, string target)
    {
        var listNode = GetChildNode(infoNode, "eventlist-meta");
        HtmlNode timeNode;
        try
        {
            var listItemNode = GetChildNode(listNode, "eventlist-meta-time");
            var spanNode = GetChildNode(listItemNode, "event-time-12hr");
            timeNode = GetChildNode(spanNode, $"event-time-12hr-{target}");
        }
        catch (InvalidOperationException)
        {
            var listItemNode = GetChildNode(listNode, "eventlist-meta-date");
            var spanNodes = GetChildNodes(listItemNode, "eventlist-meta-time");
            var index = target == "start" ? 0 : 1;
            try
            {
                timeNode = GetChildNode(spanNodes.ElementAt(index), "event-time-12hr");
            }
            catch (InvalidOperationException)
            {
                timeNode = GetChildNode(spanNodes.ElementAt(index), "event-time-localized");
            }
        }

        var dateString = timeNode.Attributes["datetime"].Value;
        var timeString = timeNode.InnerHtml;

        var dateTimeString = $"{dateString} {timeString}";

        return DateTime.Parse(dateTimeString);
    }

    protected string GetImageUrl(HtmlNode articleNode)
    {
        HtmlNode image;
        try
        {
            var imageAnchorNode = GetChildNode(articleNode, "eventlist-column-thumbnail");
            image = imageAnchorNode.ChildNodes.Where(x => x.Name == "img").First();

        }
        catch (InvalidOperationException)
        {
            var imageAnchorNode = GetChildNode(articleNode, "eventlist-column-info");
            image = imageAnchorNode.Descendants(level: 12).Where(x => x.HasClass("thumb-image")).First();
        }

        return image.Attributes["data-src"].Value;
    }

    protected string GetEventTitle(HtmlNode infoNode)
    {
        var rawEventTile = GetChildNode(infoNode, "eventlist-title").InnerText.Trim();
        return HttpUtility.HtmlDecode(rawEventTile);
    }

    protected string GetEventUrl(HtmlNode infoNode)
    {
        var headerNode = GetChildNode(infoNode, "eventlist-title");
        var relativeUrl = GetChildNode(headerNode, "eventlist-title-link").Attributes["href"].Value;
        return $"{_baseUrl}{relativeUrl}";
    }

    protected string GetDescription(HtmlNode infoNode)
    {
        HtmlNode descriptionNode;
        try
        {
            descriptionNode = GetChildNode(infoNode, "eventlist-description");
        }
        catch (InvalidOperationException)
        {
            descriptionNode = GetChildNode(infoNode, "eventlist-excerpt");
        }
        var rawDescription = descriptionNode.InnerText.Replace("\n", " ").Replace("\t", "").Trim();
        return HttpUtility.HtmlDecode(rawDescription);
    }

    private HtmlNode GetChildNode(HtmlNode node, string className)
    {
        return node.ChildNodes.Where(x => x.HasClass(className)).First();
    }

    private IEnumerable<HtmlNode> GetChildNodes(HtmlNode node, string className)
    {
        return node.ChildNodes.Where(x => x.HasClass(className));
    }
}
