namespace Mobile.Business.Modules.Authentication
{
    using System.Linq;
    using System.Threading.Tasks;
    using ApiClient.Interfaces;
    using Bootstrapper.Interfaces;
    using Interfaces;

    public class AuthorizationService : IAuthorizationService, ISingletonService
    {
        private readonly IApiService _apiService;

        public AuthorizationService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> IsUserInDriverRole()
        {
            var roles = await _apiService.GetUserRoles();
            return roles.Any(role => string.Equals(role, "Driver"));
        }
    }
}