namespace Common.Contracts.Transportation
{
    using System;

    public class PostTransporationResultMessage : IGatewayResultMessage
    {
        public Guid TransportationId { get; set; }
    }
}