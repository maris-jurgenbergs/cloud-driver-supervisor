namespace Gateway.Service.Modules.ServiceBus.Interfaces
{
    using System.Threading.Tasks;
    using Common.Contracts;
    using Microsoft.ServiceBus.Messaging;

    public interface IMessageBrokerService
    {
        QueueClient BuildQueueClient(string queueName);

        Task SendBrokeredMessage(BrokeredMessage message, string queueName);

        Task<T> WaitOnBrokeredMessage<T>(string queueName, string sessionId, int? sessionTimeout = null)
            where T : IGatewayResultMessage;
    }
}