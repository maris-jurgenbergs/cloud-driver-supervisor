namespace Transportation.Service.Modules.Transportation.MessageHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class GetTransportationDetailsQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly ITransportationService _transportationService;

        public GetTransportationDetailsQueueHandler(
            ILoggingService loggingService,
            ITransportationService transportationService,
            IServiceBusCommunicationService serviceBusCommunicationService
        )
            : base(loggingService.GetLogAction())
        {
            _transportationService = transportationService;
            _serviceBusCommunicationService = serviceBusCommunicationService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var transportationId = message.GetBody<Guid>();
            var payload = await _transportationService.GetTransportationDetails(transportationId);

            WriteLog($"Handling queue message {message.MessageId}");

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-Transportation-Details-Queue");
        }
    }
}