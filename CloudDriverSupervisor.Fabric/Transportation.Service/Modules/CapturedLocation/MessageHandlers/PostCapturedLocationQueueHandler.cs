namespace Transportation.Service.Modules.CapturedLocation.MessageHandlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Domain.Entities;
    using Infrastructure.AutoMapper.Interfaces;
    using Infrastructure.Logging.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public sealed class PostCapturedLocationQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IAutoMapperService _autoMapperService;
        private readonly ICapturedLocationService _capturedLocationService;

        public PostCapturedLocationQueueHandler(
            ILoggingService loggingService,
            IAutoMapperService autoMapperService,
            ICapturedLocationService capturedLocationService
        )
            : base(loggingService.GetLogAction())
        {
            _autoMapperService = autoMapperService;
            _capturedLocationService = capturedLocationService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var capturedLocationsMessage = message.GetBody<PostCapturedLocationsMessage>();
            var capturedLocations =
                _autoMapperService.MapObject<List<CapturedLocation>>(capturedLocationsMessage
                    .CapturedLocations);
            if (capturedLocations == null)
            {
                WriteLog(
                    $"No locations in message payload for transportationId: {capturedLocationsMessage.TransportationId.ToString()}");
                return;
            }

            _capturedLocationService.ProcessCapturedLocations(capturedLocationsMessage.TransportationId,
                capturedLocations);
        }
    }
}