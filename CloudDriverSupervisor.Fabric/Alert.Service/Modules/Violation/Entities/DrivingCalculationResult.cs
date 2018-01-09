namespace Alert.Service.Modules.Violation.Entities
{
    using System;

    public class DrivingCalculationResult
    {
        public TimeSpan DrivingTimePastWeek { get; set; }

        public TimeSpan DrivingTimePastDay { get; set; }

        public TimeSpan DrivingTimeLeftTillNextRestRequired { get; set; }

        public TimeSpan RestTimeLeftTillNextTransportationCanBeStarted { get; set; }
    }
}