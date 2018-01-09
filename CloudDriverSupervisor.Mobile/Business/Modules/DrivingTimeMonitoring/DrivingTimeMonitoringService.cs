namespace Mobile.Business.Modules.DrivingTimeMonitoring
{
    using System.Threading.Tasks;
    using ApiClient.Interfaces;
    using Bootstrapper.Interfaces;
    using Entities;
    using Interfaces;

    public class DrivingTimeMonitoringService : IDrivingTimeMonitoringService, ISingletonService
    {
        private readonly IApiService _apiService;

        public DrivingTimeMonitoringService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<DrivingTimeCalculations> GetDrivingTime()
        {
            var drivingTimeCalculations = await _apiService.GetDrivingTimeCalculations();
            return drivingTimeCalculations;
        }
    }
}