namespace GigLocal.Services;

public interface IImageService
{
    Task<string> UploadImageAsync(Stream stream);
    Task DeleteImageAsync(string imageUrl);
    Task<string> CopyImageAsync(string imageUrl);
}

public class ImageService : IImageService
{
    private readonly StorageOptions _options;
    private const string ContainerName = "public";

    public ImageService(IOptions<StorageOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }

    private BlobClient GetBlobClient()
    {
        var blobName = $"images/{Guid.NewGuid()}.jpg";
        return new BlobClient(_options.ConnectionString, ContainerName, blobName);
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
        return $"{_options.CdnEndpointHostname}/{blobClient.BlobContainerName}/{blobClient.Name}";
    }

    public Task DeleteImageAsync(string imageUrl)
    {
        var blobName = imageUrl.Split($"{ContainerName}/")[1];
        var blobClient = new BlobClient(_options.ConnectionString, ContainerName, blobName);
        return blobClient.DeleteAsync();
    }

    public async Task<string> CopyImageAsync(string imageUrl)
    {
        var blobName = imageUrl.Split($"{ContainerName}/")[1];
        var blobClient = new BlobClient(_options.ConnectionString, ContainerName, blobName);
        var destBlobClient = GetBlobClient();
        await destBlobClient.StartCopyFromUriAsync(blobClient.Uri);
        return $"{_options.CdnEndpointHostname}/{destBlobClient.BlobContainerName}/{destBlobClient.Name}";
    }
}
