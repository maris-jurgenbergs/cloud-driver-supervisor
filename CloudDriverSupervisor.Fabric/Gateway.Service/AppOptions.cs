namespace Gateway.Service
{
    public class AppOptions
    {
        public string ServiceBusConnectionString { get; set; }

        public string DomainName { get; set; }

        public string AADAppId { get; set; }

        public string AADAppSecretKey { get; set; }
    }
}