namespace Gateway.Service.AzureAd
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public class AzureActiveDirectoryService : IAzureActiveDirectoryService
    {
        private readonly IOptions<AppOptions> _appOptions;
        private readonly string _authUri;

        private readonly string _serviceAddress;

        public AzureActiveDirectoryService(IOptions<AppOptions> appOptions)
        {
            _appOptions = appOptions;
            _serviceAddress = $"https://graph.windows.net/{_appOptions.Value.DomainName}";
            _authUri = $"https://login.microsoftonline.com/{_appOptions.Value.DomainName}";
        }

        public async Task<IEnumerable<dynamic>> GetUsers()
        {
            var adClient = GetClient();
            var collection = await adClient.Users.ExecuteAsync();
            return collection.CurrentPage.Select(user => new
            {
                Name = user.GivenName,
                user.Surname,
                Email = user.Mail ?? user.UserPrincipalName,
                user.TelephoneNumber,
                AzureId = user.ObjectId
            });
        }

        private ActiveDirectoryClient GetClient()
        {
            return new ActiveDirectoryClient(
                new Uri(_serviceAddress), async () => await GetAccessToken());
        }

        private async Task<string> GetAccessToken()
        {
            var authenticationContext = new AuthenticationContext(_authUri);
            var clientCred = new ClientCredential(_appOptions.Value.AADAppId, _appOptions.Value.AADAppSecretKey);

            var authenticationResult =
                await authenticationContext.AcquireTokenAsync("https://graph.windows.net", clientCred);
            var token = authenticationResult.AccessToken;
            return token;
        }
    }
}