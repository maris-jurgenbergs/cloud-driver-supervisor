namespace Transportation.Service.Modules.Transportation.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Domain.Entities;
    using Domain.Repository.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class PatchTransportationQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly ITransportationRepository _transportationRepository;

        public PatchTransportationQueueHandler(
            ITransportationRepository transportationRepository,
            ILoggingService loggingService,
            IServiceBusCommunicationService serviceBusCommunicationService
        )
            : base(loggingService.GetLogAction())
        {
            _transportationRepository = transportationRepository;
            _serviceBusCommunicationService = serviceBusCommunicationService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var incomingMessage = message.GetBody<PatchTransportationMessage>();
            var transportation = new Transportation
            {
                TransportationId = incomingMessage.TransportationId,
                IsActive = incomingMessage.IsActive
            };
            await _transportationRepository.PatchTransportationStatus(transportation);
            WriteLog($"Handling queue message {message.MessageId}");

            var payload = new PatchTransportationResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Patch-Transportation-Queue");
        }
    }
}