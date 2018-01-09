namespace User.Service.Modules.Role.MessageHandlers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Role;
    using Domain.Repository.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class GetUserRolesQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public GetUserRolesQueueHandler(
            IRoleRepository roleRepository,
            ILoggingService loggingService,
            IServiceBusCommunicationService serviceBusCommunicationService
        )
            : base(loggingService.GetLogAction())
        {
            _roleRepository = roleRepository;
            _serviceBusCommunicationService = serviceBusCommunicationService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var userId = message.GetBody<Guid>();
            var userRole = await _roleRepository.GetUserRoles(userId);
            WriteLog($"Handling queue message {message.MessageId}");
            var payload = new GetUserRolesResultMessage
            {
                Roles = userRole.Select(role => role.RoleType.ToString()).ToArray()
            };

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-User-Roles-Queue");
        }
    }
}