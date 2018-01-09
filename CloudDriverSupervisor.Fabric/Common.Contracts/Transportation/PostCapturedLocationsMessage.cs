namespace Common.Contracts.Transportation
{
    using System;
    using System.Collections.Generic;

    public class PostCapturedLocationsResultMessage
    {
        public Guid TransportationId { get; set; }

        public IEnumerable<CapturedLocationDto> CapturedLocations { get; set; }
    }
}