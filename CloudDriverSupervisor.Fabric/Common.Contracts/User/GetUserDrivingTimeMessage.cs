namespace Common.Contracts.User
{
    using System;

    public class GetUserDrivingTimeMessage
    {
        public Guid UserId { get; set; }

        public string TransportationListSasUri { get; set; }
    }
}