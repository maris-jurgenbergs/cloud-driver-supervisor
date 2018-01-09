namespace Alert.Service.Modules.Alert.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Domain.Repositories.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class PatchAlertSeverityQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public PatchAlertSeverityQueueHandler(
            IServiceBusCommunicationService serviceBusCommunicationService,
            IAlertRepository alertRepository)
        {
            _serviceBusCommunicationService = serviceBusCommunicationService;
            _alertRepository = alertRepository;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var incomingMessage = message.GetBody<PatchAlertSeverityMessage>();
            await _alertRepository.UpdateAlertSeverityLevel(incomingMessage.AlertId, incomingMessage.SeverityLevel);
            var payload = new PatchAlertSeverityResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Patch-Alert-Severity-Queue");
        }
    }
}