namespace Common.Contracts.Transportation
{
    using System;

    public class GetUserTransportationListMessage
    {
        public Guid UserId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}