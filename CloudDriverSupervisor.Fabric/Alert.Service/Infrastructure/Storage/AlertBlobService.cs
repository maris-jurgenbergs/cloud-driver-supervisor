namespace Alert.Service.Infrastructure.Storage
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

    public class AlertBlobService : IAlertBlobService, ITransientService
    {
        private const string AlertListCacheContainerName = "alert-list-cache";
        private const string AlertCacheContainerName = "alert-cache";
        private readonly CloudBlobContainer _alertCacheBlobContainer;
        private readonly CloudBlobContainer _alertListCacheBlobContainer;

        public AlertBlobService(IOptions<AppOptions> appOptions)
        {
            var account = CloudStorageAccount.Parse(appOptions.Value.StorageConnectionString);
            var blobClient = account.CreateCloudBlobClient();
            _alertListCacheBlobContainer = blobClient.GetContainerReference(AlertListCacheContainerName);
            _alertCacheBlobContainer = blobClient.GetContainerReference(AlertCacheContainerName);
        }

        public async Task<string> SaveStreamToAlertListBlob(string blobName, Stream stream)
        {
            var blobReference = _alertListCacheBlobContainer.GetBlockBlobReference(blobName);
            if (await blobReference.ExistsAsync() &&
                blobReference.Properties.LastModified < DateTimeOffset.UtcNow.AddMinutes(-2))
            {
                blobReference.Delete();
            }

            await blobReference.UploadFromStreamAsync(stream);
            return blobName;
        }

        public async Task<string> GetAlertListBlobSasUri(string blobName)
        {
            var blobReference = await _alertListCacheBlobContainer.GetBlobReferenceFromServerAsync(blobName);
            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(2)
            };
            return blobReference.Uri + blobReference.GetSharedAccessSignature(policy);
        }

        public async Task<string> SaveStreamToAlertBlob(string blobName, Stream stream)
        {
            var blobReference = _alertCacheBlobContainer.GetBlockBlobReference(blobName);
            if (await blobReference.ExistsAsync() &&
                blobReference.Properties.LastModified < DateTimeOffset.UtcNow.AddMinutes(-2))
            {
                blobReference.Delete();
            }

            await blobReference.UploadFromStreamAsync(stream);
            return blobName;
        }

        public async Task<string> GetAlertBlobSasUri(string blobName)
        {
            var blobReference = await _alertCacheBlobContainer.GetBlobReferenceFromServerAsync(blobName);
            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(2)
            };
            return blobReference.Uri + blobReference.GetSharedAccessSignature(policy);
        }
    }
}