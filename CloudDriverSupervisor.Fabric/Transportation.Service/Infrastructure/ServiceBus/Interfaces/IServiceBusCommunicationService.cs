namespace Transportation.Service.Infrastructure.ServiceBus.Interfaces
{
    using System.Threading.Tasks;
    using Common.Contracts;
    using Microsoft.ServiceBus.Messaging;

    public interface IServiceBusCommunicationService
    {
        Task SendBrokeredMessage(BrokeredMessage message, string queueName);
        Task WaitOnBrokeredMessage<T>(string queueName, string sessionId) where T : IGatewayResultMessage;
    }
}