namespace Common.Contracts.Transportation
{
    using System;

    public class CapturedLocationDto
    {
        public double Altitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CapturedDateTimeUtc { get; set; }
    }
}