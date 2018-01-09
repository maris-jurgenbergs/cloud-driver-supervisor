namespace Mobile.Business.Modules.Alert.Entities
{
    public class AlertResultDto
    {
        public AlertStatus Status { get; set; }

        public AlertType Type { get; set; }

        public SeverityLevel SeverityLevel { get; set; }

        public float CreatedAt { get; set; }

        public string Description { get; set; }
    }
}