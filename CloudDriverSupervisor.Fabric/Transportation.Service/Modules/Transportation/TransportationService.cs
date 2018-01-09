namespace Transportation.Service.Modules.Transportation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Domain.Entities;
    using Domain.Repository.Interfaces;
    using Infrastructure.Bootstrapper.Interfaces;
    using Infrastructure.Storage.Interfaces;
    using Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class TransportationService : ITransportationService, ISingletonService
    {
        private readonly ILocationBlobService _locationBlobService;
        private readonly ITransportationRepository _transportationRepository;

        public TransportationService(
            ITransportationRepository transportationRepository,
            ILocationBlobService locationBlobService)
        {
            _transportationRepository = transportationRepository;
            _locationBlobService = locationBlobService;
        }

        public async Task<string> GetTransportationListSasUri(DateTime periodStart, DateTime periodEnd)
        {
            var transportations = await _transportationRepository.GetTransportations(periodStart, periodEnd);
            return await GenerateSasUri(periodStart, periodEnd, transportations);
        }

        public async Task<string> GetUserTransportationListSasUri(Guid userId, DateTime periodStart, DateTime periodEnd)
        {
            var transportations =
                await _transportationRepository.GetUserTransportations(userId, periodStart, periodEnd);
            return await GenerateSasUri(periodStart, periodEnd, transportations);
        }

        public async Task<IEnumerable<string>> GetTransportationListSasUri(
            IEnumerable<Dictionary<Guid, List<CapturedLocation>>> listOfParsedDictionaries)
        {
            var serializerSettings =
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var sasUriList = new List<string>();
            foreach (var parsedDictionary in listOfParsedDictionaries)
            {
                var transportationsList = new List<dynamic>();
                foreach (var pair in parsedDictionary)
                {
                    transportationsList.Add(new
                    {
                        transportation = new Transportation { TransportationId = pair.Key },
                        capturedLocations = pair.Value
                    });
                }

                var serializedTransportationsList =
                    JsonConvert.SerializeObject(transportationsList, Formatting.None, serializerSettings);

                var blobName = $"signalr-cache/{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-ff}.json";
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedTransportationsList)))
                {
                    await _locationBlobService.SaveStreamToBlob(blobName, stream);
                }

                var sasUri = await _locationBlobService.GetBlobSasUri(blobName);
                sasUriList.Add(sasUri);
            }

            return sasUriList;
        }

        public async Task<GetTransportationDetailsResultMessage> GetTransportationDetails(Guid transportationId)
        {
            var transportationDetails = await _transportationRepository.GetTransportationDetails(transportationId);
            return transportationDetails;
        }

        private async Task<string> GenerateSasUri(DateTime periodStart, DateTime periodEnd, dynamic transportations)
        {
            var serializerSettings =
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var serializedTransportationsList =
                JsonConvert.SerializeObject(transportations, Formatting.None, serializerSettings);
            var blobName = $"{periodStart:dd-MM-yyyy-HH-mm-ss-ff}-{periodEnd:dd-MM-yyyy-HH-mm-ss-ff}.json";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedTransportationsList)))
            {
                await _locationBlobService.SaveStreamToBlob(blobName, stream);
            }

            var sasUri = await _locationBlobService.GetBlobSasUri(blobName);
            return sasUri;
        }
    }
}