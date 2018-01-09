namespace Common.Contracts.Transportation
{
    public class GetTransportationListResultMessage : IGatewayResultMessage
    {
        public string PayloadSasUri { get; set; }
    }
}