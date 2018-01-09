namespace Transportation.Service.Domain.Entities
{
    public class CapturedLocation
    {
        public double Altitude { get; set; }

        public double Longitude { get; set; }

        // elapsed time in seconds since 1969
        public double CapturedDateTimeUtc { get; set; }
    }
}