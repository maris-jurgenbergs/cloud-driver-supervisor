namespace User.Service.Modules.User.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.User;
    using Domain.Entities;
    using Domain.Repository.Interfaces;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class PostUserQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly IUserRepository _userRepository;

        public PostUserQueueHandler(
            IUserRepository userRepository,
            ILoggingService loggingService,
            IServiceBusCommunicationService serviceBusCommunicationService,
            IAutoMapperService autoMapperService
        )
            : base(loggingService.GetLogAction())
        {
            _userRepository = userRepository;
            _serviceBusCommunicationService = serviceBusCommunicationService;
            _autoMapperService = autoMapperService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var userDto = message.GetBody<UserDto>();
            var user = _autoMapperService.MapObject<User>(userDto);
            await _userRepository.AddUser(user);
            WriteLog($"Handling queue message {message.MessageId}");
            var payload = new AddUserResultMessage();

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Post-User-Queue");
        }
    }
}