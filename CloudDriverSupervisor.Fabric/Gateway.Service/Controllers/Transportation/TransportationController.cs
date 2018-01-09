namespace Gateway.Service.Controllers.Transportation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Common.Contracts.Transportation;
    using Common.Contracts.User;
    using Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceBus.Messaging;
    using Modules.AutoMapper.Interfaces;
    using Modules.ServiceBus.Interfaces;

    [Route("api/[controller]")]
    [Authorize]
    public class TransportationController : Controller
    {
        private readonly IAutoMapperService _autoMapperService;
        private readonly IMessageBrokerService _messageBrokerService;

        public TransportationController(
            IMessageBrokerService messageBrokerService,
            IAutoMapperService autoMapperService)
        {
            _messageBrokerService = messageBrokerService;
            _autoMapperService = autoMapperService;
        }

        [HttpPost("{transportationId:guid}/captured-locations")]
        public async Task<ActionResult> Post(
            [FromRoute] Guid transportationId,
            [FromBody] IEnumerable<CapturedLocation> capturedLocations)
        {
            var capturedLocationDtos =
                _autoMapperService.MapObject<IEnumerable<CapturedLocationDto>>(capturedLocations);
            var postMessage = new PostCapturedLocationsMessage
            {
                TransportationId = transportationId,
                CapturedLocations = capturedLocationDtos
            };
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(postMessage)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Post-Captured-Locations-Queue";
            // Processed queue currently not in use because there is no need to wait for a response from the microservice. The response can be delivered back, that message was sent successfully.
            //const string processsedQueueName = "Processed-Post-Captured-Locations-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            return NoContent();
        }

        [HttpGet("all/{periodStart:datetime}/{periodEnd:datetime}")]
        public async Task<ActionResult> Get([FromRoute] DateTime periodStart, [FromRoute] DateTime periodEnd)
        {
            var getMessage = new GetTransportationListMessage
            {
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            };
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(getMessage)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-Transportation-List-Queue";
            const string processsedQueueName = "Processed-Get-Transportation-List-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetTransportationListResultMessage>(
                    processsedQueueName, sessionId, 5);
            return Ok(result);
        }

        [HttpPatch("{transportationId:guid}/is-active")]
        public async Task<ActionResult> Patch([FromRoute] Guid transportationId, [FromBody] bool isActive)
        {
            var sessionId = Guid.NewGuid().ToString();
            var patchMessage =
                new PatchTransportationMessage { TransportationId = transportationId, IsActive = isActive };
            var message = new BrokeredMessage(patchMessage)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Patch-Transportation-Queue";
            const string processsedQueueName = "Processed-Patch-Transportation-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<PatchTransportationResultMessage>(processsedQueueName,
                    sessionId, 5);
            return Ok(result);
        }

        [HttpPost("{transportationId:guid}/alert")]
        public async Task<ActionResult> PostAlert([FromRoute] Guid transportationId, [FromBody] AlertDto alertDto)
        {
            var sessionId = Guid.NewGuid().ToString();
            var postMessage = new PostAlertMessage
            {
                AlertDto = alertDto,
                TransportationId = transportationId
            };
            var message = new BrokeredMessage(postMessage)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Post-Alert-Queue";
            const string processsedQueueName = "Processed-Post-Alert-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            await _messageBrokerService.WaitOnBrokeredMessage<PostAlertResultMessage>(processsedQueueName, sessionId,
                5);
            return NoContent();
        }

        [HttpGet("{transportationId:guid}/alert")]
        public async Task<ActionResult> GetAlerts([FromRoute] Guid transportationId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(transportationId)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-Transportation-Alert-List-Queue";
            const string processsedQueueName = "Processed-Get-Transportation-Alert-List-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetTransportationAlertListResultMessage>(
                    processsedQueueName, sessionId, 5);
            return Ok(result.Alerts);
        }

        [HttpGet("{transportationId:guid}/details")]
        public async Task<ActionResult> GetTransportationDetails([FromRoute] Guid transportationId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(transportationId)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-Transportation-Details-Queue";
            const string processsedQueueName = "Processed-Get-Transportation-Details-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetTransportationDetailsResultMessage>(
                    processsedQueueName, sessionId, 5);

            //BAD PATTERN COPY CODE FROM HERE
            var time = await GetTime(result.User.AzureId);

            return Ok(new
            {
                fullName = $"{result.User.Name} {result.User.Surname}",
                phone = result.User.Phone,
                drivingTime = time,
                alerts = result.Alerts,
                violations = result.Violations
            });
        }

        private async Task<TimeSpan> GetTime(Guid userId)
        {
            var getMessage = new GetUserTransportationListMessage
            {
                UserId = userId,
                PeriodStart = DateTime.UtcNow.AddDays(-7),
                PeriodEnd = DateTime.UtcNow
            };
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(getMessage)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-User-Transportation-List-Queue";
            const string processsedQueueName = "Processed-Get-User-Transportation-List-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetUserTransportationListResultMessage>(
                    processsedQueueName, sessionId, 5);

            var checkMessage = new GetUserDrivingTimeMessage
            {
                UserId = userId,
                TransportationListSasUri = result.PayloadSasUri
            };
            var checkSessionId = Guid.NewGuid().ToString();
            var checkUserViolationsMessage = new BrokeredMessage(checkMessage)
            {
                SessionId = checkSessionId
            };
            const string checkQueueName = "Incoming-Get-User-Driving-Time-Queue";
            const string checkProcesssedQueueName = "Processed-Get-User-Driving-Time-Queue";

            await _messageBrokerService.SendBrokeredMessage(checkUserViolationsMessage, checkQueueName);
            var checkResult =
                await _messageBrokerService.WaitOnBrokeredMessage<GetUserDrivingTimeResultMessage>(
                    checkProcesssedQueueName, checkSessionId, 5);
            return checkResult.DrivingTimeLeftTillNextRestRequired;
        }
    }
}