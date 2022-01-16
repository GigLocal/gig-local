namespace GigLocal.Services;

public interface IImageService
{
    Task<string> UploadImageAsync(Stream stream);
    Task DeleteImageAsync(string imageUrl);
}

public class ImageService : IImageService
{
    private readonly StorageOptions _options;

    public ImageService(IOptions<StorageOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }

    private BlobClient GetBlobClient()
    {
        var blobName = $"images/{Guid.NewGuid()}.jpg";
        return new BlobClient(_options.ConnectionString, "public", blobName);
    }

    public async Task<string> UploadImageAsync(Stream stream)
    {
        using MemoryStream outStream = new();
        using Image image = Image.Load(stream);
        image.Mutate(
            i => i.Resize(new ResizeOptions {
                Mode = ResizeMode.Crop,
                Position = AnchorPositionMode.Top,
                Size = new Size(640, 360)
            })
        );

        await image.SaveAsJpegAsync(outStream);
        var blobClient = GetBlobClient();
        var binaryContent = new BinaryData(outStream.GetBuffer());
        await blobClient.UploadAsync(binaryContent);
        return blobClient.Uri.ToString();
    }

    public Task DeleteImageAsync(string imageUrl)
    {
        var blobClient = new BlobClient(new Uri(imageUrl));
        return blobClient.DeleteAsync();
    }
    // public async Task<Artist> CreateAsync(IFormFile formFile)
    // {
    //     _context.Artists.Add(newArtist);
    //     await _context.SaveChangesAsync();

    //     if (formFile?.Length > 0)
    //     {
    //         using var formFileStream = formFile.OpenReadStream();

    //         var imageUrl = await UploadImageAsync(formFileStream);

    //         newArtist.ImageUrl = imageUrl;
    //         await _context.SaveChangesAsync();
    //     }

    //     return newArtist;
    // }
}
