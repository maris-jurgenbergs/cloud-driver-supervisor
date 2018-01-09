namespace Transportation.Service.Infrastructure.ServiceBus
{
    using System;
    using System.Threading.Tasks;
    using Bootstrapper.Interfaces;
    using Bootstrapper.Options;
    using Common.Contracts;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Microsoft.ServiceBus.Messaging;

    public class ServiceBusCommunicationService : IServiceBusCommunicationService, ISingletonService
    {
        private const int MessageSessionTimeout = 1;
        private readonly IOptions<AppOptions> _appOptions;

        public ServiceBusCommunicationService(IOptions<AppOptions> appOptions)
        {
            _appOptions = appOptions;
        }

        public async Task SendBrokeredMessage(BrokeredMessage message, string queueName)
        {
            var client = BuildQueueClient(queueName);
            await client.SendAsync(message);
        }

        //TODO: handle failed messages
        public async Task WaitOnBrokeredMessage<T>(string queueName, string sessionId) where T : IGatewayResultMessage
        {
            var client = BuildQueueClient(queueName);
            var messageSession = await client.AcceptMessageSessionAsync(sessionId);
            var responseMessage = await messageSession.ReceiveAsync(TimeSpan.FromMinutes(MessageSessionTimeout));
            if (responseMessage != null)
            {
                var result = responseMessage.GetBody<T>();
            }
        }

        private QueueClient BuildQueueClient(string queueName)
        {
            return QueueClient.CreateFromConnectionString(_appOptions.Value.ServiceBusConnectionString, queueName);
        }
    }
}