namespace Alert.Service.Modules.Violation.MessageHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Contracts.User;
    using Infrastructure.ServiceBus.Interfaces;
    using Interfaces;
    using Microsoft.ServiceBus.Messaging;
    using ServiceFabric.ServiceBus.Services;

    public class GetUserDrivingLimitQueueHandler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly IServiceBusCommunicationService _serviceBusCommunicationService;
        private readonly IViolationCalculationService _violationCalculationService;

        public GetUserDrivingLimitQueueHandler(
            IServiceBusCommunicationService serviceBusCommunicationService,
            IViolationCalculationService violationCalculationService)
        {
            _serviceBusCommunicationService = serviceBusCommunicationService;
            _violationCalculationService = violationCalculationService;
        }

        protected override async Task ReceiveMessageImplAsync(
            BrokeredMessage message,
            MessageSession session,
            CancellationToken cancellationToken)
        {
            var incomingMessage = message.GetBody<GetUserDrivingTimeMessage>();
            var calculationResult =
                await _violationCalculationService.CalculateDrivingLimit(incomingMessage.TransportationListSasUri,
                    incomingMessage.UserId);
            var payload = new GetUserDrivingTimeResultMessage
            {
                DrivingTimePastWeek = calculationResult.DrivingTimePastWeek,
                DrivingTimePastDay = calculationResult.DrivingTimePastDay,
                DrivingTimeLeftTillNextRestRequired = calculationResult.DrivingTimeLeftTillNextRestRequired,
                RestTimeLeftTillNextTransportationCanBeStarted =
                    calculationResult.RestTimeLeftTillNextTransportationCanBeStarted
            };

            await _serviceBusCommunicationService.SendBrokeredMessage(new BrokeredMessage(payload)
            {
                SessionId = message.SessionId
            }, "Processed-Get-User-Driving-Time-Queue");
        }
    }
}