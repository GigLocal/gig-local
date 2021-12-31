namespace GigLocal.Services;

public interface IArtistService
{
    Task<Artist> CreateAsync(ArtistCreateModel model);
    Task DeleteImageAsync(int artistID);
    Task UpdateAsync(int id, ArtistCreateModel artist);
}

public class ArtistService : IArtistService
{
    private readonly StorageOptions _options;
    private readonly GigContext _context;

    public ArtistService(IOptions<StorageOptions> optionsAccessor, GigContext context)
    {
        _options = optionsAccessor.Value;
        _context = context;
    }

    private BlobClient GetArtistBlobClient(int artistID)
    {
        var blobName = $"artists/{artistID}/image.jpg";
        return new BlobClient(_options.ConnectionString, "public", blobName);
    }

    private async Task<string> UploadImageAsync(int artistID, Stream stream)
    {
        using MemoryStream outStream = new();
        using Image image = Image.Load(stream);
        image.Mutate(
            i => i.Resize(new ResizeOptions {
                Mode = ResizeMode.Crop,
                Position = AnchorPositionMode.Center,
                Size = new Size(640, 360)
            })
        );

        await image.SaveAsJpegAsync(outStream);
        var blobClient = GetArtistBlobClient(artistID);
        var binaryContent = new BinaryData(outStream.GetBuffer());
        await blobClient.UploadAsync(binaryContent, overwrite: true);
        return blobClient.Uri.ToString();
    }

    public Task DeleteImageAsync(int artistID)
    {
        var blobClient = GetArtistBlobClient(artistID);
        return blobClient.DeleteAsync();
    }

    public async Task<Artist> CreateAsync(ArtistCreateModel artist)
    {
        if (_context.Artists.Any(a => a.Name.ToLower().Trim() == artist.Name.ToLower().Trim()))
        {
            throw new InvalidOperationException($"Artist with name '{artist.Name}' already exists.");
        }

        var newArtist = new Artist
        {
            Name = artist.Name,
            Description = artist.Description,
            Website = artist.Website
        };

        _context.Artists.Add(newArtist);
        await _context.SaveChangesAsync();

        if (artist.FormFile?.Length > 0)
        {
            using var formFileStream = artist.FormFile.OpenReadStream();

            var imageUrl = await UploadImageAsync(newArtist.ID, formFileStream);

            newArtist.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
        }

        return newArtist;
    }

    public async Task UpdateAsync(int id, ArtistCreateModel artist)
    {
        var artistToUpdate = await _context.Artists.FindAsync(id);

        if (artistToUpdate == null)
        {
            throw new InvalidOperationException($"Artist with ID '{id}' does not exist.");
        }

        artistToUpdate.Name = artist.Name;
        artistToUpdate.Description = artist.Description;
        artistToUpdate.Website = artist.Website;

        if (artist.FormFile?.Length > 0)
        {
            using var formFileStream = artist.FormFile.OpenReadStream();
            var imageUrl = await UploadImageAsync(artistToUpdate.ID, formFileStream);

            artistToUpdate.ImageUrl = imageUrl;
        }

        await _context.SaveChangesAsync();
    }
}
