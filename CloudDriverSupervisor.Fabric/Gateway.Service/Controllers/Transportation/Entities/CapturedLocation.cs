namespace Gateway.Service.Controllers.Transportation.Entities
{
    using System;

    public class CapturedLocation
    {
        public double Altitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CapturedDateTimeUtc { get; set; }
    }
}