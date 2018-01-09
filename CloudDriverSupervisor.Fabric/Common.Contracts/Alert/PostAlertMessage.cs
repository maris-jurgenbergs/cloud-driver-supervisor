namespace Common.Contracts.Alert
{
    using System;

    public class PostAlertMessage
    {
        public AlertDto AlertDto { get; set; }

        public Guid TransportationId { get; set; }
    }
}