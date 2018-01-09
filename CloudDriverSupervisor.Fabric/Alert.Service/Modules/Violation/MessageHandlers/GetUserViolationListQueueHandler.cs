namespace Alert.Service.Modules.Violation.MessageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.User;
    using Common.Contracts.Violation;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class GetUserViolationListQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly IViolationService _violationService;

        public GetUserViolationListQueueHandler(
            IServiceBusCommunicationService serviceBusCommunicationService,
            IViolationService violationService,
            IAutoMapperService autoMapperService)
        {
            _serviceBusCommunicationService = serviceBusCommunicationService;
            _violationService = violationService;
            _autoMapperService = autoMapperService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var userId = message.GetBody<Guid>();
            var userViolations = await _violationService.GetUserViolations(userId);
            var violationDtos = _autoMapperService.MapObject<List<ViolationDto>>(userViolations.ToList());
            var payload = new GetUserViolationListResultMessage
            {
                Violations = violationDtos
            };
            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-User-Violation-List-Queue");
        }
    }
}