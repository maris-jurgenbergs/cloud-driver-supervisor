namespace Transportation.Service.Infrastructure.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Bootstrapper.Interfaces;
    using Bootstrapper.Options;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class LocationBlobService : ILocationBlobService, ITransientService
    {
        private const string LocationCacheContainerName = "location-cache";
        private readonly CloudBlobContainer _locationCacheBlobContainer;

        public LocationBlobService(IOptions<AppOptions> appOptions)
        {
            var account = CloudStorageAccount.Parse(appOptions.Value.StorageConnectionString);
            var blobClient = account.CreateCloudBlobClient();
            _locationCacheBlobContainer = blobClient.GetContainerReference(LocationCacheContainerName);
        }

        public async Task<string> SaveStreamToBlob(string blobName, Stream stream)
        {
            var blobReference = _locationCacheBlobContainer.GetBlockBlobReference(blobName);
            if (await blobReference.ExistsAsync() &&
                blobReference.Properties.LastModified < DateTimeOffset.UtcNow.AddMinutes(-2))
            {
                blobReference.Delete();
            }

            await blobReference.UploadFromStreamAsync(stream);
            return blobName;
        }

        public async Task<string> GetBlobSasUri(string blobName)
        {
            var blobReference = await _locationCacheBlobContainer.GetBlobReferenceFromServerAsync(blobName);
            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(2)
            };
            return blobReference.Uri + blobReference.GetSharedAccessSignature(policy);
        }
    }
}