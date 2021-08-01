using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GigLocal.Services
{
    public class StorageOptions
    {
        public string ConnectionString { get; set; }
    }

    public interface IStorageService
    {
        Task<string> UploadArtistImageAsync(int artistID, Stream stream);
        Task DeleteArtistImageAsync(int artistID);
    }

    public class StorageService : IStorageService
    {
        private readonly StorageOptions _options;

        public StorageService(IOptions<StorageOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        private BlobClient GetArtistBlobClient(int artistID)
        {
            var blobName = $"artists/{artistID}/image.jpg";
            return new BlobClient(_options.ConnectionString, "public", blobName);
        }

        public async Task<string> UploadArtistImageAsync(int artistID, Stream stream)
        {
            using MemoryStream outStream = new();
            using Image image = Image.Load(stream);
            image.Mutate(
                i => i.Resize(new ResizeOptions {
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                    Size = new Size(1280, 720)
                })
            );

            await image.SaveAsJpegAsync(outStream);
            var blobClient = GetArtistBlobClient(artistID);
            var binaryContent = new BinaryData(outStream.GetBuffer());
            await blobClient.UploadAsync(binaryContent);
            return blobClient.Uri.ToString();
        }

        public Task DeleteArtistImageAsync(int artistID)
        {
            var blobClient = GetArtistBlobClient(artistID);
            return blobClient.DeleteAsync();
        }
    }
}
