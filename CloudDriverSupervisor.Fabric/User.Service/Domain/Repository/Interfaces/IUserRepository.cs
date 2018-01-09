namespace User.Service.Domain.Repository.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Entities;

    public interface IUserRepository
    {
        Task<dynamic> GetUsers();
        Task DeleteUser(Guid userId);
        Task AddUser(User user);
    }
}