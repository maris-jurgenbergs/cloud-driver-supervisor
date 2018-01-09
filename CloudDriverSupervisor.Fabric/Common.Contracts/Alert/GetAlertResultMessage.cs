namespace Common.Contracts.Alert
{
    public class GetAlertResultMessage : IGatewayResultMessage
    {
        public string AlertSasUri { get; set; }
    }
}