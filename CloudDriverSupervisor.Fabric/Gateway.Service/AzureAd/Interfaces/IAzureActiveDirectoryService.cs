namespace Gateway.Service.AzureAd.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAzureActiveDirectoryService
    {
        Task<IEnumerable<object>> GetUsers();
    }
}