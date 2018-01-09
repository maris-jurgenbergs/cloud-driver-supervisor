namespace User.Service.Infrastructure.ServiceBus
{
    using System.Threading.Tasks;
    using Bootstrapper.Options;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Microsoft.ServiceBus.Messaging;

    public class ServiceBusCommunicationService : IServiceBusCommunicationService
    {
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

        private QueueClient BuildQueueClient(string queueName)
        {
            return QueueClient.CreateFromConnectionString(_appOptions.Value.ServiceBusConnectionString, queueName);
        }
    }
}