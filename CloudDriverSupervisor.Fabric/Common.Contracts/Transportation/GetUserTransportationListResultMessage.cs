namespace Common.Contracts.Transportation
{
    public class GetUserTransportationListResultMessage : IGatewayResultMessage
    {
        public string PayloadSasUri { get; set; }
    }
}