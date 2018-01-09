namespace Gateway.Service.Controllers.User
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Contracts.Role;
    using Common.Contracts.Transportation;
    using Common.Contracts.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceBus.Messaging;
    using Modules.ServiceBus.Interfaces;

    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMessageBrokerService _messageBrokerService;

        public UserController(IMessageBrokerService messageBrokerService)
        {
            _messageBrokerService = messageBrokerService;
        }

        [HttpGet("{userId:guid}/roles")]
        public async Task<ActionResult> GetUserRole([FromRoute] Guid userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(userId)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-User-Roles-Queue";
            const string processsedQueueName = "Processed-Get-User-Roles-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetUserRolesResultMessage>(processsedQueueName,
                    sessionId);
            return Ok(result.Roles);
        }

        [HttpPost("{userId:guid}/roles")]
        public async Task<ActionResult> PostUserRoles([FromRoute] Guid userId, [FromBody] IEnumerable<string> roles)
        {
            var payload = new PostUserRoleMessage
            {
                UserId = userId,
                Roles = roles
            };
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(payload)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Post-User-Roles-Queue";
            const string processsedQueueName = "Processed-Post-User-Roles-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            await _messageBrokerService.WaitOnBrokeredMessage<PostUserRoleResultMessage>(processsedQueueName,
                sessionId);
            return Ok();
        }

        [HttpDelete("{userId:guid}/roles")]
        public async Task<ActionResult> DeleteUserRoles([FromRoute] Guid userId, [FromBody] IEnumerable<string> roles)
        {
            var payload = new DeleteUserRoleMessage
            {
                UserId = userId,
                Roles = roles
            };
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(payload)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Delete-User-Roles-Queue";
            const string processsedQueueName = "Processed-Delete-User-Roles-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            await _messageBrokerService.WaitOnBrokeredMessage<DeleteUserRoleResultMessage>(processsedQueueName,
                sessionId);
            return Ok();
        }

        [HttpPost("{userId:Guid}/transportation")]
        public async Task<ActionResult> PostTransportations([FromRoute] Guid userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(userId)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Post-Transportation-Queue";
            const string processsedQueueName = "Processed-Post-Transportation-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<PostTransporationResultMessage>(processsedQueueName,
                    sessionId);
            return Ok(result.TransportationId);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Get-Users-Queue";
            const string processsedQueueName = "Processed-Get-Users-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            var result =
                await _messageBrokerService.WaitOnBrokeredMessage<GetUsersResultMessage>(processsedQueueName,
                    sessionId);
            return Ok(result);
        }

        [HttpDelete("{userId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(userId)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Delete-User-Queue";
            const string processsedQueueName = "Processed-Delete-User-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            await _messageBrokerService.WaitOnBrokeredMessage<DeleteUserResultMessage>(processsedQueueName, sessionId);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserDto user)
        {
            var sessionId = Guid.NewGuid().ToString();
            var message = new BrokeredMessage(user)
            {
                SessionId = sessionId
            };
            const string queueName = "Incoming-Post-User-Queue";
            const string processsedQueueName = "Processed-Post-User-Queue";
            await _messageBrokerService.SendBrokeredMessage(message, queueName);
            await _messageBrokerService.WaitOnBrokeredMessage<AddUserResultMessage>(processsedQueueName, sessionId);
            return Ok();
        }

        [HttpGet("{userId:guid}/driving-time")]
        public async Task<ActionResult> GetUserDrivingTime([FromRoute] Guid userId)
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
            return Ok(checkResult);
        }
    }
}