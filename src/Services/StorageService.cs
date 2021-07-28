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
        Task<string> UploadAsync(string container, string folder, MemoryStream fileStream);
    }

    public class StorageService : IStorageService
    {
        private readonly StorageOptions _options;

        public StorageService(IOptions<StorageOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public async Task<string> UploadAsync(string container, string folder, MemoryStream fileStream)
        {
            using var outStream = new MemoryStream();
            using Image image = Image.Load(fileStream.GetBuffer());
            image.Mutate(
                i => i.Resize(new ResizeOptions {
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                    Size = new Size(300, 300)
                })
            );

            await image.SaveAsJpegAsync(outStream);

            var blobName = $"{folder}/{Guid.NewGuid()}.jpg";
            var blobClient = new BlobClient(_options.ConnectionString, container, blobName, new BlobClientOptions{ });
            var binaryContent = new BinaryData(outStream.GetBuffer());
            await blobClient.UploadAsync(binaryContent);
            return blobClient.Uri.ToString();
        }
    }
}
