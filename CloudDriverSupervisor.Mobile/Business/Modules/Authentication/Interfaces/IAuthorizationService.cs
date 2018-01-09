namespace Mobile.Business.Modules.Authentication.Interfaces
{
    using System.Threading.Tasks;

    public interface IAuthorizationService
    {
        Task<bool> IsUserInDriverRole();
    }
}