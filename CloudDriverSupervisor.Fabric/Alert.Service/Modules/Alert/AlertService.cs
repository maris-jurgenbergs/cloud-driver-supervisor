namespace Alert.Service.Modules.Alert
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Repositories.Interfaces;
    using Infrastructure.Bootstrapper.Interfaces;
    using Infrastructure.Storage.Interfaces;
    using Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class AlertService : IAlertService, ISingletonService
    {
        private readonly IAlertBlobService _alertBlobService;
        private readonly IAlertRepository _alertRepository;

        public AlertService(IAlertRepository alertRepository, IAlertBlobService alertBlobService)
        {
            _alertRepository = alertRepository;
            _alertBlobService = alertBlobService;
        }

        public async Task<string> GetAlertListSasUri()
        {
            var alerts = await _alertRepository.GetAlerts();
            var blobName = $"{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-ff}-{Guid.NewGuid()}.json";
            var serializerSettings =
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var serializedAlertList = JsonConvert.SerializeObject(alerts, Formatting.None, serializerSettings);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedAlertList)))
            {
                await _alertBlobService.SaveStreamToAlertListBlob(blobName, stream);
            }

            var blobSasUri = await _alertBlobService.GetAlertListBlobSasUri(blobName);
            return blobSasUri;
        }

        public async Task<string> GetAlertSasUri(Guid alertId)
        {
            var alert = await _alertRepository.GetAlert(alertId);
            var blobName = $"{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-ff}-{Guid.NewGuid()}.json";
            var serializerSettings =
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var serializedAlertList = JsonConvert.SerializeObject(alert, Formatting.None, serializerSettings);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedAlertList)))
            {
                await _alertBlobService.SaveStreamToAlertBlob(blobName, stream);
            }

            var blobSasUri = await _alertBlobService.GetAlertBlobSasUri(blobName);
            return blobSasUri;
        }
    }
}