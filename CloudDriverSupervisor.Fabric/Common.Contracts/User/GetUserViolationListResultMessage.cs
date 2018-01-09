namespace Common.Contracts.User
{
    using System.Collections.Generic;
    using Violation;

    public class GetUserViolationListResultMessage : IGatewayResultMessage
    {
        public List<ViolationDto> Violations { get; set; }
    }
}