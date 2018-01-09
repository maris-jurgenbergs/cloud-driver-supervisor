namespace Common.Contracts.Transportation
{
    using System;
    using System.Collections.Generic;

    public class PostCapturedLocationsMessage
    {
        public Guid TransportationId { get; set; }

        public IEnumerable<CapturedLocationDto> CapturedLocations { get; set; }
    }
}