namespace Gateway.Service.Controllers.Alert
{
    using System;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceBus.Messaging;
    using Modules.ServiceBus.Interfaces;

    [Route("api/[controller]")]
    [Authorize]
    public class AlertController : Controller
    {
        private readonly IMessageBrokerService _messageBrokerService;

        public AlertController(IMessageBrokerService messageBrokerService)
        {
            _messageBrokerService = messageBrokerService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAlerts()
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-Alert-List-Queue";
            const string processsedQueueName = "Processed-Get-Alert-List-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetAlertListResultMessage>(processsedQueueName,
                    sessionId, 5);
            return Ok(result);
        }

        [HttpGet("{alertId:guid}")]
        public async Task<ActionResult> GetAlert([FromRoute] Guid alertId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(alertId)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-Alert-Queue";
            const string processsedQueueName = "Processed-Get-Alert-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetAlertResultMessage>(processsedQueueName, sessionId,
                    5);
            return Ok(result);
        }

        [HttpPatch("{alertId:guid}/severity/{severityLevel:int}")]
        public async Task<ActionResult> PatchAlertSeverityLevel(
            [FromRoute] Guid alertId,
            [FromRoute] SeverityLevel severityLevel)
        {
            var sessionId = Guid.NewGuid().ToString();
            var patchMessage = new PatchAlertSeverityMessage
            {
                AlertId = alertId,
                SeverityLevel = severityLevel
            };
            var message = new BrokeredMessage(patchMessage)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Patch-Alert-Severity-Queue";
            const string processsedQueueName = "Processed-Patch-Alert-Severity-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            await _messageBrokerService.WaitOnBrokeredMessage<PatchAlertSeverityResultMessage>(processsedQueueName,
                sessionId, 5);
            return NoContent();
        }
    }
}