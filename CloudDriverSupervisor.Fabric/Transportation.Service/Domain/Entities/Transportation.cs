namespace Transportation.Service.Domain.Entities
{
    using System;

    public class Transportation
    {
        public Guid TransportationId { get; set; }

        public bool IsActive { get; set; }

        public double CreatedAt { get; set; }
    }
}