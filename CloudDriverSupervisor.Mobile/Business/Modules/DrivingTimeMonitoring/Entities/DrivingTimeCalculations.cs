namespace Mobile.Business.Modules.DrivingTimeMonitoring.Entities
{
    using System;

    public class DrivingTimeCalculations
    {
        public TimeSpan DrivingTimePastWeek { get; set; }

        public TimeSpan DrivingTimePastDay { get; set; }

        public TimeSpan DrivingTimeLeftTillNextRestRequired { get; set; }

        public TimeSpan RestTimeLeftTillNextTransportationCanBeStarted { get; set; }
    }
}