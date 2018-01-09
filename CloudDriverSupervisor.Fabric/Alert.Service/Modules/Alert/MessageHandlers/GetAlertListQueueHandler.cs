namespace Alert.Service.Modules.Alert.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class GetAlertListQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAlertService _alertService;
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public GetAlertListQueueHandler(
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
            var alertListSasUri = await _alertService.GetAlertListSasUri();
            //var alerts =  await _alertRepository.GetAlerts();
            //var alertResultDtos = _autoMapperService.MapObject<IEnumerable<AlertResultDto>>(alerts);
            var payload = new GetAlertListResultMessage { AlertListSasUri = alertListSasUri };

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-Alert-List-Queue");
        }
    }
}