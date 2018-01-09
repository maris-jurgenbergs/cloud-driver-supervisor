namespace Alert.Service.Domain.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using SeverityLevel = Common.Contracts.Alert.SeverityLevel;

    public interface IAlertRepository
    {
        Task SaveAlert(Alert alert, Guid transportationId);
        Task<IEnumerable<Alert>> GetTransportationAlerts(Guid transportationId);
        Task<dynamic> GetAlerts();
        Task<dynamic> GetAlert(Guid alertId);
        Task UpdateAlertSeverityLevel(Guid alertId, SeverityLevel severityLevel);
    }
}