namespace Common.Contracts.Transportation
{
    using System;

    public class PatchTransportationMessage
    {
        public Guid TransportationId { get; set; }

        public bool IsActive { get; set; }
    }
}