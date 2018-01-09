namespace Transportation.Service.Infrastructure.Storage.Interfaces
{
    using System.IO;
    using System.Threading.Tasks;

    public interface ILocationBlobService
    {
        Task<string> SaveStreamToBlob(string blobName, Stream stream);
        Task<string> GetBlobSasUri(string blobName);
    }
}