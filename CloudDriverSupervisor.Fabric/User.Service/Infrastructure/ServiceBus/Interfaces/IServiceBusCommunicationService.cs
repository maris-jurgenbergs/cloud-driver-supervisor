namespace User.Service.Infrastructure.ServiceBus.Interfaces
{
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;

    public interface IServiceBusCommunicationService
    {
        Task SendBrokeredMessage(BrokeredMessage message, string queueName);
    }
}