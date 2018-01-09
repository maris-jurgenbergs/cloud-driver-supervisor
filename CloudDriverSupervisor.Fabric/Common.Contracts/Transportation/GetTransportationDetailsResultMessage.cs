namespace Common.Contracts.Transportation
{
    using System.Collections.Generic;
    using Alert;
    using User;
    using Violation;

    public class GetTransportationDetailsResultMessage : IGatewayResultMessage
    {
        public UserDto User { get; set; }

        public IEnumerable<ViolationDto> Violations { get; set; }

        public IEnumerable<AlertResultDto> Alerts { get; set; }
    }
}