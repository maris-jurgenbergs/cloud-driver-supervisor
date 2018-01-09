namespace User.Service.Modules.Role.MessageHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Role;
    using Domain.Entities;
    using Domain.Repository.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class DeleteUserRolesQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public DeleteUserRolesQueueHandler(
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
            var deleteUserRoleMessage = message.GetBody<DeleteUserRoleMessage>();
            foreach (var role in deleteUserRoleMessage.Roles)
            {
                await _roleRepository.DeleteUserRole(deleteUserRoleMessage.UserId,
                    (RoleType) Enum.Parse(typeof(RoleType), role));
            }

            WriteLog($"Handling queue message {message.MessageId}");
            var payload = new DeleteUserRoleResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Delete-User-Roles-Queue");
        }
    }
}