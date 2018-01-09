namespace Alert.Service.Modules.Alert.MessageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Domain.Repositories.Interfaces;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class GetTransportationAlertListQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public GetTransportationAlertListQueueHandler(
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
            var transportationId = message.GetBody<Guid>();
            var alerts = await _alertRepository.GetTransportationAlerts(transportationId);
            var alertResultDtos = _autoMapperService.MapObject<IEnumerable<AlertResultDto>>(alerts);
            var payload = new GetTransportationAlertListResultMessage { Alerts = alertResultDtos };

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-Transportation-Alert-List-Queue");
        }
    }
}