namespace Alert.Service.Modules.Alert.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Domain.Entities;
    using Domain.Repositories.Interfaces;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class PostAlertQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public PostAlertQueueHandler(
            IServiceBusCommunicationService serviceBusCommunicationService,
            IAlertRepository alertRepository,
            IAutoMapperService autoMapperService)
        {
            _serviceBusCommunicationService = serviceBusCommunicationService;
            _alertRepository = alertRepository;
            _autoMapperService = autoMapperService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var incomingMessage = message.GetBody<PostAlertMessage>();
            var alert = _autoMapperService.MapObject<Alert>(incomingMessage.AlertDto);
            await _alertRepository.SaveAlert(alert, incomingMessage.TransportationId);
            var payload = new PostAlertResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Post-Alert-Queue");
        }
    }
}