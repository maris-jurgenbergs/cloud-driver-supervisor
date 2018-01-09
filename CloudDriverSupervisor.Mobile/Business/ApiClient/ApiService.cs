namespace Mobile.Business.ApiClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Bootstrapper.Interfaces;
    using Entities;
    using Interfaces;
    using Modules.Alert.Entities;
    using Modules.Authentication.Interfaces;
    using Modules.Configuration.Interfaces;
    using Modules.DrivingTimeMonitoring.Entities;
    using RestSharp;

    public class ApiService : IApiService, ITransientService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfigurationService _configurationService;

        public ApiService(IConfigurationService configurationService, IAuthenticationService authenticationService)
        {
            _configurationService = configurationService;
            _authenticationService = authenticationService;
        }

        public async Task<IEnumerable<string>> GetUserRoles()
        {
            var client = new RestClient(_configurationService.WebApiUrl);
            var request = new RestRequest(_configurationService.GetUserRolesApiEndpoint, Method.GET);
            request.AddUrlSegment("userId", _authenticationService.GetUserId());
            request.AddHeader("Authorization", await _authenticationService.GetAuthorizationHeaderAsync());

            try
            {
                var response = client.Execute<List<string>>(request);
                var result = response.Data;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Guid> PostTransporation()
        {
            var client = new RestClient(_configurationService.WebApiUrl);
            var request = new RestRequest(_configurationService.PostTransportationApiEndpoint, Method.POST);
            request.AddUrlSegment("userId", _authenticationService.GetUserId());
            request.AddHeader("Authorization", await _authenticationService.GetAuthorizationHeaderAsync());

            try
            {
                var response = await client.ExecuteTaskAsync<Guid>(request);
                var result = response.Data;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task PostCapturedLocations(Guid transportationId, IEnumerable<CapturedLocation> capturedLocations)
        {
            var client = new RestClient(_configurationService.WebApiUrl);
            var request = new RestRequest(_configurationService.PostTransportationCapturedLocationsApiEndpoint,
                Method.POST);
            request.AddUrlSegment("transportationId", transportationId);
            request.AddHeader("Authorization", await _authenticationService.GetAuthorizationHeaderAsync());
            request.AddJsonBody(capturedLocations);

            var response = await client.ExecuteTaskAsync(request);
            if (!response.IsSuccessful)
            {
                //TODO: implement better error handling process
                throw response.ErrorException;
            }
        }

        public async Task<DrivingTimeCalculations> GetDrivingTimeCalculations()
        {
            var client = new RestClient(_configurationService.WebApiUrl);
            var request = new RestRequest(_configurationService.GetUserDrivingTimeApiEndpoint, Method.GET);
            request.AddUrlSegment("userId", _authenticationService.GetUserId());
            request.AddHeader("Authorization", await _authenticationService.GetAuthorizationHeaderAsync());

            try
            {
                var response = await client.ExecuteTaskAsync<DrivingTimeCalculations>(request);
                if (!response.IsSuccessful)
                {
                    throw response.ErrorException;
                }

                return response.Data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task PostAlert(Guid transportationId, AlertDto dto)
        {
            var client = new RestClient(_configurationService.WebApiUrl);
            var request = new RestRequest(_configurationService.PostAlertApiEndpoint, Method.POST);
            request.AddUrlSegment("transportationId", transportationId);
            request.AddHeader("Authorization", await _authenticationService.GetAuthorizationHeaderAsync());
            request.AddJsonBody(dto);

            var response = await client.ExecuteTaskAsync(request);
            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }
        }

        public async Task<IList<AlertResultDto>> GetAlerts(Guid transportationId)
        {
            var client = new RestClient(_configurationService.WebApiUrl);
            var request = new RestRequest(_configurationService.GetAlertsApiEndpoint, Method.GET);
            request.AddUrlSegment("transportationId", transportationId);
            request.AddHeader("Authorization", await _authenticationService.GetAuthorizationHeaderAsync());

            var response = await client.ExecuteTaskAsync<List<AlertResultDto>>(request);
            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }
    }
}