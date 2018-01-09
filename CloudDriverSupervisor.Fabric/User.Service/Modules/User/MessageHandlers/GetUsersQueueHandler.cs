namespace User.Service.Modules.User.MessageHandlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.User;
    using Domain.Repository.Interfaces;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.ServiceBus.Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class GetUsersQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAutoMapperService _autoMapperService;
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly IUserRepository _userRepository;

        public GetUsersQueueHandler(
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
            var users = await _userRepository.GetUsers();
            var userDtoList = _autoMapperService.MapObject<List<UserDto>>(users);
            WriteLog($"Handling queue message {message.MessageId}");
            var payload = new GetUsersResultMessage
            {
                UserDtos = userDtoList
            };

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-Users-Queue");
        }
    }
}