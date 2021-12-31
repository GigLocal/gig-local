namespace GigLocal.ViewModels;

public class ArtistReadModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Website { get; set; }

    [Display(Name = "Image")]
    public string ImageUrl { get; set; }
}
