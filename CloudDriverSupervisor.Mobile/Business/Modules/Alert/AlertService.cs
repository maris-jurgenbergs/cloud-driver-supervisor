namespace Mobile.Business.Modules.Alert
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ApiClient.Interfaces;
    using Bootstrapper.Interfaces;
    using Configuration.Interfaces;
    using Entities;
    using Interfaces;

    public class AlertService : IAlertService, ISingletonService
    {
        private readonly IApiService _apiService;
        private readonly IConfigurationService _configurationService;

        public AlertService(IApiService apiService, IConfigurationService configurationService)
        {
            _apiService = apiService;
            _configurationService = configurationService;
        }

        public async Task CreateAlert(int alertType, string description)
        {
            var dto = new AlertDto
            {
                Type = alertType,
                Description = description
            };
            var transportationId = _configurationService.GetCurrentTransportationId();
            if (transportationId != null)
            {
                await _apiService.PostAlert(transportationId.Value, dto);
            }
        }

        public async Task<IList<AlertResultDto>> GetAlerts()
        {
            var transportationId = _configurationService.GetCurrentTransportationId();
            if (transportationId != null)
            {
                return await _apiService.GetAlerts(transportationId.Value);
            }

            return new List<AlertResultDto>();
        }
    }
}