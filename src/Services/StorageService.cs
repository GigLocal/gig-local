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

        public async Task<string> UploadArtistImageAsync(int artistID, Stream stream)
        {
            using MemoryStream outStream = new();
            using Image image = Image.Load(stream);
            image.Mutate(
                i => i.Resize(new ResizeOptions {
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                    Size = new Size(300, 300)
                })
            );

            await image.SaveAsJpegAsync(outStream);

            var blobName = $"artists/{artistID}/image.jpg";
            var blobClient = new BlobClient(_options.ConnectionString, "public", blobName);
            var binaryContent = new BinaryData(outStream.GetBuffer());
            await blobClient.UploadAsync(binaryContent);
            return blobClient.Uri.ToString();
        }

        public Task DeleteArtistImageAsync(int artistID)
        {
            var blobName = $"artists/{artistID}/image.jpg";
            var blobClient = new BlobClient(_options.ConnectionString, "public", blobName);
            return blobClient.DeleteAsync();
        }
    }
}
