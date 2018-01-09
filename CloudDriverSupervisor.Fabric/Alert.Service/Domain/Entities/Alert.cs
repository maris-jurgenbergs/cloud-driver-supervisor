namespace Alert.Service.Domain.Entities
{
    using System;

    public class Alert
    {
        public Guid AlertId { get; set; }

        public AlertStatus Status { get; set; }

        public AlertType Type { get; set; }

        public SeverityLevel SeverityLevel { get; set; }

        public float CreatedAt { get; set; }

        public string Description { get; set; }
    }
}