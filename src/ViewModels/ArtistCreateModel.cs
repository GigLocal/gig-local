namespace GigLocal.ViewModels;

public class ArtistCreateModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; }

    public string Website { get; set; }

    [Display(Name = "Image")]
    public IFormFile FormFile { get; set; }
}
