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

    public sealed class PostUserRolesQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;

        public PostUserRolesQueueHandler(
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
            var postUserRoleMessage = message.GetBody<PostUserRoleMessage>();
            foreach (var role in (RoleType[]) Enum.GetValues(typeof(RoleType)))
            {
                await _roleRepository.DeleteUserRole(postUserRoleMessage.UserId,
                    role);
            }

            foreach (var role in postUserRoleMessage.Roles)
            {
                await _roleRepository.AddUserRole(postUserRoleMessage.UserId,
                    (RoleType) Enum.Parse(typeof(RoleType), role));
            }

            WriteLog($"Handling queue message {message.MessageId}");
            var payload = new PostUserRoleResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Post-User-Roles-Queue");
        }
    }
}