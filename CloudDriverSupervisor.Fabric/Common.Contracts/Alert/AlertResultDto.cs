namespace Common.Contracts.Alert
{
    using System;

    public class AlertResultDto
    {
        public Guid AlertId { get; set; }

        public AlertStatus Status { get; set; }

        public AlertType Type { get; set; }

        public SeverityLevel SeverityLevel { get; set; }

        public float CreatedAt { get; set; }

        public string Description { get; set; }
    }
}