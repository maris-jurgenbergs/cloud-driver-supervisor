namespace User.Service.Modules.User.MessageHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.User;
    using Domain.Repository.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class DeleteUserQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly IUserRepository _userRepository;

        public DeleteUserQueueHandler(
            IUserRepository userRepository,
            ILoggingService loggingService,
            IServiceBusCommunicationService serviceBusCommunicationService
        )
            : base(loggingService.GetLogAction())
        {
            _userRepository = userRepository;
            _serviceBusCommunicationService = serviceBusCommunicationService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var userId = message.GetBody<Guid>();
            await _userRepository.DeleteUser(userId);
            WriteLog($"Handling queue message {message.MessageId}");
            var payload = new DeleteUserResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Delete-User-Queue");
        }
    }
}