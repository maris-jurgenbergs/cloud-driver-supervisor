namespace Alert.Service.Modules.Violation.Entities
{
    using System.Collections.Generic;

    public class TransportationResult
    {
        public Transportation Transportation { get; set; }

        public List<CapturedLocation> CapturedLocations { get; set; }
    }
}