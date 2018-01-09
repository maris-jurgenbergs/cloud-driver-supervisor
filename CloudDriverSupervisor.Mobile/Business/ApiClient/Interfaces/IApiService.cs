namespace Mobile.Business.ApiClient.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Modules.Alert.Entities;
    using Modules.DrivingTimeMonitoring.Entities;

    public interface IApiService
    {
        Task<IEnumerable<string>> GetUserRoles();
        Task<Guid> PostTransporation();
        Task PostCapturedLocations(Guid transportationId, IEnumerable<CapturedLocation> capturedLocations);
        Task<DrivingTimeCalculations> GetDrivingTimeCalculations();
        Task PostAlert(Guid transportationId, AlertDto dto);
        Task<IList<AlertResultDto>> GetAlerts(Guid transportationId);
    }
}