namespace Common.Contracts.Alert
{
    using System.Collections.Generic;

    public class GetTransportationAlertListResultMessage : IGatewayResultMessage
    {
        public IEnumerable<AlertResultDto> Alerts { get; set; }
    }
}