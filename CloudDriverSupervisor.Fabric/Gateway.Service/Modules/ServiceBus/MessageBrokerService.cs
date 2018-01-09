namespace Gateway.Service.Modules.ServiceBus
{
    using System;
    using System.Threading.Tasks;
    using Common.Contracts;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Microsoft.ServiceBus.Messaging;

    public class MessageBrokerService : IMessageBrokerService
    {
        private const int MessageSessionTimeout = 1;
        private readonly IOptions<AppOptions> _appOptions;

        public MessageBrokerService(IOptions<AppOptions> appOptions)
        {
            _appOptions = appOptions;
        }

        public QueueClient BuildQueueClient(string queueName)
        {
            return QueueClient.CreateFromConnectionString(_appOptions.Value.ServiceBusConnectionString, queueName);
        }

        public async Task SendBrokeredMessage(BrokeredMessage message, string queueName)
        {
            var client = BuildQueueClient(queueName);
            await client.SendAsync(message);
        }

        //TODO: handle failed messages
        public async Task<T> WaitOnBrokeredMessage<T>(string queueName, string sessionId, int? sessionTimeout = null)
            where T : IGatewayResultMessage
        {
            var client = BuildQueueClient(queueName);
            var messageSession = await client.AcceptMessageSessionAsync(sessionId);
            var responseMessage =
                await messageSession.ReceiveAsync(TimeSpan.FromMinutes(sessionTimeout ?? MessageSessionTimeout));
            if (responseMessage != null)
            {
                var result = responseMessage.GetBody<T>();
                await responseMessage.CompleteAsync();
                return await Task.FromResult(result);
            }

            throw new TimeoutException(
                $"Response was not received from \r\nQueue: {queueName}; \r\nSessionId: {sessionId};");
        }
    }
}