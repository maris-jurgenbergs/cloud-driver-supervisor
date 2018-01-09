namespace Common.Contracts.Alert
{
    public class GetAlertListResultMessage : IGatewayResultMessage
    {
        public string AlertListSasUri { get; set; }
    }
}