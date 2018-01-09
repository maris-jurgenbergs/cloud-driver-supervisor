namespace Mobile.Business.Modules.Alert.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public interface IAlertService
    {
        Task CreateAlert(int alertType, string description);
        Task<IList<AlertResultDto>> GetAlerts();
    }
}