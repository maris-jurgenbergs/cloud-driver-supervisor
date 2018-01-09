namespace Common.Contracts.User
{
    using System;

    public class GetUserDrivingTimeResultMessage : IGatewayResultMessage
    {
        public TimeSpan DrivingTimePastWeek { get; set; }
        public TimeSpan DrivingTimePastDay { get; set; }
        public TimeSpan DrivingTimeLeftTillNextRestRequired { get; set; }
        public TimeSpan RestTimeLeftTillNextTransportationCanBeStarted { get; set; }
    }
}