namespace Mobile.Business.Modules.Configuration.Interfaces
{
    using System;

    public interface IConfigurationService
    {
        string ClientId { get; }

        string CommonAuthority { get; }

        string GraphResourceUri { get; }

        Uri ReturnUri { get; }

        string WebApiUrl { get; }

        string GetUserRolesApiEndpoint { get; }

        string PostTransportationApiEndpoint { get; }

        string PostTransportationCapturedLocationsApiEndpoint { get; }

        string GetUserDrivingTimeApiEndpoint { get; }

        string PostAlertApiEndpoint { get; }

        string GetAlertsApiEndpoint { get; }

        void SetCurrentTransportationId(Guid transportationId);
        Guid? GetCurrentTransportationId();
    }
}