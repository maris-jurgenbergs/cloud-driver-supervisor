namespace Transportation.Service.Modules.Transportation.MessageHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Domain.Entities;
    using Domain.Repository.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class PostTransportationQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly ITransportationRepository _transportationRepository;

        public PostTransportationQueueHandler(
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
            var userId = message.GetBody<Guid>();
            var transportation = new Transportation
            {
                TransportationId = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds
            };
            await _transportationRepository.SaveTransportation(transportation, userId);
            WriteLog($"Handling queue message {message.MessageId}");

            var payload = new PostTransporationResultMessage
            {
                TransportationId = transportation.TransportationId
            };
            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Post-Transportation-Queue");
        }
    }
}