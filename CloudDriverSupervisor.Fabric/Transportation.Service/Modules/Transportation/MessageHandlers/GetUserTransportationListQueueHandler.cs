namespace Transportation.Service.Modules.Transportation.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class GetUserTransportationListQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly ITransportationService _transportationService;

        public GetUserTransportationListQueueHandler(
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
            var incomingMessage = message.GetBody<GetUserTransportationListMessage>();
            var transportationListSasUri = await _transportationService.GetUserTransportationListSasUri(
                incomingMessage.UserId,
                incomingMessage.PeriodStart,
                incomingMessage.PeriodEnd);

            WriteLog($"Handling queue message {message.MessageId}");

            var payload = new GetUserTransportationListResultMessage
            {
                PayloadSasUri = transportationListSasUri
            };
            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-User-Transportation-List-Queue");
        }
    }
}