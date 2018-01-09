namespace Alert.Service.Infrastructure.Storage.Interfaces
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IAlertBlobService
    {
        Task<string> SaveStreamToAlertListBlob(string blobName, Stream stream);
        Task<string> GetAlertListBlobSasUri(string blobName);
        Task<string> SaveStreamToAlertBlob(string blobName, Stream stream);
        Task<string> GetAlertBlobSasUri(string blobName);
    }
}