namespace Alert.Service.Modules.Alert.MessageHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class GetAlertQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAlertService _alertService;
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public GetAlertQueueHandler(
            IServiceBusCommunicationService serviceBusCommunicationService,
            IAlertService alertService,
            IAutoMapperService autoMapperService)
        {
            _serviceBusCommunicationService = serviceBusCommunicationService;
            _alertService = alertService;
            _autoMapperService = autoMapperService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var alertId = message.GetBody<Guid>();
            var alertSasUri = await _alertService.GetAlertSasUri(alertId);
            var payload = new GetAlertResultMessage { AlertSasUri = alertSasUri };

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-Alert-Queue");
        }
    }
}