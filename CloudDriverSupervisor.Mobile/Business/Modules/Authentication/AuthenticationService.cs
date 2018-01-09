namespace Mobile.Business.Modules.Authentication
{
    using System;
    using System.Threading.Tasks;
    using Bootstrapper.Interfaces;
    using Configuration.Interfaces;
    using Interfaces;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public class AuthenticationService : IAuthenticationService, ISingletonService
    {
        private readonly IConfigurationService _configurationService;
        private AuthenticationContext _authenticationContext;
        private Guid _userId;

        public AuthenticationService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<string> GetAuthorizationHeaderAsync()
        {
            if (_authenticationContext == null)
            {
                throw new InvalidOperationException("Authentication context has not been initialized");
            }

            var authenticationResult = await _authenticationContext.AcquireTokenSilentAsync(_configurationService.GraphResourceUri, _configurationService.ClientId);
            return authenticationResult.CreateAuthorizationHeader();
        }

        public Guid GetUserId()
        {
            if (_userId == null)
            {
                throw new InvalidOperationException("Authentication context has not been initialized");
            }

            return _userId;
        }

        public async Task<bool> AuthenticateUserAsync(IPlatformParameters parent)
        {
            _authenticationContext = new AuthenticationContext(_configurationService.CommonAuthority);
            try
            {
                var authResult =
                    await _authenticationContext.AcquireTokenAsync(_configurationService.GraphResourceUri, _configurationService.ClientId, _configurationService.ReturnUri, parent);
                _userId = Guid.Parse(authResult.UserInfo.UniqueId);
                return true;
            }
            catch (AdalServiceException)
            {
                _authenticationContext = null;
                throw;
            }
        }
    }
}