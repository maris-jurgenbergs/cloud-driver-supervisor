namespace Gateway.Service.SignalR
{
    using Hubs;
    using Interfaces;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.ServiceBus.Messaging;
    using Modules.ServiceBus.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class TransportationHubOrchestrator : ITransportationHubOrchestrator
    {
        private readonly IHubContext<TransportationHub> _hubContext;
        private readonly QueueClient _transportationSubscriptionClient;

        public TransportationHubOrchestrator(
            IMessageBrokerService messageBrokerService,
            IHubContext<TransportationHub> hubContext)
        {
            _hubContext = hubContext;
            _transportationSubscriptionClient =
                messageBrokerService.BuildQueueClient("Processed-Post-Captured-Locations-Queue");
        }

        public void SubscribeToProcessedTransportations()
        {
            _transportationSubscriptionClient.OnMessageAsync(async message =>
            {
                var sasUriList = message.GetBody<string[]>();
                var serializerSettings =
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var serializedSasUriList = JsonConvert.SerializeObject(sasUriList, Formatting.None, serializerSettings);
                await _hubContext.Clients.All.InvokeAsync("updateTransportationList", serializedSasUriList);
            });
        }
    }
}