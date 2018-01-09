namespace Mobile.Business.Modules.Authentication.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public interface IAuthenticationService
    {
        Task<bool> AuthenticateUserAsync(IPlatformParameters parent);
        Task<string> GetAuthorizationHeaderAsync();
        Guid GetUserId();
    }
}