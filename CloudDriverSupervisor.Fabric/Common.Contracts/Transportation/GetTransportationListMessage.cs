namespace Common.Contracts.Transportation
{
    using System;

    public class GetTransportationListMessage
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}