namespace Mobile.Business.Modules.Configuration
{
    using System;
    using Bootstrapper.Interfaces;
    using Interfaces;

    public class ConfigurationService : IConfigurationService, ISingletonService
    {
        public string ClientId => "400cdd93-8e28-4745-9d1c-3c02068fdcbb";

        public string CommonAuthority =>
            "https://login.microsoftonline.com/740c233a-4a54-4241-8c89-949d116bf5e3/oauth2/authorize";

        public string GraphResourceUri => "https://graph.windows.net";

        public Uri ReturnUri => new Uri("http://CloudDriverSupervisor");

        private Guid? CurrentTransportationId { get; set; }

        //public string WebApiUrl => "https://192.168.88.161:8419/api/";
        public string WebApiUrl => "https://cdsfabric.westeurope.cloudapp.azure.com:8419/api/";

        public string GetUserRolesApiEndpoint => "user/{userId}/roles";

        public string PostTransportationApiEndpoint => "user/{userId}/transportation";

        public string PostTransportationCapturedLocationsApiEndpoint =>
            "transportation/{transportationId}/captured-locations";

        public string GetUserDrivingTimeApiEndpoint => "user/{userId}/driving-time";

        public string PostAlertApiEndpoint => "transportation/{transportationId}/alert";

        public string GetAlertsApiEndpoint => "transportation/{transportationId}/alert";

        public void SetCurrentTransportationId(Guid transportationId)
        {
            CurrentTransportationId = transportationId;
        }

        public Guid? GetCurrentTransportationId()
        {
            return CurrentTransportationId;
        }
    }
}