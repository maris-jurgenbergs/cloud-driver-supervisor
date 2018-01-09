namespace Alert.Service.Modules.Alert.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IAlertService
    {
        Task<string> GetAlertListSasUri();
        Task<string> GetAlertSasUri(Guid alertId);
    }
}